# Training Progress

Subscribe before fitting:

```csharp
using var subscription =
    learner.Progress.Subscribe(new ProgressPrinter());

IModel model = await learner.FitAsync(new LearnerInput(training));
```

| Learner | Metric | Event frequency |
| --- | --- | --- |
| Logistic regression | `Loss` | optimizer iteration |
| SDCA | `LogLoss` | epoch |
| Averaged perceptron | `ErrorRate` | epoch |
| Linear SVM | `HingeLoss` | epoch |

Epoch values are zero-based. Events contain no checkpoint in these
implementations.

In SDCA `0.5.0`, reported `LogLoss` can increase or overflow because of the
known update-direction and numerical-stability defects. Treat those events as
diagnostics, not evidence of successful convergence.

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

`FitAsync` uses `Task.Run` in `0.5.0`. Its cancellation token can cancel
scheduling before work starts, but the inner training loops do not observe
active cancellation. `LearnerInput.Environment` is also ignored by all four
learners.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/07-training-progress.cs
```

## Next

- [Troubleshooting](troubleshooting.md)
- [General training guide](../../getting-started/training.md)
