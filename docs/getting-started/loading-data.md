# Loading Data

HPD ML uses `IDataHandle` for every data source. Once data is represented as a
handle, the same transforms, learners, models, and evaluation APIs work whether
the rows came from memory, CSV, JSON, or Parquet.

Install the data-source package:

```bash
dotnet add package HPD-ML-DataSources
```

## Load in-memory columns

Use `InMemoryDataHandle.FromColumns(...)` when your application already owns
the arrays:

```csharp
using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Temperature", new[] { 18.5f, 21.0f, 23.5f }),
    ("IsWarm", new[] { false, true, true }));
```

The schema is inferred from each array's element type. See
[In-memory data](../data/in-memory-data.md) for explicit schemas, feature
vectors, dictionaries, and typed objects.

## Load CSV

`IDataHandle.LoadCsv(...)` infers column names and types:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.DataSources;

IDataHandle data = IDataHandle.LoadCsv("readings.csv");
```

CSV data is read lazily through a cursor. Creating the handle scans rows for
schema inference but does not materialize the complete file.

Configure parsing and inference with `CsvOptions`:

```csharp
var data = IDataHandle.LoadCsv(
    "readings.tsv",
    new CsvOptions
    {
        Separator = '\t',
        HasHeader = true,
        InferenceScanRows = 500,
        CommentPrefix = '#',
        SkipRows = 0,
        MaxRows = 10_000,
        MissingValuePolicy = MissingValuePolicy.NaN,
        TypeHints = new Dictionary<string, Type>
        {
            ["Temperature"] = typeof(float)
        }
    });
```

Set `InferenceScanRows` to `0` to scan all rows during inference. Type hints
override inferred types.

For a stable production schema, bypass inference:

```csharp
using HPD.ML.Core;

var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddColumn<float>("Temperature")
    .AddColumn<bool>("IsWarm")
    .Build();

var data = IDataHandle.LoadCsv("readings.csv", schema);
```

An explicit schema avoids inference changes when early rows do not represent
the full file.

## Load JSON or JSON Lines

`LoadJson(...)` supports a JSON array or one object per line:

```csharp
IDataHandle arrayData = IDataHandle.LoadJson("readings.json");

IDataHandle lineData = IDataHandle.LoadJson(
    "readings.jsonl",
    new JsonOptions { IsJsonLines = true });
```

When `IsJsonLines` is not set, HPD ML auto-detects the format from the first
JSON character: `[` selects an array and `{` selects JSON Lines.

Use property mappings, type hints, and flattening for application-shaped JSON:

```csharp
var data = IDataHandle.LoadJson(
    "events.jsonl",
    new JsonOptions
    {
        IsJsonLines = true,
        MaxFlattenDepth = 1,
        PropertyMapping = new Dictionary<string, string>
        {
            ["device.temperature"] = "Temperature"
        },
        TypeHints = new Dictionary<string, Type>
        {
            ["Temperature"] = typeof(float)
        }
    });
```

Nested properties use dotted names such as `device.temperature` when
flattening is enabled.

## Load Parquet

Parquet preserves typed columnar data and supports projection:

```csharp
IDataHandle data = IDataHandle.LoadParquet(
    "readings.parquet",
    new ParquetOptions
    {
        Columns = ["Id", "Temperature", "IsWarm"],
        BatchSize = 4096
    });
```

Use `RowGroups` to read selected row groups:

```csharp
var partition = IDataHandle.LoadParquet(
    "readings.parquet",
    new ParquetOptions { RowGroups = [0, 2] });
```

Parquet is preferable when large datasets are already stored in a typed,
columnar format.

## Inspect the schema

Inspect names and CLR types before selecting learner columns:

```csharp
foreach (var column in data.Schema.Columns)
{
    Console.WriteLine(
        $"{column.Name}: {column.Type.ClrType.Name}, vector={column.Type.IsVector}");
}
```

`RowCount` can be `null` for lazy or streaming sources. Enumerate a cursor when
an exact count is required.

## Read only needed columns

Pass the required column names to `GetCursor(...)`:

```csharp
using var rows = data.GetCursor(["Id", "Temperature"]);

while (rows.MoveNext())
{
    var id = rows.Current.GetValue<int>("Id");
    var temperature = rows.Current.GetValue<double>("Temperature");
    Console.WriteLine($"{id}: {temperature}");
}
```

CSV and JSON can avoid constructing unused row values. Parquet can additionally
push column projection into its reader.

## Materialize repeated reads

CSV and JSON handles are cursor-based. Materialize them when the complete
dataset fits in memory and will be read repeatedly:

```csharp
IDataHandle inMemory = data.Materialize();
```

Materialization trades memory usage for columnar and batch access.

## Run the cookbook example

The recipe creates temporary CSV and JSON Lines files, then compares them with
an in-memory handle:

```bash
dotnet run cookbook/GettingStarted/03-loading-data.cs
```

See
[`cookbook/GettingStarted/03-loading-data.cs`](../../cookbook/GettingStarted/03-loading-data.cs).

## Common problems

An empty CSV file cannot provide a header or inferred schema. An empty dynamic
JSON or dictionary source likewise needs an explicit schema or at least one
record.

Inference scans only the configured number of rows. Use type hints or an
explicit schema when later rows may contain wider numeric types or missing
values.

`GetValue<T>(...)` must use the stored CLR type. CSV decimal values infer as
`double` unless a type hint or explicit schema selects `float`.

Missing CSV values follow `MissingValuePolicy`: use default values, `NaN` for
floating-point columns, or throw an error.

## Next

- [Prepare data](preparing-data.md)
- [In-memory data details](../data/in-memory-data.md)

Dedicated CSV, JSON, and Parquet guides are planned for the data documentation
track.
