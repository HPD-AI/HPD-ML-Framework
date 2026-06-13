# Clustering

Clustering groups unlabeled rows by similarity. Published `0.5.0` provides
batch K-means and mini-batch K-means over dense `float[]` feature vectors.

Install:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-Clustering
dotnet add package HPD-ML-Evaluation
```

Import `HPD.ML.Clustering` to discover the C# 14 learner extension members on
`ILearner`.

## Learning path

1. [Data contracts](data-contracts.md)
2. [Choosing a learner](choosing-a-learner.md)
3. [Batch K-means](k-means.md)
4. [Mini-batch K-means](mini-batch-k-means.md)
5. [Initialization](initialization.md)
6. [Prediction and model inspection](prediction-and-model-inspection.md)
7. [Evaluation](evaluation.md)
8. [Training progress](training-progress.md)
9. [Troubleshooting](troubleshooting.md)

## Learners

| Learner | Main configuration | Current special behavior |
| --- | --- | --- |
| K-means | clusters, initialization, iterations, tolerance, seed | Lloyd's algorithm over the materialized dataset |
| Mini-batch K-means | clusters, batch size, initialization, iterations, tolerance, seed | Samples batches after materializing the complete dataset |

Both learners append `PredictedLabel` as a one-based `uint` and `Score` as a
`float[]` of squared Euclidean distances to every centroid.

The guides and recipes describe verified published `0.5.0` behavior. There is
not yet a versioned Clustering `0.6.0` proposal. Items identified for a future
release are recommendations, not committed package behavior.
