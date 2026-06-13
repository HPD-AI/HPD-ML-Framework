# SSA Anomaly Detection

SSA anomaly detection learns a univariate recurrence from ordered training
data, predicts each later observation, and scores the prediction error.

```csharp
ILearner learner = ILearner.SsaSpikeDetection(
    inputColumn: "Value",
    options: new SsaAnomalyOptions
    {
        WindowSize = 8,
        RankSelection = RankSelectionMethod.Exact,
        ErrorFunction = ErrorFunction.AbsoluteDifference,
        Alerting = AlertingMode.RawScore,
        Threshold = 2
    });

IModel model = learner.Fit(new LearnerInput(training));
IDataHandle output = model.Transform.Apply(observations);
```

Output columns are `Alert`, `RawScore`, `PValue`, and `MartingaleScore`.

## Training contract

SSA training:

- reads scalar `float` values from the selected input column;
- materializes the full series;
- uses the first `SeriesLength` rows when that option is positive;
- requires the effective series length to be at least `WindowSize + 1`;
- selects rank automatically when `Rank = 0`.

Published `0.5.0` does not reliably validate window, rank, finite values, or
all option combinations before decomposition. Keep `1 <= Rank < WindowSize`
when selecting rank manually.

## Error functions

Available functions are signed difference, absolute difference, signed
proportion, absolute proportion, and squared difference. Proportion modes
return zero when the prediction is zero.

## Inference warm-up

The fitted model does not seed the observation window from the training tail.
Each cursor starts empty. The first `WindowSize` observations emit placeholder
scores and no alert; meaningful scoring begins on the following row.

Validation data and initial models are ignored in `0.5.0`. Adaptive SSA is not
implemented.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/03-ssa-spike-detection.cs
```

