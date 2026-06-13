# Seasonality and Decomposition

The Time Series package exposes two public utilities for examining complete
`double` series.

## Detect a period

```csharp
int period = SeasonalityDetector.DetectPeriod(values);
```

The detector computes autocorrelation through FFT and returns the strongest
peak above `randomnessThreshold`, limited to `min(maxLag, length / 4)`.
It returns `-1` when:

- fewer than 16 values are supplied;
- variance is effectively zero;
- no qualifying peak is found.

## Decompose a series

```csharp
var (seasonal, trend, residual) =
    StlDecomposition.Decompose(values, period: 12);
```

The arrays have the same length as the input and approximately satisfy:

```text
value = seasonal + trend + residual
```

Despite the public STL name, published `0.5.0` uses moving-average smoothing,
not LOESS. Treat it as a lightweight seasonal/trend decomposition rather than
a certified Cleveland STL implementation.

The input length must be at least twice the period. Period, iteration count,
threshold, lag, and finite-value validation are incomplete in `0.5.0`.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/08-seasonality-and-decomposition.cs
```

