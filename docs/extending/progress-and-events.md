# Progress and Events

`ILearner.Progress` is an `IObservable<ProgressEvent>`. Subscribe before
calling `Fit` or `FitAsync`:

```csharp
using var subscription = learner.Progress.Subscribe(observer);
IModel model = learner.Fit(input);
```

`ProgressEvent` can carry:

- `Epoch`
- `MetricName`
- `MetricValue`
- `Checkpoint`

## Extension behavior

A custom learner should:

- emit finite, meaningful metrics at a documented cadence;
- use one documented epoch base;
- call `OnCompleted` exactly once after success;
- call `OnError` after a fitting failure;
- define cancellation terminal behavior;
- emit no events after termination.

`ProgressSubject` provides a small implementation and can route events through
HPD Events when constructed with an event coordinator.

Published 0.5.0 caveats:

- built-in learners use different metric names and epoch conventions;
- several learners complete without useful progress;
- failure and cancellation signaling is inconsistent;
- `ProgressSubject` invokes observers while holding its internal lock;
- disposing it calls completion but does not establish a strict terminal state.

Avoid slow, reentrant, or exception-throwing observers in current releases.
Keep one progress subject per fit when repeated fitting is supported.

## Environment progress

`IExecutionEnvironment.CreateProgress<T>(name)` is separate from
`ILearner.Progress`. Published 0.5.0 does not define a universal bridge between
them. An extension may use both, but document which events appear on each path.

## Run the recipe

```bash
dotnet run cookbook/Extending/05-progress-and-cancellation.cs
```
