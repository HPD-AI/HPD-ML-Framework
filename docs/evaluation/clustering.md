# Clustering Metrics

`ClusteringMetrics` returns NMI, average distance, and Davies-Bouldin index.
NMI is higher-is-better; distance and DBI are lower-is-better.

Published `0.5.0` always requires true `Label` and `PredictedLabel`.
`Score` and `Features` are optional during schema inspection, but missing
inputs silently produce zero for average distance or DBI.

K-means scores are squared centroid distances, so the reported average is an
average minimum squared distance. Single-cluster DBI and perfect
single-class/single-cluster NMI also return zero. Empty score arrays can fail,
and feature dimensions are not validated consistently.

```bash
dotnet run cookbook/Evaluation/06-clustering-metrics.cs
```
