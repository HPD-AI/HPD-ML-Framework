# Custom Data Handles

Implement `IDataHandle` when data should remain lazy, streaming, remote, or
stored in a representation that `InMemoryDataHandle` does not cover.

The contract has five parts:

```csharp
ISchema Schema { get; }
long? RowCount { get; }
OrderingPolicy Ordering { get; }
MaterializationCapabilities Capabilities { get; }

IRowCursor GetCursor(IEnumerable<string> columnsNeeded);
IDataHandle Materialize();
IAsyncEnumerable<IRow> StreamRows(CancellationToken ct = default);
bool TryGetColumnBatch<T>(...);
```

## Start with the cursor path

`GetCursor(...)` is the universal baseline. A custom handle can report
`CursorOnly` and return `false` from `TryGetColumnBatch<T>` until a real batch
path exists.

Use `CursorDataHandle` when only a schema and cursor factory are needed:

```csharp
IDataHandle data = new CursorDataHandle(
    schema,
    columns => new MyCursor(columns),
    rowCount: null,
    ordering: OrderingPolicy.Ordered);
```

The cursor factory may be called more than once. Open source resources inside
the produced cursor and release them from `IRowCursor.Dispose()`.

## Describe only working capabilities

Do not advertise:

- `ColumnarAccess` without a working columnar path;
- `BatchAccess` when every batch request returns `false`;
- `DeviceResident` for host-backed values;
- `KnownDensity` when sparsity is unknown.

`RowCount == null` means an exact count is not cheaply available. It does not
mean zero.

## Projection in 0.5.0

`columnsNeeded` expresses the intended projection. Custom implementations
should reject unknown names, avoid loading unrelated values, and prevent reads
of inactive columns.

Published 0.5.0 does not enforce this consistently across built-in handles.
Treat strict projection as an extension-quality requirement even though some
built-ins are currently looser.

## Materialization and streaming

`Materialize()` should return a stable in-memory snapshot. `StreamRows(ct)`
must observe cancellation between rows and release its cursor when enumeration
ends.

Values read through cursors, streams, materialization, and supported batch
paths must agree.

## Run the recipe

```bash
dotnet run cookbook/Extending/01-custom-data-handle.cs
```

## Related

- [Custom rows and cursors](custom-rows-and-cursors.md)
- [Data handles, rows, and cursors](../concepts/data-handles-rows-and-cursors.md)
