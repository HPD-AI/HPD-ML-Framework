# Training Runtime

`LightGbmLearner.Fit` performs five stages:

```text
materialize managed rows
create native datasets
train a native booster
export the booster as text
parse a managed tree ensemble
```

Native LightGBM is required through model export. The returned model then uses
managed scoring.

## Validation data

`LearnerInput.ValidationData` is converted to a native dataset that references
the training dataset:

```csharp
IModel model = learner.Fit(
    new LearnerInput(training, ValidationData: validation));
```

Check the exact `LearnerInput` construction syntax available in your build;
the important contract is that validation belongs to learner input rather
than `LightGbmOptions`.

## Early stopping limitations

`EarlyStoppingRounds` is active only when validation data is present.
Published 0.5.0:

- reads only the first native evaluation result;
- calls it `Eval`;
- assumes lower is always better;
- stops after the configured non-improving count;
- exports the final booster state rather than the best iteration.

The minimize-only comparison is incorrect for maximize metrics such as NDCG.
The exported model can also retain rounds after the best validation score.

Do not rely on 0.5.0 early stopping for model selection.

## Progress

Subscribe before fitting:

```csharp
using var subscription =
    learner.Progress.Subscribe(new ProgressPrinter());

sealed class ProgressPrinter : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"{value.Epoch}: {value.MetricName}={value.MetricValue}");

    public void OnCompleted() => Console.WriteLine("Complete");
    public void OnError(Exception error) =>
        Console.Error.WriteLine(error.Message);
}
```

Epochs are zero-based. If native LightGBM exposes no evaluation result, the
learner publishes zero as the score. Completion is published after the managed
model is created.

## Cancellation

`FitAsync(input, token)` uses `Task.Run`. The token can prevent task startup,
but active materialization and the native training loop never inspect it.
Cancellation after work begins is therefore not cooperative.

## Initial models

`LearnerInput.InitialModel` is ignored. Published 0.5.0 does not support
continued booster training or warm start.

## Native ownership

Training datasets and boosters are disposed on normal and exceptional exits.
The wrappers are named `SafeDatasetHandle` and `SafeBoosterHandle`, but they
implement ordinary `IDisposable` rather than deriving from .NET `SafeHandle`.
They have no finalization fallback if an object is abandoned.

## Feature importance

After training, the learner requests split-count importance. Failure is
silently swallowed and produces `null`.

When available:

```csharp
var parameters = (TreeEnsembleParameters)model.Parameters;
double[]? importance = parameters.FeatureImportance;
```

There is no gain-importance option, feature-name mapping, or diagnostic reason
when importance is absent.

## Recommendation

Treat all native training behavior as preview until a packed runtime,
end-to-end training tests, and managed prediction parity are available.

## Next

- [Managed scoring and model inspection](managed-scoring-and-model-inspection.md)
- [Troubleshooting](troubleshooting.md)
