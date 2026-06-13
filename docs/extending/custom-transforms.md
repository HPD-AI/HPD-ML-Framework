# Custom Transforms

Implement `ITransform` for behavior that is already known and does not need to
learn from training rows:

```csharp
public interface ITransform
{
    ISchema GetOutputSchema(ISchema inputSchema);
    IDataHandle Apply(IDataHandle input);
    TransformProperties Properties { get; }
}
```

## Keep schema computation data-free

`GetOutputSchema(...)` should:

- validate required columns and exact types;
- describe every output column;
- avoid reading rows;
- avoid mutating the input schema;
- reject output-name collisions or apply a deliberate policy.

`Apply(...)` should return rows matching that schema. A common lazy pattern is:

```csharp
return new CursorDataHandle(
    outputSchema,
    columns => new MappedCursor(
        input.GetCursor(sourceColumns),
        row => MapRow(row, outputSchema)),
    input.RowCount,
    input.Ordering);
```

`DictionaryRow` is useful for small output rows. For high-throughput paths,
write a row wrapper that delegates unchanged columns and computes only the new
values.

## Declare properties honestly

```csharp
public TransformProperties Properties => new()
{
    PreservesRowCount = true,
    IsStateful = false,
    RequiresOrdering = false
};
```

Published 0.5.0 treats these properties primarily as metadata. Core does not
centrally reject an unordered handle for a transform that requires ordering,
and resource requirements are not composed. Validate mandatory requirements
inside the extension until the 0.6.0 shared contract is implemented.

## Reuse and state

A stateless transform should allocate no mutable per-enumeration state on the
transform instance. Put cursor-specific state in the cursor. This permits the
same transform to be reused safely across datasets.

`LambdaTransform` is a useful one-off escape hatch, but it captures delegates,
is not a stable serialization contract, and is less suitable for reusable
extension packages.

## Run the recipe

```bash
dotnet run cookbook/Extending/03-custom-transform.cs
```
