#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Fit mini-batch K-means to a generated in-memory dataset.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

var rng = new Random(7);
var features = new List<float[]>();
for (int i = 0; i < 120; i++)
{
    float center = i < 60 ? 0f : 10f;
    features.Add([
        center + (float)(rng.NextDouble() - 0.5),
        center + (float)(rng.NextDouble() - 0.5)
    ]);
}

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", features.ToArray()));

ILearner learner = ILearner.MiniBatchKMeans(
    options: new MiniBatchKMeansOptions
    {
        NumberOfClusters = 2,
        BatchSize = 20,
        MaxIterations = 100,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    });

IModel model = learner.Fit(new LearnerInput(training));
var parameters = (ClusteringModelParameters)model.Parameters;

for (int cluster = 0; cluster < parameters.K; cluster++)
{
    Console.WriteLine(
        $"Cluster {cluster + 1}: " +
        string.Join(", ", parameters.GetCentroid(cluster).ToArray().Select(x => x.ToString("F3"))));
}

Console.WriteLine(
    "Note: published 0.5.0 samples mini-batches after materializing every row.");
