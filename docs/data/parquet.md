# Parquet

Parquet is present in the published `HPD-ML-DataSources` `0.5.0` API, but it is
not implemented.

The following calls unconditionally throw `NotImplementedException`:

```csharp
IDataHandle.LoadParquet("data.parquet");
ParquetDataHandle.Create("data.parquet");
ParquetWriter.Write(data, "data.parquet");
```

`ParquetOptions.Columns`, `RowGroups`, and `BatchSize` describe the intended
API but do not currently activate a reader.

## What to do today

- Convert Parquet data to CSV or JSON Lines before loading it.
- Use an application-owned Parquet library and construct an in-memory handle
  from the resulting arrays.
- Do not ship code that catches `NotImplementedException` and treats the
  source as empty.

There is intentionally no `cookbook/Data` Parquet recipe for `0.5.0`: cookbook
files are runnable examples, not sketches of unavailable behavior.

## Planned correction

The `0.6.0` DataSources proposal requires either a tested Parquet reader and
writer or removal of the unsupported public surface. Until that work lands,
Parquet should not be listed as a supported format.

## Next

- [CSV](csv.md)
- [JSON and JSON Lines](json.md)
- [In-memory data](in-memory-data.md)
