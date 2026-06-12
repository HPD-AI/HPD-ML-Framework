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
