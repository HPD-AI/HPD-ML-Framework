# Normalization

Normalization learners consume scalar `float` columns and return fitted
transforms. Convert inferred `double` or integer columns to `float` first.

## Min-max

```csharp
ILearner learner = ILearner.MinMaxNormalize(
    "Value",
    scaleMin: -1f,
    scaleMax: 1f,
    outputColumn: "ValueScaled");
```

The learner stores the training minimum and maximum. Values outside that range
are not clipped. A zero training range uses a denominator of one.

## Mean-variance

```csharp
ILearner learner = ILearner.MeanVarianceNormalize(
    "Value",
    outputColumn: "ValueZScore");
```

This computes population mean and standard deviation, then applies
`(value - mean) / stddev`. A zero standard deviation is treated as one by the
transform.

## Quantile bins

```csharp
ILearner learner = ILearner.BinNormalize(
    "Value",
    numBins: 4,
    outputColumn: "ValueBin");
```

The learner stores quantile edges. The transform outputs the bin index scaled
to `[0, 1]`, not the raw bin number.

## Output schema

With no distinct output name, normalization replaces the source values while
retaining the existing schema field. With `outputColumn`, it appends a scalar
`float` field and preserves the source.

In `0.5.0`, mean-variance and bin normalization do not validate a missing
column during schema inspection as consistently as min-max does. Invalid
`numBins` and empty-data behavior are also not guarded reliably. Use a
non-empty training set and `numBins >= 1`.

## Run the recipe

```bash
dotnet run cookbook/Transforms/04-normalization.cs
```

## Next

- [Conversion and hashing](conversion-and-hashing.md)
- [Fit once and reuse](fixed-and-learned-transforms.md)

