#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Observe learner progress and the pre-start cancellation boundary.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-2f], [-1f], [1f], [2f] }),
    ("Label", new[] { false, false, true, true }));

ILearner learner = ILearner.LinearSvm(
    options: new LinearSvmOptions
    {
        NumberOfIterations = 3,
        Seed = 42
    });

using (learner.Progress.Subscribe(new Printer()))
    await learner.FitAsync(new LearnerInput(data));

using var cts = new CancellationTokenSource();
cts.Cancel();
try
{
    await learner.FitAsync(new LearnerInput(data), cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("A token canceled before scheduling prevented the fit.");
}

Console.WriteLine(
    "Published 0.5.0 does not generally stop synchronous training already in progress.");

sealed class Printer : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"epoch={value.Epoch} {value.MetricName}={value.MetricValue:F4}");
    public void OnCompleted() => Console.WriteLine("progress complete");
    public void OnError(Exception error) =>
        Console.WriteLine($"progress error={error.GetType().Name}");
}

// Guide: docs/operations/cancellation-progress-and-diagnostics.md

