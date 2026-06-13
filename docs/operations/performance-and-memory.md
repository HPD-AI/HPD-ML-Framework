# Performance and Memory

HPD ML separates row access from materialization, but consumers decide whether
execution remains streaming.

## Know the boundary

Cursor-backed CSV and JSON Lines handles can enumerate rows without retaining
the full file. `FilteredDataHandle` is also lazy. The following operations
materialize or buffer in published `0.5.0`:

- `Materialize()`
- `CachedDataHandle` on first consumption
- shuffling and train/test splitting
- most built-in training-data loaders
- cross-validation fold construction
- permutation feature importance inputs
- LightGBM dataset construction

Several learners copy features into jagged arrays and labels into separate
arrays. LightGBM additionally creates a row-major native-training matrix.
Estimate peak memory from the consumer, not only from the source handle.

## Repeated passes

Use `CachedDataHandle` when a repeatable source will be consumed many times and
the materialized size is acceptable. It is thread-safe for initialization
through `Lazy<T>`, but 0.5.0 does not publish a general concurrent-cursor
contract for every inner handle.

Avoid wrapping stateful transforms in `TransformedDataHandle` merely for
convenience. Each consumption calls `Apply` again and may create a fresh
execution.

## Batch access

`TryGetColumnBatch<T>` is an optional fast path. In-memory data supports it;
cursor-backed file sources commonly return `false`. Always retain a cursor
fallback unless batch access is an explicit validated requirement.

## Production safeguards

1. Record row counts, vector widths, and estimated bytes before fitting.
2. Set application limits for rows, dimensions, and file sizes.
3. Measure peak working set using production-shaped data.
4. Avoid concurrent fits when each fit materializes the same source.
5. Cache only after confirming that the memory trade is acceptable.

## Run the recipe

```bash
dotnet run cookbook/Operations/01-materialization-budget.cs
```

See also [materialization](../data/materialization.md) and
[lazy, eager, and cached execution](../concepts/lazy-eager-and-cached-execution.md).

