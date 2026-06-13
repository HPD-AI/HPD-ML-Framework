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

## Published 0.5.0 behavior

The input is materialized, randomly shuffled, and exposed through indexed
views. A seed makes fold membership deterministic.

`folds` is not validated. Zero divides by zero, one creates an empty training
fold, and more folds than rows creates empty test folds.

The optional `featurePipeline` is an already configured transform applied to
each train and test fold. It does not fit preparation learners independently
inside each fold. Fit learned normalization, category maps, and text
vocabularies within each fold to avoid leakage.

`BestModel(metricName)` selects the highest metric value. Use it only for
higher-is-better metrics such as accuracy or AUC. It is wrong for log loss,
MAE, RMSE, average distance, and DBI. An unknown metric can fall back to the
first fold model.

`StdDev` is the population standard deviation. Fold models are trained on
partial data; refit the chosen configuration on all training rows.

Published `0.5.0` has no stratified, grouped, or ordered cross-validation.

```bash
dotnet run cookbook/Evaluation/07-cross-validation.cs
```
