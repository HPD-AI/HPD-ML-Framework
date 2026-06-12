#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// This sample configures a learner, observes progress, and inspects its model.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle trainingData = CreateData(offset: 0.0f);
IDataHandle validationData = CreateData(offset: 0.2f);

ILearner learner = ILearner.LinearSvm(
    options: new LinearSvmOptions
    {
        NumberOfIterations = 8,
        Lambda = 0.01,
        Seed = 42
    });

using var progress = learner.Progress.Subscribe(new ProgressPrinter());

IModel model = await learner.FitAsync(
    new LearnerInput(
        TrainData: trainingData,
        ValidationData: validationData));

var parameters = (LinearModelParameters)model.Parameters;

Console.WriteLine($"Features: {parameters.FeatureCount}");
Console.WriteLine($"Weights: {string.Join(", ", parameters.Weights.Select(w => w.ToString("F3")))}");
Console.WriteLine($"Bias: {parameters.Bias:F3}");

static IDataHandle CreateData(float offset) =>
    InMemoryDataHandle.FromColumns(
        ("Features", new float[][]
        {
            [-2.0f + offset, -1.0f],
            [-1.5f + offset, -0.5f],
            [-1.0f + offset, -1.5f],
            [1.0f + offset, 1.0f],
            [1.5f + offset, 0.5f],
            [2.0f + offset, 1.5f]
        }),
        ("Label", new[] { false, false, false, true, true, true }));

sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"Epoch {value.Epoch}: {value.MetricName}={value.MetricValue:F4}");

    public void OnCompleted() => Console.WriteLine("Training complete.");

    public void OnError(Exception error) =>
        Console.Error.WriteLine($"Training failed: {error.Message}");
}
