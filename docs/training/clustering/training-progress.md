# Training Progress

Subscribe before fitting:

```csharp
using var subscription =
    learner.Progress.Subscribe(new ProgressPrinter());

IModel model = await learner.FitAsync(new LearnerInput(training));
```

Both learners publish `AverageDistance` once per iteration. The value is the
average **squared** distance observed during that iteration:

- batch K-means reports the complete training set;
- mini-batch K-means reports only the sampled batch.

Epoch values are zero-based. The mini-batch series can rise between iterations
because each event measures a different random sample.

```csharp
sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"{value.Epoch}: {value.MetricName}={value.MetricValue:F4}");

    public void OnCompleted() => Console.WriteLine("Complete");
    public void OnError(Exception error) => Console.Error.WriteLine(error);
}
```

`FitAsync` uses `Task.Run`. Its token can cancel scheduling before work starts,
but active initialization and training loops do not observe cancellation.
`LearnerInput.Environment` and validation data are ignored.

Active cancellation and consistent environment integration are recommended
`0.6.0` corrections.

## Run the recipe

```bash
dotnet run cookbook/Clustering/08-training-progress.cs
```

## Next

- [Troubleshooting](troubleshooting.md)
- [General training guide](../../getting-started/training.md)
