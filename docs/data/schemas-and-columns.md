# Schemas and Columns

An `ISchema` is the immutable contract between a data source, transforms, and
learners. It describes values without reading rows.

## Inspect a schema

```csharp
foreach (var column in data.Schema.Columns)
{
    Console.WriteLine(
        $"{column.Name}: {column.Type.ClrType.Name}, " +
        $"vector={column.Type.IsVector}");
}
```

`FindByName(...)` is case-sensitive and returns `null` when the name is absent.

## Build an exact schema

```csharp
using HPD.ML.Core;

var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddColumn("City", new FieldType(typeof(string)))
    .AddVectorColumn<float>("Features", 4)
    .AddColumn<bool>("Label", role: "Label")
    .Build();
```

The generic scalar helpers require unmanaged values. Use `FieldType` directly
for reference types such as `string`.

## Field types

`IFieldType` exposes:

- `ClrType`
- `IsVector`
- `Dimensions`

A vector declares its element CLR type plus dimensions. This metadata is
separate from the runtime row value representation.

## Annotations and roles

`AddColumn<T>(name, role)` writes a boolean annotation named
`role:{role}`. Common roles include `Label`, `Feature`, `Weight`, and
`GroupId`.

```csharp
var label = schema.FindByName("Label")!;
label.Annotations.TryGetValue<bool>("role:Label", out var isLabel);
```

Roles are metadata. Individual learners may still use configured or conventional
column names.

## Inference versus explicit schemas

Use inference for exploration and small examples. Use explicit schemas when:

- later records may contain wider values;
- `float` rather than `double` is required;
- vector dimensions matter;
- annotations or roles matter;
- an empty typed source must retain columns;
- production files must obey a fixed contract.

In `0.5.0`, CSV supports explicit schema loading. JSON does not.

## Merge behavior

`MergeHorizontal(...)` combines columns. `ErrorOnConflict` rejects duplicate
names; `LastWriterWins` replaces the prior column and adds audit metadata.

`MergeVertical(...)` requires the same column count, order, names, and CLR
types. It validates schema compatibility; it does not append data itself.

## Run the recipe

```bash
dotnet run cookbook/Data/05-explicit-schema.cs
```

## Next

- [Reading rows](reading-rows.md)
- [In-memory data](in-memory-data.md)
- [CSV](csv.md)
