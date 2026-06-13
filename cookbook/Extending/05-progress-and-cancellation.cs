#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Emit useful progress and terminate cancellation deliberately.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Value", Enumerable.Range(1, 5).Select(value => (float)value).ToArray()));

var learner = new ProgressLearner();
using var subscription = learner.Progress.Subscribe(new Printer());
learner.Fit(new LearnerInput(data));

using var cts = new CancellationTokenSource();
cts.Cancel();

var canceledLearner = new ProgressLearner();
try
{
    canceledLearner.Fit(
        new LearnerInput(
            data,
            Environment: new DefaultExecutionEnvironment(
                cancellationToken: cts.Token)));
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancellation observed.");
}

sealed class ProgressLearner : ILearner
{
    private readonly ProgressSubject _progress = new();

    public IObservable<ProgressEvent> Progress => _progress;
    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;

    public IModel Fit(LearnerInput input)
    {
        try
        {
            for (var epoch = 1; epoch <= 3; epoch++)
            {
                input.Environment?.CancellationToken
                    .ThrowIfCancellationRequested();

                _progress.OnNext(new ProgressEvent
                {
                    Epoch = epoch,
                    MetricName = "Work",
                    MetricValue = epoch / 3.0
                });
            }

            var model = new Model(new IdentityTransform(), EmptyParameters.Value);
            _progress.OnCompleted();
            return model;
        }
        catch (Exception error)
        {
            _progress.OnError(error);
            throw;
        }
    }

    public Task<IModel> FitAsync(
        LearnerInput input,
        CancellationToken ct = default) =>
        Task.Run(() =>
        {
            ct.ThrowIfCancellationRequested();
            return Fit(input);
        }, ct);
}

sealed class Printer : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"epoch={value.Epoch} {value.MetricName}={value.MetricValue:F2}");

    public void OnCompleted() => Console.WriteLine("complete");
    public void OnError(Exception error) =>
        Console.WriteLine($"error: {error.GetType().Name}");
}

sealed class IdentityTransform : ITransform
{
    public TransformProperties Properties => new() { PreservesRowCount = true };
    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;
    public IDataHandle Apply(IDataHandle input) => input;
}

sealed class EmptyParameters : ILearnedParameters
{
    public static EmptyParameters Value { get; } = new();
}
