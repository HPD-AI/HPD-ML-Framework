# Choosing an Evaluator

| Task | Transform | Required input | Direction |
| --- | --- | --- | --- |
| Binary | `BinaryClassificationMetrics` | `Label`, scalar score | AUC/accuracy higher; log loss lower |
| Multiclass | `MulticlassMetrics` | `Label`, `PredictedLabel`, `Score` | accuracy higher; log loss lower |
| Regression | `RegressionMetrics` | `Label`, `Score` | errors lower; R-squared higher |
| Ranking | `RankingMetrics` | `Label`, `Score`, `GroupId` | NDCG/DCG higher |
| Clustering | `ClusteringMetrics` | labels; optional score/features | NMI higher; distance/DBI lower |
| Counts | `ConfusionMatrix` | `Label`, `PredictedLabel` | inspect counts |

Published `0.5.0` does not expose metric direction as metadata. This matters
because `BestModel` always chooses the largest value and permutation
importance always calculates `baseline - permuted`.

Use `Probability` for current binary log loss and threshold metrics. Raw
`Score` is suitable for ranking but is not necessarily a probability.
