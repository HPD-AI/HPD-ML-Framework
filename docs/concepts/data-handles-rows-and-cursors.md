# Data Handles, Rows, and Cursors

`IDataHandle` is a lazy or materialized reference to schema-bearing rows.

## Handle metadata

```csharp
Console.WriteLine(data.RowCount?.ToString() ?? "unknown");
Console.WriteLine(data.Ordering);
Console.WriteLine(data.Capabilities);
```

- `RowCount == null` means an exact count is not cheaply available.
- `Ordering` reports the claimed encounter-order guarantee.
- `Capabilities` reports optional access paths.

The cursor remains the universal baseline even when a batch path is absent.

## Read a projection

```csharp
using var cursor = data.GetCursor(["Id", "Features"]);
while (cursor.MoveNext())
{
    int id = cursor.Current.GetValue<int>("Id");
    float[] features = cursor.Current.GetValue<float[]>("Features");
}
```

A cursor is forward-only. `Current` is valid only after a successful
`MoveNext()` and before exhaustion. Dispose file-backed cursors.

`columnsNeeded` is a projection request. Published `0.5.0` implementations do
not enforce inactive-column access consistently, so code should still read only
requested columns.

## Stream rows

```csharp
await foreach (var row in data.StreamRows(cancellationToken))
    Console.WriteLine(row.GetValue<int>("Id"));
```

`IAsyncEnumerable` gives asynchronous control flow and cancellation. It does
not guarantee native asynchronous I/O; current CSV and JSON paths often adapt
synchronous parsing.

Treat source rows as iteration-scoped views. Copy values that must survive the
next cursor advance.

## Batch access

`TryGetColumnBatch<T>` is optional. Check its Boolean result and keep a cursor
fallback. In-memory `0.5.0` batch access supports scalar numeric arrays; it
does not flatten vector arrays into the advertised tensor shape.

## Materialization

`Materialize()` consumes the complete handle and returns an in-memory columnar
copy. Use it when repeated passes justify the memory cost.

## Run the recipe

```bash
dotnet run cookbook/Concepts/02-cursors-streaming-and-materialization.cs
```

## Next

- [Lazy, eager, and cached execution](lazy-eager-and-cached-execution.md)
- [Reading rows](../data/reading-rows.md)
