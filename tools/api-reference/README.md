# API Reference Generator

Generates the HPD ML Markdown API reference from published NuGet package
assemblies and their XML documentation files.

```bash
dotnet run --project tools/api-reference -- generate \
  --packages ~/.nuget/packages \
  --version 0.5.0 \
  --output docs/reference/api
```

Verify that committed output is current:

```bash
dotnet run --project tools/api-reference -- check \
  --packages ~/.nuget/packages \
  --version 0.5.0 \
  --output docs/reference/api
```

The generator uses metadata-only inspection. It does not load HPD ML
assemblies or optional native runtimes.
