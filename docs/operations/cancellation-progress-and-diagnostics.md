# Cancellation, Progress, and Diagnostics

Cancellation and progress are not uniform across 0.5.0 learners.

## Cancellation

Most built-in `FitAsync` implementations call:

```csharp
Task.Run(() => Fit(input), cancellationToken)
```

The token can prevent scheduled work from starting. Once synchronous
materialization or training begins, most learners do not observe it. The
environment cancellation token is also frequently ignored.

Row streams implemented by Core, CSV, JSON, split, and scan handles check
cancellation between rows. This does not make downstream synchronous training
cancellable. Although the Parquet handle's stream method contains a
cancellation check, published 0.5.0 cannot create or load that handle because
its Parquet metadata and column readers throw `NotImplementedException`.

Use process or worker isolation when a strict timeout must stop active 0.5.0
training. Do not return a partially trained model after a timeout.

## Progress

Subscribe before fitting. Metric names, epoch bases, cadence, and terminal
behavior vary by learner. Some learners emit completion with no intermediate
events.

`ProgressSubject` invokes observers synchronously while holding its lock and
does not enforce a terminal state. Keep observers fast, non-reentrant, and
exception-free. Dispose subscriptions.

The task result or thrown exception is authoritative; progress completion is
not sufficient proof of success.

## Diagnostics to record

- operation and model identifiers;
- package versions and backend request;
- row count and feature dimensions;
- seed and learner options;
- phase, epoch, metric name, and finite metric value;
- elapsed time and peak memory;
- cancellation request and observed completion times;
- complete exception details.

## Run the recipe

```bash
dotnet run cookbook/Operations/04-cancellation-and-progress.cs
```
