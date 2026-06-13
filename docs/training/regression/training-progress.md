# Training Progress

Subscribe before fitting:

```csharp
using var subscription =
    learner.Progress.Subscribe(new ProgressPrinter());

IModel model = await learner.FitAsync(new LearnerInput(training));
```

| Learner | Metric | Frequency |
| --- | --- | --- |
| Ordinary least squares | `Loss` | L-BFGS iteration |
| SDCA regression | `SquaredLoss` | epoch |
| Online gradient descent | `SquaredLoss` | pass |
| Poisson regression | `Loss` | L-BFGS iteration |

Epoch values are zero-based. Current events contain no checkpoints.

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

Published `0.5.0` does not consistently validate non-finite loss. Treat `NaN`
or infinity as a failed fit even if a model is returned.

`FitAsync` uses `Task.Run`. Its cancellation token can cancel scheduling before
work begins, but active training loops do not observe cancellation.
`LearnerInput.Environment` is ignored by all four learners.

## Run the recipe

```bash
dotnet run cookbook/Regression/06-training-progress.cs
```

## Next

- [Troubleshooting](troubleshooting.md)
- [General training guide](../../getting-started/training.md)

