# Reading Rows

`GetCursor(...)` is the universal synchronous read path. `StreamRows(...)`
provides asynchronous enumeration with cancellation.

## Use a cursor

```csharp
using var cursor = data.GetCursor(["Id", "Temperature"]);

while (cursor.MoveNext())
{
    var id = cursor.Current.GetValue<int>("Id");
    var temperature = cursor.Current.GetValue<float>("Temperature");
}
```

A cursor is forward-only. `Current` is invalid before the first successful
`MoveNext()` and after exhaustion. Dispose file-backed cursors promptly.

Treat source rows as cursor-scoped views. In particular, JSON Lines rows in
`0.5.0` reference a parsed document that is disposed when the cursor advances.
Read or copy needed values before calling `MoveNext()` again.

Request only the columns the consumer needs. This is the intended projection
contract, though published `0.5.0` does not yet reject reads of inactive
columns consistently.

## Read exact CLR types

`GetValue<T>(...)` is typed access, not a general conversion API. `T` should
match the schema's CLR type:

```csharp
var column = data.Schema.FindByName("Temperature")
    ?? throw new InvalidOperationException("Temperature is missing.");

Console.WriteLine(column.Type.ClrType);
```

CSV decimal values normally infer as `double`. Requesting `float` without a
type hint or explicit schema will fail.

`TryGetValue<T>(...)` returns `false` for a missing or incompatible value in
many row implementations, but source-specific missing-value policies still
apply.

## Stream asynchronously

```csharp
await foreach (var row in data.StreamRows(cancellationToken))
{
    Console.WriteLine(row.GetValue<int>("Id"));
}
```

The current CSV and JSON implementations adapt a synchronous cursor; the
enumeration is asynchronous in shape but file parsing itself is not
asynchronous I/O.

## Row count

`RowCount == null` means the source cannot return an exact count cheaply. Do
not treat it as zero.

To count without retaining values:

```csharp
long count = 0;
using var cursor = data.GetCursor([]);
while (cursor.MoveNext())
    count++;
```

Published `0.5.0` sources may not fully optimize an empty projection, but the
pattern expresses that no values are needed.

## Batch access

`TryGetColumnBatch<T>(...)` is optional. In-memory scalar numeric arrays support
it; CSV and JSON return `false`. Consumers must keep a cursor fallback.

## Run the recipe

```bash
dotnet run cookbook/Data/06-cursors-and-streaming.cs
```

## Next

- [Materialization](materialization.md)
- [Schemas and columns](schemas-and-columns.md)
