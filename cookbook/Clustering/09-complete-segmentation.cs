#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Train, inspect, score, and evaluate a complete customer-segmentation model.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle customers = InMemoryDataHandle.FromColumns(
    ("Customer", new[] { "Ava", "Bo", "Cy", "Dee", "Eve", "Fox", "Gia", "Hal", "Ira" }),
    ("KnownSegment", new[] { 0, 0, 0, 1, 1, 1, 2, 2, 2 }),
    ("Signals", new float[][]
    {
        [0.10f, 0.15f], [0.15f, 0.10f], [0.12f, 0.18f],
        [0.50f, 0.55f], [0.55f, 0.50f], [0.52f, 0.58f],
        [0.88f, 0.85f], [0.92f, 0.88f], [0.85f, 0.92f]
    }));

ILearner learner = ILearner.KMeans(
    featureColumn: "Signals",
    options: new KMeansOptions
    {
        NumberOfClusters = 3,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    });

IModel model = learner.Fit(new LearnerInput(customers));
var parameters = (ClusteringModelParameters)model.Parameters;

Console.WriteLine("Centroids");
for (int cluster = 0; cluster < parameters.K; cluster++)
{
    Console.WriteLine(
        $"  label {cluster + 1}: " +
        string.Join(", ", parameters.GetCentroid(cluster).ToArray().Select(x => x.ToString("F3"))));
}

IDataHandle predictions = model.Transform.Apply(customers);
Console.WriteLine("\nAssignments");
using (var rows = predictions.GetCursor(["Customer", "PredictedLabel", "Score"]))
{
    while (rows.MoveNext())
    {
        uint label = rows.Current.GetValue<uint>("PredictedLabel");
        float[] score = rows.Current.GetValue<float[]>("Score");
        Console.WriteLine(
            $"  {rows.Current.GetValue<string>("Customer")}: " +
            $"label {label}, squared-distance {score[(int)label - 1]:F4}");
    }
}

IDataHandle metrics = ITransform.ClusteringMetrics(
    labelColumn: "KnownSegment",
    featuresColumn: "Signals")
    .Apply(predictions);

using var metric = metrics.GetCursor([
    "NormalizedMutualInformation",
    "AverageDistance",
    "DaviesBouldinIndex"
]);
metric.MoveNext();
Console.WriteLine(
    $"\nNMI: {metric.Current.GetValue<double>("NormalizedMutualInformation"):F4}");
Console.WriteLine(
    $"Average squared distance: {metric.Current.GetValue<double>("AverageDistance"):F4}");
Console.WriteLine(
    $"Davies-Bouldin index: {metric.Current.GetValue<double>("DaviesBouldinIndex"):F4}");
