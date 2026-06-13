# Multiclass Classification Metrics

```csharp
IDataHandle metrics = ITransform.MulticlassMetrics().Apply(predictions);
```

Output contains `MicroAccuracy`, `MacroAccuracy`, `LogLoss`, and
`LogLossReduction`. `Score` must be a `float[]` for log loss.

Published `0.5.0` maps score positions to the sorted union of observed labels
and predictions. Use contiguous zero-based labels with vectors in that order.
There is no model metadata declaring class order.

Missing or incompatible score vectors produce zero log loss, and malformed
rows can be skipped from log-loss averaging. Probabilities are clamped but not
checked for normalization. Empty input returns zeros.

```bash
dotnet run cookbook/Evaluation/04-multiclass-metrics.cs
```
