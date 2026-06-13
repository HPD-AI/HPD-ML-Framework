# Ranking Metrics

```csharp
IDataHandle metrics = ITransform.RankingMetrics(
    truncationLevels: [1, 3, 5]).Apply(predictions);
```

For each `K`, output contains `NDCG@K` and `DCG@K`, averaged equally across
groups. The gain is `2^relevance - 1`; the discount is
`log2(position + 2)`.

Defaults are `1`, `3`, `5`, and `10`. Published `0.5.0` does not validate
zero, negative, or duplicate levels. Empty input returns zeros. A group with
zero ideal DCG reports NDCG zero, and tie behavior is not a public contract.

```bash
dotnet run cookbook/Evaluation/05-ranking-metrics.cs
```
