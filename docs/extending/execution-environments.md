# Execution Environments

`IExecutionEnvironment` carries optional execution context:

```text
logger
seed
cancellation token
progress factory
task scheduler
default device preference
backend request
```

Use `DefaultExecutionEnvironment` when an application does not need a custom
implementation:

```csharp
var environment = new DefaultExecutionEnvironment(
    seed: 42,
    cancellationToken: token,
    backend: BackendSpec.Cpu());
```

## Define what the learner consumes

Published 0.5.0 learners use different subsets of the environment. A custom
learner should document:

- seed precedence against learner options;
- whether active work checks cancellation;
- whether `Scheduler` is used;
- which messages are logged;
- whether device and backend requests are supported;
- whether unsupported requests are rejected or explicitly ignored.

Never silently fall back from an explicit backend request.

## Child environments

`DefaultExecutionEnvironment.CreateChild()`:

- inherits logger, cancellation, scheduler, device preference, and backend;
- derives `seed + 1` when the parent has a seed;
- creates a child event coordinator when the parent has one.

This is convenient for nested work, but it is not a general resource container
or dependency-injection scope.

## Custom environments

Implement `IExecutionEnvironment` only when execution context comes from
another host or scheduler. Keep properties immutable and ensure
`CreateProgress<T>` has a clear threading and delivery policy.

## Run the recipe

```bash
dotnet run cookbook/Extending/06-execution-environment.cs
```
