#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Inspect centroid coordinates and use the parameter-level nearest-cluster API.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0f, 0f], [0.5f, 0.2f], [-0.3f, 0.1f],
        [10f, 10f], [10.4f, 9.8f], [9.7f, 10.2f]
    }));

IModel model = ILearner.KMeans(
    options: new KMeansOptions
    {
        NumberOfClusters = 2,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    }).Fit(new LearnerInput(data));

var parameters = (ClusteringModelParameters)model.Parameters;
for (int cluster = 0; cluster < parameters.K; cluster++)
{
    Console.WriteLine(
        $"Label {cluster + 1}: " +
        string.Join(", ", parameters.GetCentroid(cluster).ToArray().Select(x => x.ToString("F3"))));
}

float[] point = [9.9f, 10.1f];
var nearest = parameters.NearestCluster(point);
Console.WriteLine(
    $"Point [9.9, 10.1] -> label {nearest.Cluster + 1}, " +
    $"squared-distance {nearest.Distance:F4}");
