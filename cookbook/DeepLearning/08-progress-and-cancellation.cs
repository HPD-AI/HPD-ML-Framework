#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Observe the completion-only progress contract in published 0.5.0.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f], [3f] }),
    ("Label", new[] { 1f, 3f, 5f, 7f }));

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(1, 1)]);
var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 5, LearningRate = 0.01f });

var observer = new ProgressPrinter();
using var subscription = learner.Progress.Subscribe(observer);
await learner.FitAsync(new LearnerInput(data));

Console.WriteLine($"Metric events: {observer.Events}");
Console.WriteLine("Active-loop cancellation is not supported in 0.5.0.");

sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public int Events { get; private set; }

    public void OnNext(ProgressEvent value)
    {
        Events++;
        Console.WriteLine($"{value.MetricName}={value.MetricValue}");
    }

    public void OnCompleted() => Console.WriteLine("Progress completed.");
    public void OnError(Exception error) => Console.Error.WriteLine(error.Message);
}

