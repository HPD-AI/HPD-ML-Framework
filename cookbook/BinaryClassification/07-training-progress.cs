#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Observe the metric names and zero-based epochs emitted by each learner.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f], [-1.5f], [-1f], [-0.5f],
        [0.5f], [1f], [1.5f], [2f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

await Run("Logistic", ILearner.LogisticRegression(
    options: new LogisticRegressionOptions { MaxIterations = 3 }), data);
await Run("SDCA", ILearner.Sdca(
    options: new SdcaOptions
    {
        NumberOfIterations = 3,
        L2Regularization = 1.0,
        ConvergenceTolerance = 0,
        Seed = 42
    }), data);
await Run("Perceptron", ILearner.AveragedPerceptron(
    options: new AveragedPerceptronOptions { NumberOfIterations = 3 }), data);
await Run("SVM", ILearner.LinearSvm(
    options: new LinearSvmOptions { NumberOfIterations = 3, Seed = 42 }), data);

static async Task Run(string name, ILearner learner, IDataHandle data)
{
    Console.WriteLine($"\n{name}");
    using var subscription = learner.Progress.Subscribe(new ProgressPrinter());
    await learner.FitAsync(new LearnerInput(data));
}

sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"epoch={value.Epoch} {value.MetricName}={value.MetricValue:F4}");

    public void OnCompleted() => Console.WriteLine("complete");
    public void OnError(Exception error) => Console.Error.WriteLine(error.Message);
}
