# Evaluation

Install `HPD-ML-Evaluation` and apply the clustering metric transform:

```csharp
IDataHandle metrics = ITransform.ClusteringMetrics().Apply(predictions);
```

Published `0.5.0` requires both `Label` and `PredictedLabel`, even though
clustering training itself is unsupervised.

| Metric | Required columns | Interpretation |
| --- | --- | --- |
| `NormalizedMutualInformation` | `Label`, `PredictedLabel` | Agreement between known classes and clusters; higher is better |
| `AverageDistance` | `Score` | Mean minimum **squared** centroid distance; lower is better |
| `DaviesBouldinIndex` | `PredictedLabel`, `Features` | Within-cluster spread relative to centroid separation; lower is better |

`Score` and `Features` are optional in the implementation. Their missing
metrics silently return zero, which is indistinguishable from a perfect value.
The true label is not optional: omitting it fails while rows are read.

Additional published edge behavior:

- NMI returns zero for a single-class/single-cluster perfect partition because
  both entropies are zero.
- DBI returns zero when features are unavailable, feature extraction fails,
  fewer than two predicted clusters exist, or feature counts do not align.
- Coincident cluster centroids skip their zero-separation ratio instead of
  reporting an infinite or undefined result.
- The evaluator materializes labels, predictions, scores, and features.

Use known labels when validating cluster recovery. For genuinely unlabeled
data, inspect squared distance and cluster stability separately rather than
fabricating labels.

Optional labels, explicit unavailable metrics, squared-distance naming, and
defined degenerate cases are recommended `0.6.0` corrections.

## Run the recipe

```bash
dotnet run cookbook/Clustering/07-clustering-metrics.cs
```

## Next

- [Prediction and model inspection](prediction-and-model-inspection.md)
- [Troubleshooting](troubleshooting.md)
