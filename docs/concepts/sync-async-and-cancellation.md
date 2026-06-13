# Sync, Async, and Cancellation

HPD ML provides synchronous cursors and fitting alongside asynchronous row
streams and `FitAsync`.

## Row streaming

```csharp
await foreach (var row in data.StreamRows(cancellationToken))
{
    // Cancellation is checked between rows by compliant implementations.
}
```

This gives asynchronous enumeration semantics. Published `0.5.0` file sources
may still perform synchronous parsing and I/O.

## Fit versus FitAsync

```csharp
IModel sync = learner.Fit(input);
IModel asyncModel = await learner.FitAsync(input, cancellationToken);
```

Most `0.5.0` learners implement `FitAsync` by scheduling synchronous `Fit`
with `Task.Run`. The token can cancel before the task begins, but active
materialization and training loops usually do not observe it.

The environment also contains a cancellation token. There is no universal
`0.5.0` rule combining the explicit `FitAsync` token and environment token.

## Practical guidance

- Use `FitAsync` to avoid blocking the caller, not as proof of cooperative
  cancellation.
- Check workload-specific cancellation documentation.
- Cancel row streams when consumers no longer need more rows.
- Dispose cursors and subscriptions deterministically.
- Do not assume `TaskScheduler` is honored by a learner.

## Errors and progress

Progress observables do not share one complete terminal contract in `0.5.0`.
Some learners call completion after fitting but emit no intermediate events.
Treat the returned task or thrown exception as the authoritative operation
result.

## Run the recipes

```bash
dotnet run cookbook/Concepts/02-cursors-streaming-and-materialization.cs
dotnet run cookbook/Concepts/07-execution-environments.cs
```

## Next

- [Execution environments](execution-environments.md)
- [Contract boundaries and troubleshooting](contract-boundaries-and-troubleshooting.md)
