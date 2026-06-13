# Lazy, Eager, and Cached Execution

HPD ML separates pipeline construction from row consumption, but not every
operation has the same execution strategy.

## Schema-only work

`GetOutputSchema(...)` and schema access should not enumerate rows:

```csharp
ISchema expected = transform.GetOutputSchema(input.Schema);
```

## Lazy handles

Cursor-backed transforms and filters commonly defer work until a cursor,
stream, materialization, writer, evaluator, or learner consumes the result.

```csharp
IDataHandle filtered = new FilteredDataHandle(
    input,
    row => row.GetValue<float>("Value") > 0);
```

The filtered handle cannot know its exact row count without enumeration, so it
reports `null`.

## Cached handles

`CachedDataHandle` materializes its inner handle on first cursor, stream,
batch, or explicit materialization request and reuses the result:

```csharp
IDataHandle cached = new CachedDataHandle(source);
```

This trades memory for stable repeated reads.

## Operations that materialize

Published `0.5.0` shuffling and train/test splitting materialize their inputs.
Most built-in learners also copy all required training rows into managed
arrays. An API returning `IDataHandle` is therefore not proof of bounded-memory
execution.

## TransformedDataHandle boundary

`TransformedDataHandle` describes itself as lazy, computes schema eagerly, and
calls `transform.Apply(source)` separately from each consumption method. Pure
transforms normally remain correct, but repeated consumption can repeat setup
and stateful transforms can create independent executions.

Prefer the handle directly returned by `transform.Apply(input)` unless a
specific API requires `TransformedDataHandle`.

## Run the recipe

```bash
dotnet run cookbook/Concepts/03-lazy-and-cached-execution.cs
```

## Next

- [Transforms and composition](transforms-and-composition.md)
- [Materialization](../data/materialization.md)
