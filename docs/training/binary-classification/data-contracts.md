# Data Contracts

All four linear binary learners use the same default columns:

```text
Features   float or float[]
Label      value accepted by Convert.ToBoolean
```

Use custom names when constructing the learner:

```csharp
ILearner learner = ILearner.LogisticRegression(
    labelColumn: "IsFraud",
    featureColumn: "Signals");
```

## Features

A vector is the usual representation:

```csharp
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0.2f, 1.5f],
        [-0.4f, 0.1f]
    }),
    ("Label", new[] { true, false }));
```

A scalar `float` is also supported. Other numeric CLR types are not converted
by the training loader. Convert them to `float` during preparation.

Every vector should have the same nonzero length. In `0.5.0`, training
overwrites the recorded feature count for every row, so the final training
row's vector length becomes `FeatureCount`. If that final vector is longer,
earlier short vectors can fail during optimization. If it is shorter, longer
vectors are partly ignored. Prediction likewise assumes every row contains at
least the fitted weight count.

## Labels

`bool` is the clearest label type. Integer `0` and `1` also work because the
loader calls `Convert.ToBoolean`.

This conversion also treats other nonzero numeric values as `true`. String
labels such as `"positive"` are not a supported category mapping. Encode raw
labels explicitly before fitting.

## Current validation gaps

Published `0.5.0` does not reliably reject empty data, single-class data,
mixed vector lengths, nulls, `NaN`, infinity, or invalid option ranges before
training. Validate production input before calling `Fit`.

These checks are planned as framework-enforced contracts for `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/05-custom-input-columns.cs
```

## Next

- [Choose a learner](choosing-a-learner.md)
- [Prepare features](../../getting-started/preparing-data.md)
