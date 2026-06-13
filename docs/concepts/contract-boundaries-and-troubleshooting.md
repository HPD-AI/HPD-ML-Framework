# Contract Boundaries and Troubleshooting

Use this checklist when a pipeline behaves differently from its schema or
metadata.

## A schema succeeds but rows fail later

`GetOutputSchema(...)` does not read values. Consume a small cursor after each
important stage and use the exact CLR type declared by the schema.

## A typed read returns nonsense or throws

Do not probe numeric types with repeated `TryGetValue<T>` calls. Published
`0.5.0` has row paths that can reinterpret boxed value types. Inspect the
schema and request its exact CLR type.

## A supposedly lazy operation uses memory

Training, evaluation, shuffling, splitting, caching, and many learned
transforms materialize rows. An `IDataHandle` return type does not guarantee
streaming execution.

## Batch access fails

Capabilities are optional optimization claims. Call
`TryGetColumnBatch<T>(...)` and retain a cursor fallback. Transformed handles
can copy source capability flags even when output columns do not have a
working batch path.

## Ordering-sensitive results reset

Each new stateful cursor commonly starts fresh in `0.5.0`. Keep one cursor for
one sequence. Do not shuffle ordered input.

## Optional learner input has no effect

Validation data, initial models, environment fields, and backends are
learner-specific. In `0.5.0`, unsupported values can be silently ignored.

## Cancellation arrives too late

Most `FitAsync` implementations wrap synchronous work. Cancellation may only
prevent startup. Consult the workload guide before promising interruption of
active training.

## Model behavior changes after inspection

Several parameter types expose mutable arrays. Do not mutate learned parameter
storage.

## A saved model does not reproduce predictions

The `0.5.0` ZIP serializer does not generally reconstruct saved topology or
inference state. See [Models and persistence](../models/index.md).

## An advertised API always throws

Published `0.5.0` Parquet read and write entry points are placeholders that
throw `NotImplementedException`.

## Next

- [Data troubleshooting by format](../data/index.md)
- [Models troubleshooting](../models/troubleshooting.md)
- [Getting Started](../getting-started/index.md)
