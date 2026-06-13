# Training Progress and Cancellation

`NeuralNetworkLearner.Progress` exists, but published 0.5.0 emits no epoch,
batch, or loss events. A successful fit only calls `OnCompleted`.

```csharp
using var subscription = learner.Progress.Subscribe(new ProgressPrinter());
learner.Fit(new LearnerInput(training));
```

`FitAsync` is implemented with `Task.Run`. Its cancellation token can prevent
scheduled work from starting, but active row loading and training loops do not
observe cancellation.

The environment cancellation token is also not checked during active work.
Do not rely on it to stop a long training run.

The 0.6.0 proposal calls for materialization/training cancellation and
one-based epoch loss progress with defined completion and error behavior.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/08-progress-and-cancellation.cs
```

