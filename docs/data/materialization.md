# Materialization

`Materialize()` reads a complete handle and returns an in-memory columnar copy.
Use it when the dataset fits in memory and repeated passes are worth the cost.

```csharp
IDataHandle memory = source.Materialize();
```

## What changes

Before materialization, CSV and JSON handles report:

```text
RowCount: null
Capabilities: CursorOnly
```

The resulting in-memory handle reports a known row count plus columnar, batch,
and density capabilities.

`InMemoryDataHandle.Materialize()` returns the same object because it is already
materialized.

## When it helps

Materialize when:

- training or evaluation will read the same source repeatedly;
- the original file may change between cursor passes;
- an operation benefits from numeric batch access;
- an exact row count is needed alongside retained values.

Stay lazy when:

- the source is larger than available memory;
- only one pass is needed;
- filtering or projection will discard most values;
- JSON Lines should be processed as a stream.

## Memory model

Materialization builds one CLR array per schema column. Its memory cost includes
the values, array headers, strings and nested arrays, and temporary lists used
while collecting rows.

Published `0.5.0` does not expose a spill-to-disk cache or bounded
materialization mode.

## Failure behavior

Materialization reads every schema column using its declared CLR type. A value
that was not encountered during inference can therefore fail late. Stable
production pipelines should prefer type hints or explicit schemas.

## Run the recipe

```bash
dotnet run cookbook/Data/07-materialization.cs
```

## Next

- [Reading rows](reading-rows.md)
- [Writing data](writing-data.md)
