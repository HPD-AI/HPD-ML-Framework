# Regression Metrics

```csharp
IDataHandle metrics = ITransform.RegressionMetrics(
    featureCount: 3).Apply(predictions);
```

Output contains `MAE`, `MSE`, `RMSE`, `RSquared`, and `AdjustedRSquared`.
Errors are lower-is-better; R-squared is higher-is-better and may be negative.

Published `0.5.0` returns zeros for empty input and reports
`RSquared = 0` for constant labels, including perfect predictions. Without a
valid `featureCount`, adjusted R-squared silently equals ordinary R-squared.
Use `double` columns for direct metric handles.

```bash
dotnet run cookbook/Evaluation/03-regression-metrics.cs
```
