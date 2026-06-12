# Data

HPD ML represents source data, transformed rows, predictions, and metrics with
`IDataHandle`. A handle combines an immutable schema with one or more ways to
consume values.

Install the packages used by this track:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-DataSources
```

## Choose a guide

| Task | Guide | Cookbook |
| --- | --- | --- |
| Build data from arrays | [In-memory data](in-memory-data.md) | `01-in-memory-columns.cs` |
| Use dictionaries or typed records | [In-memory data](in-memory-data.md) | `02-dictionaries-and-typed-rows.cs` |
| Load delimited text | [CSV](csv.md) | `03-csv.cs` |
| Load JSON or JSON Lines | [JSON](json.md) | `04-json-and-json-lines.cs` |
| Define exact columns and types | [Schemas and columns](schemas-and-columns.md) | `05-explicit-schema.cs` |
| Iterate rows | [Reading rows](reading-rows.md) | `06-cursors-and-streaming.cs` |
| Cache a complete source in memory | [Materialization](materialization.md) | `07-materialization.cs` |
| Export data | [Writing data](writing-data.md) | `08-writing-csv.cs` |
| Understand current Parquet status | [Parquet](parquet.md) | No runnable 0.5.0 recipe |

## The shared contract

Every handle exposes:

- `Schema`: ordered column names, CLR types, vector dimensions, and annotations
- `RowCount`: an exact count when it is known without enumeration
- `Ordering`: the ordering guarantee made by the source
- `Capabilities`: cursor, columnar, batch, device, and density capabilities
- `GetCursor(...)`: forward-only synchronous row reading
- `StreamRows(...)`: asynchronous row enumeration
- `Materialize()`: a complete in-memory columnar copy
- `TryGetColumnBatch<T>(...)`: an optional numeric batch fast path

Transforms and models consume and return `IDataHandle`, so these reading rules
continue to apply after feature preparation and prediction.

## Published baseline

The Data cookbook is verified against published `HPD-ML-*` version `0.5.0`.
The guides call out current limitations rather than describing planned
`0.6.0` behavior as available.

Run the complete track:

```bash
for recipe in cookbook/Data/*.cs; do dotnet run "$recipe"; done
```
