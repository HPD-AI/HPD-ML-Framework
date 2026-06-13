# Data Contracts

All four regression learners use these default columns:

```text
Features   float[] (scalar float is intended but unreliable in 0.5.0)
Label      value accepted by Convert.ToDouble
```

Custom names are supplied when constructing a learner:

```csharp
ILearner learner = ILearner.OrdinaryLeastSquares(
    labelColumn: "Price",
    featureColumn: "Signals");
```

## Features

A vector is the usual representation:

```csharp
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [1f, 2f], [2f, 3f] }),
    ("Label", new[] { 5f, 8f }));
```

The loader intends to support a scalar `float`, but published `0.5.0` probes
`float[]` first through row implementations that can throw or reinterpret a
scalar instead of returning `false`. Use `float[]` features, including
one-element vectors, for reliable package behavior. Prepare `double`, integer,
text, or categorical inputs as `float[]` before fitting.

Every vector should have the same nonzero length. Published `0.5.0` overwrites
the recorded feature count for every row, so the final row determines
`FeatureCount`. A longer final vector can make earlier short rows fail; a
shorter final vector causes longer earlier vectors to be partly ignored.
Prediction requires at least the fitted number of values.

## Labels

Labels are read with `Convert.ToDouble`, so common numeric CLR types work.
Use finite numeric labels with an explicit meaning. Strings, booleans, nulls,
`NaN`, and infinity are not a stable regression contract even when a
particular conversion happens to succeed.

Poisson regression adds one check: labels less than zero throw. Published
`0.5.0` still accepts fractional non-negative labels and does not reliably
reject non-finite values.

## Current validation gaps

Published `0.5.0` does not consistently reject empty data, zero-length or
mixed-length vectors, scalar/vector mixing, nulls, non-finite values, or
unsupported option ranges before optimization. These checks are planned for
`0.6.0`.

All four learners copy the complete feature and label columns into memory.

## Run the recipe

```bash
dotnet run cookbook/Regression/05-custom-input-columns.cs
```

## Next

- [Choose a learner](choosing-a-learner.md)
- [Prepare features](../../getting-started/preparing-data.md)
