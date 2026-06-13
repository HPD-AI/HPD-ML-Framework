# Data Contracts

Published 0.5.0 uses these default columns:

```text
Features   float[] (scalar float is attempted but unreliable)
Label      float
GroupId    ranking group identifier
```

Custom feature and label names are constructor arguments:

```csharp
ILearner learner = ILearner.LightGbmRegression(
    labelColumn: "Target",
    featureColumn: "Signals");
```

## Dense features

Training materializes every feature row into a dense row-major `float[]`
matrix. There is no sparse training path.

Use a fixed-length, non-empty `float[]` for every row:

```csharp
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [1f, 2f],
        [2f, 4f],
        [3f, 6f]
    }),
    ("Label", new[] { 1f, 2f, 3f }));
```

Published 0.5.0 takes the column count from the first row:

- later longer vectors are truncated while flattening;
- later shorter vectors can fail during copying;
- empty data reaches native dataset creation with zero rows and columns;
- prediction treats a missing feature index as zero.

These are implementation behaviors, not recommended contracts. Keep feature
width identical from training through prediction.

The scalar fallback probes `float[]` first through row implementations that
can throw or reinterpret a scalar value. Use one-element vectors rather than
scalar features.

## Labels

Use `float` labels in published 0.5.0. The loader intends to support `double`
by trying `float` first and then casting a `double` fallback. With
`InMemoryDataHandle`, that first generic probe can reinterpret boxed `double`
storage instead of returning `false`, so the explicit conversion may never
run and labels can be corrupted silently.

Dictionary-backed transformed rows can coerce `double` to `float`, but that
does not make `double` a stable package-wide training contract. Convert labels
to `float` before constructing the training handle.

The learner does not validate objective-specific label domains before calling
LightGBM.

Prepare labels explicitly:

| Objective | Recommended label representation |
| --- | --- |
| Binary | `0f` or `1f` |
| Multiclass | zero-based integer class index stored as `float` |
| Regression, MAE, Huber | finite `float` |
| Poisson | finite non-negative `float` |
| Tweedie | value valid for the chosen variance power |
| Ranking | finite relevance value |

## Ranking groups

Ranking always reads a column named `GroupId`. Equal adjacent values are
collapsed into native group sizes.

Rows for a group must be contiguous. Published 0.5.0 does not validate
contiguity, missing identifiers, or repeated groups that reappear later.
There is no public custom group-column option.

## Weights and categorical features

`DatasetBuilder` can materialize weights internally, but
`LightGbmLearner.Fit` always passes `weightColumn: null`. Training weights are
therefore not available through the public learner.

Categorical tuning options are forwarded in the parameter string, but the
dataset builder does not mark any feature indices as categorical. Encode
categorical values upstream and treat the result as numeric dense features.

## Missing values

Options expose:

```csharp
HandleMissingValue
UseZeroAsMissing
```

These are forwarded to native LightGBM. Managed scoring does not retain the
complete native missing/default-direction split contract, so prediction
fidelity for missing values is not certified.

## Memory behavior

Training stores all feature rows separately and then allocates a second
flattened matrix. Native LightGBM also creates its dataset. Plan for multiple
copies of dense training data in memory.

## Next

- [Objectives and options](objectives-and-options.md)
- [Training runtime](training-runtime.md)
