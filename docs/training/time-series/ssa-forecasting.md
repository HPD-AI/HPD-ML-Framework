# SSA Forecasting

SSA forecasting learns autoregressive coefficients from an ordered scalar
series and recursively predicts a fixed number of future values.

```csharp
ILearner learner = ILearner.SsaForecasting(
    inputColumn: "Value",
    options: new SsaForecastOptions
    {
        WindowSize = 8,
        Horizon = 4,
        ConfidenceLevel = 0.95f
    });

IModel model = learner.Fit(new LearnerInput(training));
IDataHandle output = model.Transform.Apply(observations);
```

Each row appends fixed-length `float[]` columns:

```text
Forecast
LowerBound
UpperBound
```

The current observation is consumed first. Forecast slot zero predicts the
next observation, slot one predicts two steps ahead, and so on.

## Warm-up and state

Each cursor starts with an empty inference window. The first
`WindowSize - 1` rows contain zero-filled vectors. The row that fills the
window produces the first forecast.

There is no public API to forecast without consuming an observation, override
the horizon at prediction time, checkpoint state, or resume another batch.

## Confidence-bound limitation

Published `0.5.0` attempts to estimate residual variance from the observation
window. Its window holds exactly `WindowSize` values while the calculation
requires at least `WindowSize + 1`, so variance always falls back to `1.0`.

Confidence levels are mapped through a small threshold table rather than a
general normal-quantile calculation. Treat the bounds as current package
output, not calibrated intervals. Training-residual uncertainty and validated
confidence semantics are planned for `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/06-ssa-forecasting.cs
```

