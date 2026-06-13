# Training Progress

Only SSA anomaly and forecasting components train learned parameters. Their
learners expose `IObservable<ProgressEvent>` through `Progress`.

Published `0.5.0` emits no intermediate progress events. It calls
`OnCompleted` after fitting succeeds:

```csharp
var observer = new CompletionObserver();
using var subscription = learner.Progress.Subscribe(observer);
IModel model = learner.Fit(new LearnerInput(training));
```

`FitAsync` schedules synchronous fitting with `Task.Run`. Its cancellation
token can prevent work from starting, but materialization, covariance
construction, and eigendecomposition do not observe cancellation after they
begin.

`LearnerInput.Environment`, validation data, and initial models are ignored.

Phase-level progress, `OnError`, linked environment cancellation, and active
loop cancellation are planned `0.6.0` corrections.

## Memory behavior

SSA materializes the complete selected series and constructs trajectory and
covariance arrays. Memory and compute grow with both series length and
`WindowSize`; test realistic settings before fitting large histories.

