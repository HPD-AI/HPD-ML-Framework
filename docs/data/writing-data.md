# Writing Data

Published `HPD-ML-DataSources` `0.5.0` can write CSV. JSON writing is absent,
and Parquet writing is a public stub that throws `NotImplementedException`.

## Write CSV

```csharp
using HPD.ML.DataSources;

CsvWriter.Write(data, "output.csv");
await CsvWriter.WriteAsync(data, "output.csv", ct: cancellationToken);
```

Both methods write schema columns in order. The synchronous path uses a cursor;
the asynchronous path consumes `StreamRows(...)`.

`CsvOptions` controls the separator, header, quote, and encoding:

```csharp
CsvWriter.Write(
    data,
    "output.tsv",
    new CsvOptions
    {
        Separator = '\t',
        HasHeader = true
    });
```

Writer calls ignore reader-only settings such as inference limits and missing
value policy.

## Escaping

Fields containing the separator, quote character, or newline are quoted.
Embedded quotes are doubled.

Be aware that the `0.5.0` reader is line-based. Although the writer can emit a
field containing a newline, loading that file back does not correctly
reconstruct the record.

## Type formatting

The writer calls `ToString()` on values, so numeric output follows the process
culture. The CSV reader parses `float` and `double` with invariant culture but
currently parses `int` and `long` with the process culture. For portable
`0.5.0` output, run under an invariant-compatible culture or preformat
culture-sensitive values as strings.

Consistent invariant numeric formatting and parsing are planned for `0.6.0`.

## Safe output handling

Write to a temporary path and move the completed file into place when partial
output must never be observed. The current writer does not provide atomic
replacement or automatic cleanup after a failure.

## Run the recipe

```bash
dotnet run cookbook/Data/08-writing-csv.cs
```

## Next

- [CSV](csv.md)
- [Parquet status](parquet.md)
- [Materialization](materialization.md)
