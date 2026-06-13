#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Calculate NDCG and DCG across two query groups.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle ranked = InMemoryDataHandle.FromColumns(
    ("GroupId", new[] { "q1", "q1", "q1", "q2", "q2", "q2" }),
    ("Label", new[] { 3.0, 2.0, 0.0, 0.0, 2.0, 1.0 }),
    ("Score", new[] { 0.9, 0.7, 0.1, 0.8, 0.6, 0.3 }));

IDataHandle metrics = ITransform.RankingMetrics(
    truncationLevels: [1, 3]).Apply(ranked);

using var row = metrics.GetCursor(["NDCG@1", "DCG@1", "NDCG@3", "DCG@3"]);
row.MoveNext();
foreach (string name in new[] { "NDCG@1", "DCG@1", "NDCG@3", "DCG@3" })
    Console.WriteLine($"{name,-7}: {row.Current.GetValue<double>(name):F4}");

Console.WriteLine("\nMetrics are unweighted means across groups.");
