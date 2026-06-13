#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Evaluate known cluster recovery, squared distances, and cluster separation.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle predictions = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 0, 0, 0, 1, 1, 1 }),
    ("PredictedLabel", new[] { 0, 0, 0, 1, 1, 1 }),
    ("Score", new float[][]
    {
        [0.00f, 200f], [0.05f, 196f], [0.05f, 204f],
        [200f, 0.00f], [204f, 0.05f], [196f, 0.05f]
    }),
    ("Features", new float[][]
    {
        [0f, 0f], [0.2f, 0.1f], [-0.2f, -0.1f],
        [10f, 10f], [10.2f, 10.1f], [9.8f, 9.9f]
    }));

IDataHandle metrics = ITransform.ClusteringMetrics().Apply(predictions);
using var row = metrics.GetCursor(
    ["NormalizedMutualInformation", "AverageDistance", "DaviesBouldinIndex"]);
row.MoveNext();

Console.WriteLine(
    $"NMI:                    {row.Current.GetValue<double>("NormalizedMutualInformation"):F4}");
Console.WriteLine(
    $"Average squared dist.:  {row.Current.GetValue<double>("AverageDistance"):F4}");
Console.WriteLine(
    $"Davies-Bouldin index:   {row.Current.GetValue<double>("DaviesBouldinIndex"):F4}");
