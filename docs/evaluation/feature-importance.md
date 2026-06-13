# Permutation Feature Importance

Permutation importance measures the drop in a selected metric after shuffling
one feature:

```csharp
IDataHandle importance = PermutationFeatureImportance.Compute(
    model,
    testData,
    ITransform.BinaryClassificationMetrics(
        scoreColumn: "Probability"),
    metricName: "Accuracy",
    featureColumns: ["Features"],
    permutations: 10,
    seed: 42);
```

The result contains `FeatureName`, `MetricDrop`, and `MetricDropStdDev`.
The data is materialized once because each feature is read repeatedly.

A vector-valued column such as `Features` is shuffled as one column; its
individual dimensions are not ranked separately.

Published `0.5.0` always calculates `baseline - permuted`. Use a
higher-is-better metric such as accuracy, AUC, or R-squared. Lower-is-better
metrics can produce negative importance for useful features.

Unknown metric names are read as zero rather than rejected. The current
wrapper also does not reliably substitute shuffled scalar value types through
every `GetValue<T>` path. Reference/vector columns, including `float[]`, are
the reliable demonstrated path.

`permutations` and feature names are not validated up front, and the API has
no cancellation token.

```bash
dotnet run cookbook/Evaluation/08-permutation-feature-importance.cs
```
