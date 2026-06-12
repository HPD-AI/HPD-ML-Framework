# Cross-Validation

Cross-validation repeatedly splits, trains, predicts, and aggregates metrics:

```csharp
var result = await CrossValidator.EvaluateAsync(
    data,
    learner,
    ITransform.BinaryClassificationMetrics(
        scoreColumn: "Probability"),
    folds: 5,
    seed: 42,
    ct: cancellationToken);
```

`result.Folds` contains each fold's model and metrics.
`result.AggregateMetrics` contains `Metric`, `Mean`, and `StdDev` rows.

The optional `featurePipeline` is an already configured transform applied to
each train and test fold. It does not fit preparation learners independently
inside each fold. Fit learned normalization, category maps, and text
vocabularies within each fold to avoid leakage.

`BestModel(metricName)` selects the highest metric value. Use it only for
higher-is-better metrics such as accuracy or AUC, not loss or error metrics.
