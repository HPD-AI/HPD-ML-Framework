# Custom Rows and Cursors

`IRowCursor` is forward-only:

```csharp
public interface IRowCursor : IDisposable
{
    bool MoveNext();
    IRow Current { get; }
}
```

`Current` must throw before the first successful `MoveNext()`. Dispose any
stream, parser, reader, native handle, or inner cursor owned by the cursor.

## Row lifetime

Use the conservative lifetime rule:

```text
the current row is valid until the cursor advances or is disposed
```

Consumers that retain data should copy values, not retain the `IRow` object.
This matches streaming sources whose current row references parser-owned
storage.

## Typed access

`IRow.GetValue<T>` is not an arbitrary conversion API. Prefer exact schema CLR
types:

```csharp
var column = row.Schema.FindByName("Temperature")
    ?? throw new KeyNotFoundException();

if (column.Type.ClrType != typeof(float))
    throw new InvalidCastException();

float value = row.GetValue<float>("Temperature");
```

For custom rows:

- missing `GetValue<T>` calls should throw;
- missing `TryGetValue<T>` calls should return `false`;
- incompatible `GetValue<T>` calls should throw `InvalidCastException`;
- incompatible `TryGetValue<T>` calls should return `false`;
- never reinterpret the bits of one value type as another.

Published 0.5.0's `HPD.ML.Core.Row` uses unsafe value-type reads, so requesting
a mismatched type can silently corrupt values. Custom implementations should
not copy that behavior. `DictionaryRow` is safer when constructing transform
outputs because it performs a limited set of numeric conversions.

## Projection

Store the active-column set in the cursor or row and reject inactive reads.
Wrappers must request every source column needed for their computation, even
when the caller only requests newly generated output columns.

## Run the recipe

```bash
dotnet run cookbook/Extending/02-custom-row-and-cursor.cs
```
