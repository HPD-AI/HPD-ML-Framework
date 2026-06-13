# Extending Troubleshooting

## A custom cursor returns wrong values

Ensure `GetValue<T>` uses the schema's exact CLR type. Do not copy the unsafe
bit-reinterpretation behavior of `HPD.ML.Core.Row`.

## A transform cannot read its source column

The caller may request only the new output column. Append every source column
needed by the transform when creating the inner cursor.

## Schema and runtime rows disagree

Use one schema-building function from both `GetOutputSchema(...)` and
`Apply(...)`. Test every declared output column through a cursor.

## A stateful transform changes between enumerations

State is probably stored on the transform instance. Initialize state inside
each produced cursor.

## Cancellation does not stop fitting

`Task.Run(..., token)` does not cancel active synchronous work. Link the async
token with `LearnerInput.Environment?.CancellationToken` and check it during
row loading and bounded training steps.

## Progress never completes

Call exactly one terminal method on success, failure, or cancellation. Avoid
sharing one terminal `ProgressSubject` across repeated fits.

## An explicit backend request falls back

Treat explicit backend kinds as requirements. Match a registered provider,
validate capabilities and runtime availability, or throw an actionable error.

## ZIP save succeeds but load fails

Register an `IParameterWriter<TParams>` before saving and loading. Its
`TypeName` must exactly equal `typeof(TParams).Name` in published 0.5.0.

## An extension works from source but not from NuGet

Run a standalone file-based app against the packed package. Check public
visibility, transitive package dependencies, extension namespaces, runtime
assets, and trimming metadata.

## Related

- [Testing and packaging](testing-and-packaging.md)
- [Contract boundaries](../concepts/contract-boundaries-and-troubleshooting.md)
