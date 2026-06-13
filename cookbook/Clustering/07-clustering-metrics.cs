#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Evaluate recovered clusters when known reference labels are available.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 0, 0, 0, 1, 1, 1 }),
    ("Features", new float[][]
    {
        [0f, 0f], [0.2f, 0.1f], [-0.2f, -0.1f],
        [10f, 10f], [10.2f, 10.1f], [9.8f, 9.9f]
    }));

IModel model = ILearner.KMeans(
    options: new KMeansOptions
    {
        NumberOfClusters = 2,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    }).Fit(new LearnerInput(data));

IDataHandle predictions = model.Transform.Apply(data);
IDataHandle metrics = ITransform.ClusteringMetrics().Apply(predictions);

using var row = metrics.GetCursor([
    "NormalizedMutualInformation",
    "AverageDistance",
    "DaviesBouldinIndex"
]);
row.MoveNext();

Console.WriteLine(
    $"NMI: {row.Current.GetValue<double>("NormalizedMutualInformation"):F4}");
Console.WriteLine(
    $"Average squared distance: {row.Current.GetValue<double>("AverageDistance"):F4}");
Console.WriteLine(
    $"Davies-Bouldin index: {row.Current.GetValue<double>("DaviesBouldinIndex"):F4}");
