using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

try
{
    var options = Options.Parse(args);
    var packages = PackageInput.Discover(options.PackagesRoot, options.Version);
    if (packages.Count == 0)
    {
        throw new InvalidOperationException(
            $"No HPD-ML-* {options.Version} assemblies were found below '{options.PackagesRoot}'.");
    }

    var toolDirectory = AppContext.BaseDirectory;
    var guideMapPath = Path.GetFullPath(
        Path.Combine(toolDirectory, "..", "..", "..", "..", "guide-links.json"));
    if (!File.Exists(guideMapPath))
    {
        guideMapPath = Path.Combine(Directory.GetCurrentDirectory(), "tools", "api-reference", "guide-links.json");
    }

    var guideLinks = File.Exists(guideMapPath)
        ? JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(guideMapPath))
            ?? new Dictionary<string, string>(StringComparer.Ordinal)
        : new Dictionary<string, string>(StringComparer.Ordinal);

    var model = ApiReader.Read(packages, guideLinks);
    var outputParent = Path.GetDirectoryName(options.Output)
        ?? throw new InvalidOperationException($"Output '{options.Output}' has no parent directory.");
    Directory.CreateDirectory(outputParent);
    var temporaryOutput = Path.Combine(outputParent, $".api-reference-{Guid.NewGuid():N}");

    try
    {
        MarkdownWriter.Write(model, temporaryOutput, options.Version);
        LinkValidator.Validate(temporaryOutput);

        if (options.Command == "check")
        {
            DirectoryComparer.EnsureEqual(temporaryOutput, options.Output);
            Console.WriteLine($"API reference is current ({model.Types.Count} types).");
        }
        else
        {
            DirectorySync.Replace(temporaryOutput, options.Output);
            Console.WriteLine(
                $"Generated {model.Types.Count} type pages across {model.Packages.Count} packages.");
        }
    }
    finally
    {
        if (Directory.Exists(temporaryOutput))
        {
            Directory.Delete(temporaryOutput, recursive: true);
        }
    }

    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

internal sealed record Options(string Command, string PackagesRoot, string Version, string Output)
{
    public static Options Parse(string[] arguments)
    {
        if (arguments.Length == 0 || arguments[0] is not ("generate" or "check"))
        {
            throw new ArgumentException(
                "Usage: api-reference <generate|check> --packages <root> --version <version> --output <directory>");
        }

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 1; index < arguments.Length; index += 2)
        {
            if (index + 1 >= arguments.Length || !arguments[index].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Invalid argument near '{arguments[index]}'.");
            }

            values[arguments[index][2..]] = arguments[index + 1];
        }

        return new Options(
            arguments[0],
            Path.GetFullPath(Required(values, "packages")),
            Required(values, "version"),
            Path.GetFullPath(Required(values, "output")));
    }

    private static string Required(IReadOnlyDictionary<string, string> values, string name) =>
        values.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException($"Missing required option '--{name}'.");
}

internal sealed record PackageInput(
    string PackageId,
    string Version,
    string AssemblyPath,
    string XmlPath)
{
    public static IReadOnlyList<PackageInput> Discover(string root, string version)
    {
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException($"Package root '{root}' does not exist.");
        }

        return Directory
            .EnumerateDirectories(root, "hpd-ml-*", SearchOption.TopDirectoryOnly)
            .Select(directory =>
            {
                var packageId = ReadPackageId(directory, version);
                var frameworkDirectory = Path.Combine(directory, version, "lib", "net10.0");
                if (!Directory.Exists(frameworkDirectory))
                {
                    return null;
                }

                var assembly = Directory.EnumerateFiles(frameworkDirectory, "HPD.ML.*.dll").SingleOrDefault();
                if (assembly is null)
                {
                    return null;
                }

                return new PackageInput(
                    packageId,
                    version,
                    assembly,
                    Path.ChangeExtension(assembly, ".xml"));
            })
            .Where(item => item is not null)
            .Cast<PackageInput>()
            .OrderBy(item => item.PackageId, StringComparer.Ordinal)
            .ToArray();
    }

    private static string ReadPackageId(string packageDirectory, string version)
    {
        var nuspec = Directory
            .EnumerateFiles(Path.Combine(packageDirectory, version), "*.nuspec", SearchOption.TopDirectoryOnly)
            .SingleOrDefault();
        if (nuspec is null)
        {
            return Path.GetFileName(packageDirectory);
        }

        var document = XDocument.Load(nuspec);
        return document.Descendants().FirstOrDefault(element => element.Name.LocalName == "id")?.Value
            ?? Path.GetFileName(packageDirectory);
    }
}

internal sealed record ApiModel(
    IReadOnlyList<ApiPackage> Packages,
    IReadOnlyList<ApiType> Types,
    IReadOnlyDictionary<string, ApiType> TypeLookup);

internal sealed record ApiPackage(
    string PackageId,
    string AssemblyName,
    IReadOnlyList<ApiType> Types);

internal sealed record ApiType(
    string PackageId,
    string AssemblyName,
    string Namespace,
    string Name,
    string FullName,
    string Kind,
    string Declaration,
    string? Summary,
    string? Remarks,
    IReadOnlyList<string> BaseTypes,
    IReadOnlyList<string> ExtensionReceivers,
    IReadOnlyList<ApiMember> Members,
    string? GuideLink,
    string FileName);

internal sealed record ApiMember(
    string Group,
    string Name,
    string Declaration,
    string? Summary,
    string? Remarks);

internal static class ApiModelReaderExtensions
{
    public static string GetFullTypeName(this MetadataReader reader, TypeDefinition definition)
    {
        var name = reader.GetString(definition.Name);
        if (definition.GetDeclaringType().IsNil)
        {
            var @namespace = reader.GetString(definition.Namespace);
            return string.IsNullOrEmpty(@namespace) ? name : $"{@namespace}.{name}";
        }

        return $"{reader.GetFullTypeName(reader.GetTypeDefinition(definition.GetDeclaringType()))}+{name}";
    }
}

internal static class ApiReader
{
    public static ApiModel Read(
        IReadOnlyList<PackageInput> packages,
        IReadOnlyDictionary<string, string> guideLinks)
    {
        var packageModels = new List<ApiPackage>();
        var allTypes = new List<ApiType>();

        foreach (var package in packages)
        {
            using var stream = File.OpenRead(package.AssemblyPath);
            using var peReader = new PEReader(stream);
            var reader = peReader.GetMetadataReader();
            var assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);
            var xml = XmlDocumentation.Load(package.XmlPath);
            var formatter = new MetadataFormatter(reader);
            var types = new List<ApiType>();

            foreach (var handle in reader.TypeDefinitions)
            {
                var definition = reader.GetTypeDefinition(handle);
                if (!IsPublic(reader, definition) || IsCompilerGenerated(reader, definition))
                {
                    continue;
                }

                var metadataFullName = reader.GetFullTypeName(definition);
                var fullName = formatter.FormatFullTypeName(definition);
                var @namespace = reader.GetString(definition.Namespace);
                if (string.IsNullOrEmpty(@namespace) && !definition.GetDeclaringType().IsNil)
                {
                    @namespace = NamespaceOf(reader.GetFullTypeName(
                        reader.GetTypeDefinition(definition.GetDeclaringType())));
                }

                var documentation = xml.Get($"T:{ToDocumentationTypeName(metadataFullName)}");
                var members = ReadMembers(reader, definition, formatter, xml);
                var type = new ApiType(
                    package.PackageId,
                    assemblyName,
                    @namespace,
                    formatter.FormatTypeDisplayName(definition),
                    fullName,
                    GetKind(reader, definition),
                    formatter.FormatTypeDeclaration(definition),
                    documentation?.Summary,
                    documentation?.Remarks,
                    formatter.GetBaseTypes(definition),
                    formatter.GetExtensionReceivers(definition),
                    members,
                    guideLinks.GetValueOrDefault(fullName),
                    $"{Slug(fullName)}.md");
                types.Add(type);
                allTypes.Add(type);
            }

            packageModels.Add(new ApiPackage(
                package.PackageId,
                assemblyName,
                types.OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray()));
        }

        var orderedTypes = allTypes.OrderBy(type => type.FullName, StringComparer.Ordinal).ToArray();
        return new ApiModel(
            packageModels,
            orderedTypes,
            orderedTypes.ToDictionary(type => type.FullName, StringComparer.Ordinal));
    }

    private static IReadOnlyList<ApiMember> ReadMembers(
        MetadataReader reader,
        TypeDefinition definition,
        MetadataFormatter formatter,
        XmlDocumentation xml)
    {
        var members = new List<ApiMember>();
        var isEnum = !definition.BaseType.IsNil &&
            formatter.FormatType(definition.BaseType) == "System.Enum";

        foreach (var handle in definition.GetMethods())
        {
            var method = reader.GetMethodDefinition(handle);
            var name = reader.GetString(method.Name);
            if (name == "<Clone>$")
            {
                continue;
            }
            var isDocumentedSpecialMethod =
                name is ".ctor" or ".cctor" ||
                name.StartsWith("op_", StringComparison.Ordinal);
            if ((method.Attributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public ||
                ((method.Attributes & MethodAttributes.SpecialName) != 0 && !isDocumentedSpecialMethod))
            {
                continue;
            }

            var documentation = xml.FindMethod(reader.GetFullTypeName(definition), name);
            members.Add(new ApiMember(
                name is ".ctor" or ".cctor" ? "Constructors" : "Methods",
                formatter.FormatMethodDisplayName(name, definition),
                formatter.FormatMethod(method, definition),
                documentation?.Summary,
                documentation?.Remarks));
        }

        foreach (var handle in definition.GetProperties())
        {
            var property = reader.GetPropertyDefinition(handle);
            var accessors = property.GetAccessors();
            if (!IsPublic(reader, accessors.Getter) && !IsPublic(reader, accessors.Setter))
            {
                continue;
            }

            var name = reader.GetString(property.Name);
            var documentation = xml.Get($"P:{ToDocumentationTypeName(reader.GetFullTypeName(definition))}.{name}");
            members.Add(new ApiMember(
                "Properties",
                name,
                formatter.FormatProperty(property, accessors, definition),
                documentation?.Summary,
                documentation?.Remarks));
        }

        foreach (var handle in definition.GetEvents())
        {
            var @event = reader.GetEventDefinition(handle);
            var accessors = @event.GetAccessors();
            if (!IsPublic(reader, accessors.Adder) && !IsPublic(reader, accessors.Remover))
            {
                continue;
            }

            var name = reader.GetString(@event.Name);
            var documentation = xml.Get($"E:{ToDocumentationTypeName(reader.GetFullTypeName(definition))}.{name}");
            members.Add(new ApiMember(
                "Events",
                name,
                formatter.FormatEvent(@event),
                documentation?.Summary,
                documentation?.Remarks));
        }

        foreach (var handle in definition.GetFields())
        {
            var field = reader.GetFieldDefinition(handle);
            if ((field.Attributes & FieldAttributes.FieldAccessMask) != FieldAttributes.Public ||
                (field.Attributes & FieldAttributes.SpecialName) != 0)
            {
                continue;
            }

            var name = reader.GetString(field.Name);
            var documentation = xml.Get($"F:{ToDocumentationTypeName(reader.GetFullTypeName(definition))}.{name}");
            members.Add(new ApiMember(
                isEnum ? "Values" : "Fields",
                name,
                formatter.FormatField(field, isEnum),
                documentation?.Summary,
                documentation?.Remarks));
        }

        return members
            .OrderBy(member => member.Group, StringComparer.Ordinal)
            .ThenBy(member => member.Name, StringComparer.Ordinal)
            .ThenBy(member => member.Declaration, StringComparer.Ordinal)
            .ToArray();
    }

    private static bool IsPublic(MetadataReader reader, TypeDefinition definition)
    {
        var visibility = definition.Attributes & TypeAttributes.VisibilityMask;
        if (visibility == TypeAttributes.Public)
        {
            return true;
        }

        return visibility == TypeAttributes.NestedPublic &&
            !definition.GetDeclaringType().IsNil &&
            IsPublic(reader, reader.GetTypeDefinition(definition.GetDeclaringType()));
    }

    private static bool IsPublic(MetadataReader reader, MethodDefinitionHandle handle)
    {
        if (handle.IsNil)
        {
            return false;
        }

        var method = reader.GetMethodDefinition(handle);
        return (method.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
    }

    private static bool IsCompilerGenerated(MetadataReader reader, TypeDefinition definition)
    {
        var name = reader.GetString(definition.Name);
        return name.StartsWith("<", StringComparison.Ordinal) ||
            HasAttribute(reader, definition.GetCustomAttributes(), "CompilerGeneratedAttribute");
    }

    internal static bool HasAttribute(
        MetadataReader reader,
        CustomAttributeHandleCollection attributes,
        string attributeName)
    {
        foreach (var handle in attributes)
        {
            var attribute = reader.GetCustomAttribute(handle);
            var constructor = attribute.Constructor;
            string? name = constructor.Kind switch
            {
                HandleKind.MemberReference => GetAttributeTypeName(
                    reader,
                    reader.GetMemberReference((MemberReferenceHandle)constructor).Parent),
                HandleKind.MethodDefinition => reader.GetString(
                    reader.GetTypeDefinition(
                        reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType()).Name),
                _ => null
            };

            if (name == attributeName)
            {
                return true;
            }
        }

        return false;
    }

    internal static string? GetAttributeTypeName(MetadataReader reader, EntityHandle handle) =>
        handle.Kind switch
        {
            HandleKind.TypeReference => reader.GetString(reader.GetTypeReference((TypeReferenceHandle)handle).Name),
            HandleKind.TypeDefinition => reader.GetString(reader.GetTypeDefinition((TypeDefinitionHandle)handle).Name),
            _ => null
        };

    private static string GetKind(MetadataReader reader, TypeDefinition definition)
    {
        if ((definition.Attributes & TypeAttributes.Interface) != 0)
        {
            return "interface";
        }

        var baseType = definition.BaseType.IsNil ? string.Empty : new MetadataFormatter(reader).FormatType(definition.BaseType);
        if (baseType == "System.Enum")
        {
            return "enum";
        }

        if (baseType == "System.ValueType")
        {
            return "struct";
        }

        if (baseType == "System.MulticastDelegate")
        {
            return "delegate";
        }

        return "class";
    }

    private static string NamespaceOf(string fullName)
    {
        var nested = fullName.IndexOf('+', StringComparison.Ordinal);
        var outer = nested >= 0 ? fullName[..nested] : fullName;
        var separator = outer.LastIndexOf('.');
        return separator < 0 ? string.Empty : outer[..separator];
    }

    private static string ToDocumentationTypeName(string fullName) => fullName.Replace('+', '.');
    internal static string TrimArity(string name)
    {
        var builder = new StringBuilder(name.Length);
        for (var index = 0; index < name.Length; index++)
        {
            if (name[index] == '`')
            {
                while (index + 1 < name.Length && char.IsDigit(name[index + 1]))
                {
                    index++;
                }
                continue;
            }

            builder.Append(name[index]);
        }

        return builder.ToString();
    }

    internal static string Slug(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (var character in value.ToLowerInvariant())
        {
            builder.Append(char.IsLetterOrDigit(character) ? character : '-');
        }

        return string.Join('-', builder.ToString().Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}

internal sealed class MetadataFormatter : ISignatureTypeProvider<string, GenericContext>
{
    private readonly MetadataReader _reader;

    public MetadataFormatter(MetadataReader reader) => _reader = reader;

    public string FormatTypeDeclaration(TypeDefinition definition)
    {
        var visibility = definition.GetDeclaringType().IsNil ? "public" : "public";
        var kind = GetDeclarationKind(definition);
        var modifiers = new List<string> { visibility };

        if (kind == "class")
        {
            if ((definition.Attributes & TypeAttributes.Abstract) != 0 &&
                (definition.Attributes & TypeAttributes.Sealed) != 0)
            {
                modifiers.Add("static");
            }
            else
            {
                if ((definition.Attributes & TypeAttributes.Abstract) != 0)
                {
                    modifiers.Add("abstract");
                }
                if ((definition.Attributes & TypeAttributes.Sealed) != 0)
                {
                    modifiers.Add("sealed");
                }
            }
        }

        modifiers.Add(kind);
        var name = FormatTypeDisplayName(definition);
        var bases = GetBaseTypes(definition)
            .Where(type => type is not "System.Object" and not "System.ValueType" and not "System.Enum" and not "System.MulticastDelegate")
            .ToArray();
        return $"{string.Join(' ', modifiers)} {name}{(bases.Length == 0 ? string.Empty : $" : {string.Join(", ", bases)}")}";
    }

    public string FormatFullTypeName(TypeDefinition definition)
    {
        if (!definition.GetDeclaringType().IsNil)
        {
            return $"{FormatFullTypeName(_reader.GetTypeDefinition(definition.GetDeclaringType()))}.{FormatTypeDisplayName(definition)}";
        }

        var @namespace = _reader.GetString(definition.Namespace);
        var name = FormatTypeDisplayName(definition);
        return string.IsNullOrEmpty(@namespace) ? name : $"{@namespace}.{name}";
    }

    public IReadOnlyList<string> GetBaseTypes(TypeDefinition definition)
    {
        var result = new List<string>();
        if (!definition.BaseType.IsNil)
        {
            result.Add(FormatType(definition.BaseType));
        }

        result.AddRange(definition.GetInterfaceImplementations()
            .Select(handle => FormatType(_reader.GetInterfaceImplementation(handle).Interface)));
        return result.Distinct(StringComparer.Ordinal).ToArray();
    }

    public IReadOnlyList<string> GetExtensionReceivers(TypeDefinition definition)
    {
        if (!ApiReader.HasAttribute(_reader, definition.GetCustomAttributes(), "ExtensionAttribute"))
        {
            return Array.Empty<string>();
        }

        var receivers = new HashSet<string>(StringComparer.Ordinal);
        foreach (var nestedHandle in definition.GetNestedTypes())
        {
            var group = _reader.GetTypeDefinition(nestedHandle);
            if (!ApiReader.HasAttribute(_reader, group.GetCustomAttributes(), "ExtensionAttribute"))
            {
                continue;
            }

            foreach (var markerHandle in group.GetNestedTypes())
            {
                var marker = _reader.GetTypeDefinition(markerHandle);
                foreach (var methodHandle in marker.GetMethods())
                {
                    var method = _reader.GetMethodDefinition(methodHandle);
                    if (_reader.GetString(method.Name) != "<Extension>$")
                    {
                        continue;
                    }

                    var signature = method.DecodeSignature(this, GenericContext.Empty);
                    if (signature.ParameterTypes.Length == 1)
                    {
                        receivers.Add(signature.ParameterTypes[0]);
                    }
                }
            }
        }

        return receivers.OrderBy(value => value, StringComparer.Ordinal).ToArray();
    }

    public string FormatMethod(MethodDefinition method, TypeDefinition declaringType)
    {
        var signature = method.DecodeSignature(
            this,
            new GenericContext(
                GetGenericNames(declaringType.GetGenericParameters()),
                GetGenericNames(method.GetGenericParameters())));
        var name = _reader.GetString(method.Name);
        var parameters = method.GetParameters()
            .Select(handle => _reader.GetParameter(handle))
            .Where(parameter => parameter.SequenceNumber > 0)
            .OrderBy(parameter => parameter.SequenceNumber)
            .ToArray();
        var nullableContext = GetNullableContext(method.GetCustomAttributes()) ??
            GetNullableContext(declaringType.GetCustomAttributes());
        var renderedParameters = signature.ParameterTypes
            .Select((type, index) =>
            {
                var parameter = index < parameters.Length ? parameters[index] : default;
                var parameterName = index < parameters.Length
                    ? _reader.GetString(parameter.Name)
                    : $"arg{index}";
                var prefix = index < parameters.Length
                    ? GetParameterModifier(parameter.Attributes, type)
                    : string.Empty;
                var renderedType = type.TrimEnd('&');
                if (index < parameters.Length &&
                    IsNullable(parameters[index].GetCustomAttributes(), renderedType, nullableContext))
                {
                    renderedType = MakeNullable(renderedType);
                }
                var defaultValue = index < parameters.Length
                    ? FormatDefaultValue(parameters[index])
                    : string.Empty;
                return $"{prefix}{renderedType} {parameterName}{defaultValue}";
            });
        var genericNames = GetGenericNames(method.GetGenericParameters());
        var genericSuffix = genericNames.Length == 0 ? string.Empty : $"<{string.Join(", ", genericNames)}>";
        var displayName = FormatMethodDisplayName(name, declaringType);
        var modifiers = new List<string> { "public" };
        if ((method.Attributes & MethodAttributes.Static) != 0)
        {
            modifiers.Add("static");
        }
        if ((method.Attributes & MethodAttributes.Abstract) != 0)
        {
            modifiers.Add("abstract");
        }
        else if ((method.Attributes & MethodAttributes.Virtual) != 0 &&
                 (method.Attributes & MethodAttributes.Final) == 0)
        {
            modifiers.Add("virtual");
        }

        var renderedReturnType = signature.ReturnType;
        var returnParameter = method.GetParameters()
            .Select(handle => _reader.GetParameter(handle))
            .FirstOrDefault(parameter => parameter.SequenceNumber == 0);
        if (name != ".ctor" &&
            !returnParameter.Equals(default(Parameter)) &&
            IsNullable(returnParameter.GetCustomAttributes(), renderedReturnType, nullableContext))
        {
            renderedReturnType = MakeNullable(renderedReturnType);
        }
        var returnType = name == ".ctor" ? string.Empty : $"{renderedReturnType} ";
        return $"{string.Join(' ', modifiers)} {returnType}{displayName}{genericSuffix}({string.Join(", ", renderedParameters)})";
    }

    public string FormatMethodDisplayName(string metadataName, TypeDefinition declaringType) =>
        metadataName is ".ctor" or ".cctor"
            ? ApiReader.TrimArity(_reader.GetString(declaringType.Name))
            : FormatMethodName(metadataName);

    public string FormatProperty(
        PropertyDefinition property,
        PropertyAccessors accessors,
        TypeDefinition declaringType)
    {
        var signature = property.DecodeSignature(this, GenericContext.Empty);
        var propertyType = signature.ReturnType;
        var nullableContext = GetNullableContext(property.GetCustomAttributes()) ??
            GetNullableContext(declaringType.GetCustomAttributes());
        if (IsNullable(property.GetCustomAttributes(), propertyType, nullableContext))
        {
            propertyType = MakeNullable(propertyType);
        }
        var access = new List<string>();
        if (IsPublic(accessors.Getter))
        {
            access.Add("get;");
        }
        if (IsPublic(accessors.Setter))
        {
            var setter = _reader.GetMethodDefinition(accessors.Setter);
            var setterSignature = setter.DecodeSignature(this, GenericContext.Empty);
            access.Add(setterSignature.ReturnType.Contains(InitOnlyMarker, StringComparison.Ordinal)
                ? "init;"
                : "set;");
        }
        return $"public {propertyType} {_reader.GetString(property.Name)} {{ {string.Join(' ', access)} }}";
    }

    public string FormatEvent(EventDefinition @event) =>
        $"public event {FormatType(@event.Type)} {_reader.GetString(@event.Name)}";

    public string FormatField(FieldDefinition field, bool isEnum)
    {
        if (isEnum)
        {
            return _reader.GetString(field.Name);
        }

        var modifiers = new List<string> { "public" };
        if ((field.Attributes & FieldAttributes.Literal) != 0)
        {
            modifiers.Add("const");
        }
        else
        {
            if ((field.Attributes & FieldAttributes.Static) != 0)
            {
                modifiers.Add("static");
            }
            if ((field.Attributes & FieldAttributes.InitOnly) != 0)
            {
                modifiers.Add("readonly");
            }
        }
        return $"{string.Join(' ', modifiers)} {field.DecodeSignature(this, GenericContext.Empty)} {_reader.GetString(field.Name)}";
    }

    public string FormatType(EntityHandle handle) =>
        handle.Kind switch
        {
            HandleKind.TypeDefinition => GetTypeFromDefinition(
                _reader,
                (TypeDefinitionHandle)handle,
                0),
            HandleKind.TypeReference => GetTypeFromReference(
                _reader,
                (TypeReferenceHandle)handle,
                0),
            HandleKind.TypeSpecification => _reader
                .GetTypeSpecification((TypeSpecificationHandle)handle)
                .DecodeSignature(this, GenericContext.Empty),
            _ => handle.Kind.ToString()
        };

    public string GetArrayType(string elementType, ArrayShape shape)
    {
        var commas = shape.Rank <= 1 ? string.Empty : new string(',', shape.Rank - 1);
        return $"{elementType}[{commas}]";
    }

    public string GetByReferenceType(string elementType) => $"{elementType}&";
    public string GetFunctionPointerType(MethodSignature<string> signature) => "delegate*";
    public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments) =>
                $"{ApiReader.TrimArity(genericType)}<{string.Join(", ", typeArguments)}>";
    public string GetGenericMethodParameter(GenericContext genericContext, int index) =>
        index < genericContext.MethodParameters.Length ? genericContext.MethodParameters[index] : $"TMethod{index}";
    public string GetGenericTypeParameter(GenericContext genericContext, int index) =>
        index < genericContext.TypeParameters.Length ? genericContext.TypeParameters[index] : $"T{index}";
    public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired) =>
        isRequired && modifier == "System.Runtime.CompilerServices.IsExternalInit"
            ? $"{unmodifiedType}{InitOnlyMarker}"
            : unmodifiedType;
    public string GetPinnedType(string elementType) => elementType;
    public string GetPointerType(string elementType) => $"{elementType}*";
    public string GetPrimitiveType(PrimitiveTypeCode typeCode) =>
        typeCode switch
        {
            PrimitiveTypeCode.Boolean => "bool",
            PrimitiveTypeCode.Byte => "byte",
            PrimitiveTypeCode.Char => "char",
            PrimitiveTypeCode.Double => "double",
            PrimitiveTypeCode.Int16 => "short",
            PrimitiveTypeCode.Int32 => "int",
            PrimitiveTypeCode.Int64 => "long",
            PrimitiveTypeCode.IntPtr => "nint",
            PrimitiveTypeCode.Object => "object",
            PrimitiveTypeCode.SByte => "sbyte",
            PrimitiveTypeCode.Single => "float",
            PrimitiveTypeCode.String => "string",
            PrimitiveTypeCode.UInt16 => "ushort",
            PrimitiveTypeCode.UInt32 => "uint",
            PrimitiveTypeCode.UInt64 => "ulong",
            PrimitiveTypeCode.UIntPtr => "nuint",
            PrimitiveTypeCode.Void => "void",
            _ => typeCode.ToString()
        };
    public string GetSZArrayType(string elementType) => $"{elementType}[]";
    public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) =>
        ApiReader.TrimArity(reader.GetFullTypeName(reader.GetTypeDefinition(handle)).Replace('+', '.'));
    public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
    {
        var reference = reader.GetTypeReference(handle);
        var name = reader.GetString(reference.Name);
        var @namespace = reader.GetString(reference.Namespace);
        return ApiReader.TrimArity(string.IsNullOrEmpty(@namespace) ? name : $"{@namespace}.{name}");
    }
    public string GetTypeFromSpecification(
        MetadataReader reader,
        GenericContext genericContext,
        TypeSpecificationHandle handle,
        byte rawTypeKind) =>
        reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);

    public string FormatTypeDisplayName(TypeDefinition definition)
    {
        var name = ApiReader.TrimArity(_reader.GetString(definition.Name));
        var genericNames = GetGenericNames(definition.GetGenericParameters());
        return genericNames.Length == 0 ? name : $"{name}<{string.Join(", ", genericNames)}>";
    }

    private string GetDeclarationKind(TypeDefinition definition)
    {
        if ((definition.Attributes & TypeAttributes.Interface) != 0)
        {
            return "interface";
        }
        var baseType = definition.BaseType.IsNil ? string.Empty : FormatType(definition.BaseType);
        return baseType switch
        {
            "System.Enum" => "enum",
            "System.ValueType" => "struct",
            "System.MulticastDelegate" => "delegate",
            _ => "class"
        };
    }

    private ImmutableArray<string> GetGenericNames(GenericParameterHandleCollection handles) =>
        handles.Select(handle => _reader.GetString(_reader.GetGenericParameter(handle).Name)).ToImmutableArray();

    private bool IsPublic(MethodDefinitionHandle handle) =>
        !handle.IsNil &&
        (_reader.GetMethodDefinition(handle).Attributes & MethodAttributes.MemberAccessMask) ==
        MethodAttributes.Public;

    private static string GetParameterModifier(ParameterAttributes attributes, string type)
    {
        if (!type.EndsWith('&'))
        {
            return string.Empty;
        }
        if ((attributes & ParameterAttributes.Out) != 0)
        {
            return "out ";
        }
        if ((attributes & ParameterAttributes.In) != 0)
        {
            return "in ";
        }
        return "ref ";
    }

    private const string InitOnlyMarker = "|init-only|";

    private string FormatMethodName(string metadataName) =>
        metadataName switch
        {
            "op_Equality" => "operator ==",
            "op_Inequality" => "operator !=",
            "op_Implicit" => "implicit operator",
            "op_Explicit" => "explicit operator",
            _ => metadataName
        };

    private string FormatDefaultValue(Parameter parameter)
    {
        if ((parameter.Attributes & ParameterAttributes.HasDefault) == 0)
        {
            return string.Empty;
        }

        var constantHandle = parameter.GetDefaultValue();
        if (constantHandle.IsNil)
        {
            return " = default";
        }

        var constant = _reader.GetConstant(constantHandle);
        var blob = _reader.GetBlobReader(constant.Value);
        var value = constant.TypeCode switch
        {
            ConstantTypeCode.Boolean => blob.ReadBoolean() ? "true" : "false",
            ConstantTypeCode.Char => $"'{EscapeChar((char)blob.ReadUInt16())}'",
            ConstantTypeCode.SByte => blob.ReadSByte().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.Byte => blob.ReadByte().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.Int16 => blob.ReadInt16().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.UInt16 => blob.ReadUInt16().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.Int32 => blob.ReadInt32().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.UInt32 => blob.ReadUInt32().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.Int64 => blob.ReadInt64().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.UInt64 => blob.ReadUInt64().ToString(System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.Single => blob.ReadSingle().ToString("R", System.Globalization.CultureInfo.InvariantCulture) + "f",
            ConstantTypeCode.Double => blob.ReadDouble().ToString("R", System.Globalization.CultureInfo.InvariantCulture),
            ConstantTypeCode.String => $"\"{EscapeString(blob.ReadUTF16(blob.Length))}\"",
            ConstantTypeCode.NullReference => "null",
            _ => "default"
        };
        return $" = {value}";
    }

    private bool IsNullable(
        CustomAttributeHandleCollection attributes,
        string type,
        byte? nullableContext = null)
    {
        if (!CanBeNullable(type))
        {
            return false;
        }

        return GetNullableFlag(attributes) switch
        {
            1 => false,
            2 => true,
            _ => nullableContext == 2
        };
    }

    private byte? GetNullableFlag(CustomAttributeHandleCollection attributes) =>
        GetNullableAnnotation(attributes, "NullableAttribute", isContext: false);

    private byte? GetNullableContext(CustomAttributeHandleCollection attributes) =>
        GetNullableAnnotation(attributes, "NullableContextAttribute", isContext: true);

    private byte? GetNullableAnnotation(
        CustomAttributeHandleCollection attributes,
        string expectedName,
        bool isContext)
    {
        foreach (var handle in attributes)
        {
            var attribute = _reader.GetCustomAttribute(handle);
            var constructor = attribute.Constructor;
            var attributeName = constructor.Kind switch
            {
                HandleKind.MemberReference => ApiReader.GetAttributeTypeName(
                    _reader,
                    _reader.GetMemberReference((MemberReferenceHandle)constructor).Parent),
                HandleKind.MethodDefinition => _reader.GetString(
                    _reader.GetTypeDefinition(
                        _reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType()).Name),
                _ => null
            };
            if (attributeName != expectedName)
            {
                continue;
            }

            var blob = _reader.GetBlobReader(attribute.Value);
            if (blob.Length < 3 || blob.ReadUInt16() != 1)
            {
                return null;
            }

            if (isContext || blob.RemainingBytes == 3)
            {
                return blob.ReadByte();
            }

            if (blob.RemainingBytes >= 7)
            {
                var count = blob.ReadInt32();
                return count > 0 && blob.RemainingBytes > 0 ? blob.ReadByte() : null;
            }
        }

        return null;
    }

    private static bool CanBeNullable(string type) =>
        type is "string" or "object" ||
        type.EndsWith("[]", StringComparison.Ordinal) ||
        (!IsValueTypeKeyword(type) &&
         !type.StartsWith("System.Nullable<", StringComparison.Ordinal) &&
         !type.EndsWith("*", StringComparison.Ordinal));

    private static bool IsValueTypeKeyword(string type) =>
        type is "bool" or "byte" or "sbyte" or "char" or "short" or "ushort" or
            "int" or "uint" or "long" or "ulong" or "nint" or "nuint" or
            "float" or "double" or "decimal" or "void";

    private static string MakeNullable(string type) =>
        type.EndsWith("?", StringComparison.Ordinal) ? type : $"{type}?";

    private static string EscapeString(string value) =>
        value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);

    private static string EscapeChar(char value) =>
        value switch
        {
            '\\' => "\\\\",
            '\'' => "\\'",
            '\r' => "\\r",
            '\n' => "\\n",
            '\t' => "\\t",
            _ => value.ToString()
        };
}

internal readonly record struct GenericContext(
    ImmutableArray<string> TypeParameters,
    ImmutableArray<string> MethodParameters)
{
    public static GenericContext Empty { get; } = new(
        ImmutableArray<string>.Empty,
        ImmutableArray<string>.Empty);
}

internal sealed record XmlMember(string? Summary, string? Remarks);

internal sealed class XmlDocumentation
{
    private readonly IReadOnlyDictionary<string, XmlMember> _members;

    private XmlDocumentation(IReadOnlyDictionary<string, XmlMember> members) => _members = members;

    public static XmlDocumentation Load(string path)
    {
        if (!File.Exists(path))
        {
            return new XmlDocumentation(new Dictionary<string, XmlMember>(StringComparer.Ordinal));
        }

        var document = XDocument.Load(path, LoadOptions.PreserveWhitespace);
        var members = document.Descendants("member")
            .Where(element => element.Attribute("name") is not null)
            .ToDictionary(
                element => element.Attribute("name")!.Value,
                element => new XmlMember(
                    Normalize(element.Element("summary")),
                    Normalize(element.Element("remarks"))),
                StringComparer.Ordinal);
        return new XmlDocumentation(members);
    }

    public XmlMember? Get(string id) => _members.GetValueOrDefault(id);

    public XmlMember? FindMethod(string fullTypeName, string methodName)
    {
        var prefix = $"M:{fullTypeName.Replace('+', '.')}.{methodName}";
        return _members.FirstOrDefault(pair => pair.Key.StartsWith(prefix, StringComparison.Ordinal)).Value;
    }

    private static string? Normalize(XElement? element)
    {
        if (element is null)
        {
            return null;
        }

        var text = string.Join(" ", element.Nodes().Select(RenderNode))
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal);
        while (text.Contains("  ", StringComparison.Ordinal))
        {
            text = text.Replace("  ", " ", StringComparison.Ordinal);
        }
        text = text.Trim();
        return text.Length == 0 ? null : text;
    }

    private static string RenderNode(XNode node) =>
        node switch
        {
            XText text => text.Value,
            XElement element when element.Name.LocalName is "see" or "seealso" =>
                $"`{CleanReference(element.Attribute("cref")?.Value ?? element.Attribute("href")?.Value ?? string.Empty)}`",
            XElement element when element.Name.LocalName is "c" or "paramref" or "typeparamref" =>
                $"`{element.Attribute("name")?.Value ?? element.Value}`",
            XElement element => string.Join(" ", element.Nodes().Select(RenderNode)),
            _ => string.Empty
        };

    private static string CleanReference(string reference)
    {
        if (reference.Length > 2 && reference[1] == ':')
        {
            reference = reference[2..];
        }
        return reference.Replace('{', '<').Replace('}', '>');
    }
}

internal static class MarkdownWriter
{
    private const string GeneratedNotice =
        "<!-- Generated by tools/api-reference. Do not edit by hand. -->";

    public static void Write(ApiModel model, string output, string version)
    {
        Directory.CreateDirectory(output);
        Directory.CreateDirectory(Path.Combine(output, "packages"));
        Directory.CreateDirectory(Path.Combine(output, "namespaces"));
        Directory.CreateDirectory(Path.Combine(output, "types"));

        WriteIndex(model, output, version);
        WritePackagePages(model, output, version);
        WriteNamespacePages(model, output, version);
        WriteTypePages(model, output, version);
        WriteManifest(model, output, version);
    }

    private static void WriteIndex(ApiModel model, string output, string version)
    {
        var builder = Header("HPD ML API Reference");
        builder.AppendLine();
        builder.AppendLine($"This reference is generated from the published `HPD-ML-*` `{version}`");
        builder.AppendLine("NuGet assemblies and their attached XML documentation.");
        builder.AppendLine();
        builder.AppendLine("> Undocumented entries are labeled explicitly. Generated pages describe the");
        builder.AppendLine("> shipped API surface; use the authored guides for behavioral contracts and");
        builder.AppendLine("> known limitations.");
        builder.AppendLine();
        builder.AppendLine("## Packages");
        builder.AppendLine();
        foreach (var package in model.Packages)
        {
            builder.AppendLine(
                $"- [{DisplayPackage(package.PackageId)}](packages/{ApiReader.Slug(package.PackageId)}.md) " +
                $"({package.Types.Count} exported types)");
        }
        builder.AppendLine();
        builder.AppendLine("## Namespaces");
        builder.AppendLine();
        foreach (var group in model.Types.GroupBy(type => type.Namespace).OrderBy(group => group.Key, StringComparer.Ordinal))
        {
            builder.AppendLine(
                $"- [{Code(group.Key)}](namespaces/{ApiReader.Slug(group.Key)}.md) ({group.Count()} types)");
        }
        File.WriteAllText(Path.Combine(output, "index.md"), builder.ToString());
    }

    private static void WritePackagePages(ApiModel model, string output, string version)
    {
        foreach (var package in model.Packages)
        {
            var builder = Header(DisplayPackage(package.PackageId));
            builder.AppendLine();
            builder.AppendLine($"- Package: `{DisplayPackage(package.PackageId)}`");
            builder.AppendLine($"- Version: `{version}`");
            builder.AppendLine($"- Assembly: `{package.AssemblyName}`");
            builder.AppendLine($"- Exported types: {package.Types.Count}");
            builder.AppendLine();
            foreach (var namespaceGroup in package.Types.GroupBy(type => type.Namespace).OrderBy(group => group.Key))
            {
                builder.AppendLine($"## {namespaceGroup.Key}");
                builder.AppendLine();
                foreach (var type in namespaceGroup)
                {
                    builder.AppendLine($"- [{Code(type.Name)}](../types/{type.FileName}) - {Summary(type.Summary)}");
                }
                builder.AppendLine();
            }
            File.WriteAllText(
                Path.Combine(output, "packages", $"{ApiReader.Slug(package.PackageId)}.md"),
                builder.ToString());
        }
    }

    private static void WriteNamespacePages(ApiModel model, string output, string version)
    {
        foreach (var group in model.Types.GroupBy(type => type.Namespace).OrderBy(group => group.Key))
        {
            var builder = Header(group.Key);
            builder.AppendLine();
            builder.AppendLine($"Generated from published HPD ML `{version}` packages.");
            builder.AppendLine();
            foreach (var packageGroup in group.GroupBy(type => type.PackageId).OrderBy(item => item.Key))
            {
                builder.AppendLine($"## {DisplayPackage(packageGroup.Key)}");
                builder.AppendLine();
                foreach (var type in packageGroup.OrderBy(type => type.FullName))
                {
                    builder.AppendLine($"- [{Code(type.Name)}](../types/{type.FileName}) - {Summary(type.Summary)}");
                }
                builder.AppendLine();
            }
            File.WriteAllText(
                Path.Combine(output, "namespaces", $"{ApiReader.Slug(group.Key)}.md"),
                builder.ToString());
        }
    }

    private static void WriteTypePages(ApiModel model, string output, string version)
    {
        foreach (var type in model.Types)
        {
            var builder = Header(type.FullName);
            builder.AppendLine();
            builder.AppendLine($"- Package: `{DisplayPackage(type.PackageId)}` `{version}`");
            builder.AppendLine($"- Assembly: `{type.AssemblyName}`");
            builder.AppendLine($"- Namespace: `{type.Namespace}`");
            builder.AppendLine($"- Kind: `{type.Kind}`");
            if (type.GuideLink is not null)
            {
                builder.AppendLine($"- Guide: [Related documentation]({type.GuideLink})");
            }
            builder.AppendLine();
            WriteDocumentation(builder, type.Summary, type.Remarks, type.PackageId, version);
            builder.AppendLine();
            builder.AppendLine("## Declaration");
            builder.AppendLine();
            builder.AppendLine("```csharp");
            builder.AppendLine(type.Declaration);
            builder.AppendLine("```");

            if (type.ExtensionReceivers.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("## Extension Discovery");
                builder.AppendLine();
                builder.AppendLine($"Import `{type.Namespace}` to discover extension members on:");
                builder.AppendLine();
                foreach (var receiver in type.ExtensionReceivers)
                {
                    builder.AppendLine($"- `{receiver}`");
                }
            }

            foreach (var group in type.Members.GroupBy(member => member.Group).OrderBy(group => GroupOrder(group.Key)))
            {
                builder.AppendLine();
                builder.AppendLine($"## {group.Key}");
                foreach (var member in group)
                {
                    builder.AppendLine();
                    builder.AppendLine($"### {member.Name}");
                    builder.AppendLine();
                    builder.AppendLine("```csharp");
                    builder.AppendLine(member.Declaration);
                    builder.AppendLine("```");
                    builder.AppendLine();
                    WriteDocumentation(builder, member.Summary, member.Remarks, type.PackageId, version);
                }
            }

            File.WriteAllText(Path.Combine(output, "types", type.FileName), builder.ToString());
        }
    }

    private static void WriteManifest(ApiModel model, string output, string version)
    {
        var manifest = new
        {
            schemaVersion = 1,
            packageVersion = version,
            packages = model.Packages.Select(package => new
            {
                packageId = DisplayPackage(package.PackageId),
                package.AssemblyName,
                types = package.Types.Select(type => new
                {
                    type.FullName,
                    type.Kind,
                    type.Declaration,
                    extensionReceivers = type.ExtensionReceivers,
                    members = type.Members.Select(member => new
                    {
                        member.Group,
                        member.Name,
                        member.Declaration
                    })
                })
            })
        };
        File.WriteAllText(
            Path.Combine(output, "api-manifest.json"),
            JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }) + "\n");
    }

    private static StringBuilder Header(string title)
    {
        var builder = new StringBuilder();
        builder.AppendLine(GeneratedNotice);
        builder.AppendLine();
        builder.AppendLine($"# {EscapeInlineHtml(title)}");
        return builder;
    }

    private static void WriteDocumentation(
        StringBuilder builder,
        string? summary,
        string? remarks,
        string packageId,
        string version)
    {
        if (summary is null)
        {
            builder.AppendLine(
                $"Documentation: Not provided in `{DisplayPackage(packageId)}` `{version}`.");
        }
        else
        {
            builder.AppendLine(EscapeInlineHtml(summary));
        }
        if (remarks is not null)
        {
            builder.AppendLine();
            builder.AppendLine(EscapeInlineHtml(remarks));
        }
    }

    private static int GroupOrder(string group) =>
        group switch
        {
            "Constructors" => 0,
            "Properties" => 1,
            "Fields" => 2,
            "Values" => 2,
            "Events" => 3,
            "Methods" => 4,
            _ => 5
        };

    private static string Summary(string? value) =>
        value is null ? "Undocumented in the published XML file." : EscapeInlineHtml(value);
    private static string Code(string value) => $"`{value}`";
    private static string DisplayPackage(string packageId) => packageId;
    private static string EscapeInlineHtml(string value) =>
        value.Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
}

internal static class LinkValidator
{
    public static void Validate(string root)
    {
        var errors = new List<string>();
        foreach (var file in Directory.EnumerateFiles(root, "*.md", SearchOption.AllDirectories))
        {
            var content = File.ReadAllText(file);
            var index = 0;
            while ((index = content.IndexOf("](", index, StringComparison.Ordinal)) >= 0)
            {
                var end = content.IndexOf(')', index + 2);
                if (end < 0)
                {
                    break;
                }
                var link = content[(index + 2)..end];
                index = end + 1;
                if (link.StartsWith('#') ||
                    link.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    link.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var target = link.Split('#')[0];
                var resolved = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, target));
                if (!File.Exists(resolved))
                {
                    errors.Add($"{Path.GetRelativePath(root, file)} -> {link}");
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Generated reference contains broken links:\n" + string.Join("\n", errors));
        }
    }
}

internal static class DirectoryComparer
{
    public static void EnsureEqual(string expected, string actual)
    {
        if (!Directory.Exists(actual))
        {
            throw new InvalidOperationException($"Output directory '{actual}' does not exist.");
        }

        var expectedFiles = Files(expected);
        var actualFiles = Files(actual);
        var paths = expectedFiles.Keys.Union(actualFiles.Keys, StringComparer.Ordinal).OrderBy(path => path);
        var differences = paths.Where(path =>
            !expectedFiles.TryGetValue(path, out var expectedContent) ||
            !actualFiles.TryGetValue(path, out var actualContent) ||
            !expectedContent.AsSpan().SequenceEqual(actualContent)).ToArray();
        if (differences.Length > 0)
        {
            throw new InvalidOperationException(
                "Generated API reference is stale:\n" + string.Join("\n", differences));
        }
    }

    private static Dictionary<string, byte[]> Files(string root) =>
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .ToDictionary(
                path => Path.GetRelativePath(root, path),
                File.ReadAllBytes,
                StringComparer.Ordinal);
}

internal static class DirectorySync
{
    public static void Replace(string source, string destination)
    {
        if (Directory.Exists(destination))
        {
            Directory.Delete(destination, recursive: true);
        }
        Copy(source, destination);
    }

    private static void Copy(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var directory in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, directory)));
        }
        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)));
        }
    }
}
