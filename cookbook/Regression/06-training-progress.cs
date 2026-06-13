#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Print the metric names and zero-based epochs reported by each learner.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Regression;

IDataHandle linear = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-2f], [-1f], [0f], [1f], [2f], [3f] }),
    ("Label", new[] { -3f, -1f, 1f, 3f, 5f, 7f }));

IDataHandle counts = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [0.4f], [0.8f], [1.2f], [1.6f], [2f] }),
    ("Label", new[] { 1f, 1f, 2f, 2f, 3f, 5f }));

(string Name, ILearner Learner, IDataHandle Data)[] runs =
[
    ("OLS", ILearner.OrdinaryLeastSquares(
        options: new OlsOptions { MaxIterations = 3 }), linear),
    ("SDCA", ILearner.SdcaRegression(
        options: new SdcaRegressionOptions
        {
            NumberOfIterations = 3,
            ConvergenceTolerance = 0,
            L2Regularization = 0.1,
            Seed = 42
        }), linear),
    ("Online GD", ILearner.OnlineGradientDescent(
        options: new OnlineGradientDescentOptions
        {
            NumberOfIterations = 3,
            LearningRate = 0.01
        }), linear),
    ("Poisson", ILearner.PoissonRegression(
        options: new PoissonRegressionOptions { MaxIterations = 3 }), counts)
];

foreach (var (name, learner, data) in runs)
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

