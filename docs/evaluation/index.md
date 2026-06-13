# Evaluation

Install:

```bash
dotnet add package HPD-ML-Evaluation
```

Evaluation transforms consume scored `IDataHandle` rows and return metric data
handles. Published `0.5.0` includes binary, multiclass, regression, ranking,
clustering, confusion-matrix, cross-validation, and permutation-importance
APIs.

- [Choosing an evaluator](choosing-an-evaluator.md)
- [Data and output contracts](data-and-output-contracts.md)
- [Binary classification](binary-classification.md)
- [Multiclass classification](multiclass-classification.md)
- [Regression](regression.md)
- [Ranking](ranking.md)
- [Clustering](clustering.md)
- [Confusion matrices](confusion-matrices.md)
- [Cross-validation](cross-validation.md)
- [Permutation feature importance](feature-importance.md)
- [Troubleshooting](troubleshooting.md)

These guides distinguish verified `0.5.0` behavior from proposed `0.6.0`
corrections. Do not interpret an undocumented zero as proof of perfect quality.

Run the cookbook:

```bash
dotnet run cookbook/Evaluation/01-binary-metrics.cs
```
