#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Print zero-based progress events from both clustering learners.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0f, 0f], [0.3f, 0.1f], [-0.2f, -0.1f],
        [10f, 10f], [10.3f, 10.1f], [9.8f, 9.9f],
        [20f, 0f], [20.3f, 0.1f], [19.8f, -0.1f]
    }));

(string Name, ILearner Learner)[] runs =
[
    ("K-means", ILearner.KMeans(
        options: new KMeansOptions
        {
            NumberOfClusters = 3,
            MaxIterations = 10,
            Seed = 42
        })),
    ("Mini-batch", ILearner.MiniBatchKMeans(
        options: new MiniBatchKMeansOptions
        {
            NumberOfClusters = 3,
            BatchSize = 5,
            MaxIterations = 10,
            Seed = 42
        }))
];

foreach (var (name, learner) in runs)
{
    Console.WriteLine($"\n{name}");
    using var subscription = learner.Progress.Subscribe(new ProgressPrinter());
    learner.Fit(new LearnerInput(data));
}

sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"epoch={value.Epoch} {value.MetricName}={value.MetricValue:F4}");

    public void OnCompleted() => Console.WriteLine("complete");
    public void OnError(Exception error) => Console.Error.WriteLine(error.Message);
}
