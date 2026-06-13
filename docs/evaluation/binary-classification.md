# Binary Classification Metrics

```csharp
IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    labelColumn: "Label",
    scoreColumn: "Probability",
    threshold: 0.5)
    .Apply(predictions);
```

Output includes AUC, accuracy, F1, precision, recall, log loss, log-loss
reduction, and positive/negative precision and recall.

Published `0.5.0` uses the same score for AUC ranking, threshold decisions, and
log loss. Raw `Score` is often a margin or logit. Clamping it into `(0, 1)`
does not calibrate it, so select a verified `Probability` column for the
combined evaluator.

Empty input returns zeros. Single-class AUC returns `0.5`. Undefined precision
or recall denominators produce zero. Out-of-range log-loss values are clamped
rather than rejected.

```bash
dotnet run cookbook/Evaluation/01-binary-metrics.cs
```
