# CSV

`HPD-ML-DataSources` loads CSV and other single-character-delimited text into a
lazy, strictly ordered `IDataHandle`.

## Load a file

```csharp
using HPD.ML.Abstractions;
using HPD.ML.DataSources;

IDataHandle data = IDataHandle.LoadCsv("readings.csv");
```

Creating the handle reads the header and scans records for schema inference.
The complete dataset is not retained in memory. Each cursor opens and reads the
file again.

## Inferred types

Published `0.5.0` infers:

```text
int -> long -> double -> string
bool -> string when mixed with another type
```

Empty fields do not contribute to inference. Decimal text infers as `double`;
use a type hint or explicit schema for `float`.

```csharp
var data = IDataHandle.LoadCsv(
    "readings.csv",
    new CsvOptions
    {
        InferenceScanRows = 500,
        TypeHints = new Dictionary<string, Type>
        {
            ["Temperature"] = typeof(float)
        }
    });
```

`InferenceScanRows = 0` scans all records in CSV. The scan still does not
materialize the source, so later cursor reads reopen the file.

## Stable schemas

Use an explicit schema when column types must not depend on sampled values:

```csharp
using HPD.ML.Core;

var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddColumn<float>("Temperature")
    .Build();

var data = IDataHandle.LoadCsv("readings.csv", schema);
```

Schema column order maps to field order. Names are case-sensitive.

## Parsing options

`CsvOptions` supports:

- `Separator`
- `HasHeader`
- `Quote`
- `CommentPrefix`
- `Encoding`
- `TypeHints`
- `InferenceScanRows`
- `MissingValuePolicy`
- `SkipRows`
- `MaxRows`

Without a header, columns are named `Column0`, `Column1`, and so on.
`SkipRows` applies after the header. Comment detection checks the first
character of a physical line.

## Missing values

`DefaultValue` returns `default(T)`. `NaN` returns `NaN` for `float` and
`double`. `Error` throws when the value is read.

```csharp
var data = IDataHandle.LoadCsv(
    "readings.csv",
    new CsvOptions { MissingValuePolicy = MissingValuePolicy.NaN });
```

## Current 0.5.0 limitations

- The parser is line-based. Quoted fields cannot safely contain newlines.
- The writer can emit quoted newlines that the reader cannot round-trip.
- `float` and `double` parsing use invariant culture. `int` and `long` parsing,
  plus all writer formatting, currently use the process culture.
- `RowCount` remains `null` until materialization.
- Requested cursor columns are accepted as a projection hint, but row objects
  do not enforce inactive-column access.

These issues are planned for correction in `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/Data/03-csv.cs
```

## Next

- [Schemas and columns](schemas-and-columns.md)
- [Reading rows](reading-rows.md)
- [Writing data](writing-data.md)
