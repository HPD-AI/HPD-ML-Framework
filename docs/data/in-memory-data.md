# In-Memory Data

Use an in-memory handle for generated values, application-owned arrays, tests,
and datasets already loaded by another system.

## Create columns

`InMemoryDataHandle.FromColumns(...)` infers one schema field from each array's
element type:

```csharp
using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 101, 102, 103 }),
    ("Temperature", new[] { 18.5f, 21.0f, 23.5f }));
```

`RowCount` is known and `Materialize()` returns the same handle.

All arrays must logically have equal lengths. Published `0.5.0` does not
validate this during construction; mismatches can fail later while reading.

## Feature vectors

Rows can hold arrays:

```csharp
var trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0.1f, 0.2f],
        [0.8f, 0.9f]
    }),
    ("Label", new[] { false, true }));
```

This infers the CLR element type `float[]`, but it does not infer fixed vector
dimensions. Use an explicit schema when dimensions are part of the contract:

```csharp
var schema = new SchemaBuilder()
    .AddVectorColumn<float>("Features", 2)
    .AddColumn<bool>("Label", role: "Label")
    .Build();
```

See [Schemas and columns](schemas-and-columns.md).

## Create dictionary rows

`FromDictionaries(...)` examines all rows, discovers every key, and widens the
built-in inferred numeric types `int`, `long`, and `double`:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.DataSources;

IReadOnlyDictionary<string, object>[] rows =
[
    new Dictionary<string, object> { ["Id"] = 1, ["Score"] = 2 },
    new Dictionary<string, object> { ["Id"] = 2, ["Score"] = 2.5 }
];

var data = IDataHandle.FromDictionaries(rows);
```

Here `Score` becomes `double`. Other numeric CLR types, such as `float` mixed
with `int`, may fall back to `string`; use an explicit schema when those types
must be preserved.

Missing dictionary keys are filled with the CLR default for the inferred
column type. An empty sequence cannot be inferred.

Column names and dictionary keys are case-sensitive.

## Create typed rows

Typed enumerables require a schema and extractor:

```csharp
var schema = new SchemaBuilder()
    .AddColumn("City", new FieldType(typeof(string)))
    .AddColumn<float>("Temperature")
    .Build();

var data = IDataHandle.FromEnumerable(
    readings,
    schema,
    reading => new Dictionary<string, object>
    {
        ["City"] = reading.City,
        ["Temperature"] = reading.Temperature
    });
```

This path is reflection-free and suitable for Native AOT. Extractor keys and
value types must match the schema exactly.

## Capabilities

In-memory handles report:

```text
ColumnarAccess | BatchAccess | KnownDensity
```

`TryGetColumnBatch<T>(...)` works for scalar numeric `T[]` columns. Nested
arrays such as `float[][]` do not use that scalar batch path.

## Run the recipes

```bash
dotnet run cookbook/Data/01-in-memory-columns.cs
dotnet run cookbook/Data/02-dictionaries-and-typed-rows.cs
```

## Next

- [Schemas and columns](schemas-and-columns.md)
- [Reading rows](reading-rows.md)
- [Materialization](materialization.md)
