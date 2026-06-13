#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Compare initialization strategies and verify seeded repeatability.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0f, 0f], [0.2f, 0.1f], [-0.2f, -0.1f],
        [10f, 10f], [10.2f, 9.9f], [9.8f, 10.1f],
        [20f, 0f], [20.2f, 0.1f], [19.8f, -0.1f]
    }));

foreach (KMeansInitialization initialization in Enum.GetValues<KMeansInitialization>())
{
    ClusteringModelParameters first = Fit(initialization);
    ClusteringModelParameters second = Fit(initialization);
    bool repeatable = first.Centroids.SequenceEqual(second.Centroids);

    Console.WriteLine($"{initialization}: repeatable={repeatable}");
    for (int cluster = 0; cluster < first.K; cluster++)
    {
        Console.WriteLine(
            $"  {cluster + 1}: " +
            string.Join(", ", first.GetCentroid(cluster).ToArray().Select(x => x.ToString("F2"))));
    }
}

ClusteringModelParameters Fit(KMeansInitialization initialization)
{
    ILearner learner = ILearner.KMeans(
        options: new KMeansOptions
        {
            NumberOfClusters = 3,
            Initialization = initialization,
            Seed = 42
        });
    return (ClusteringModelParameters)learner.Fit(new LearnerInput(data)).Parameters;
}
