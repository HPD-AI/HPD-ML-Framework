# Execution Environments

`IExecutionEnvironment` carries optional execution context:

```text
logger
seed
cancellation token
progress routing
task scheduler
default device preference
backend request
```

```csharp
var environment = new DefaultExecutionEnvironment(
    seed: 42,
    cancellationToken: cancellationToken,
    backend: BackendSpec.Cpu());

var model = learner.Fit(new LearnerInput(
    TrainData: trainingData,
    Environment: environment));
```

## Backend specifications

Core provides factories for default, CPU, BLAS, MLX, PJRT, and LightGBM
requests. A `BackendSpec` is a request, not runtime installation or proof that
the learner supports that backend.

Managed classification, regression, clustering, and time-series learners
generally do not use backend selection in `0.5.0`. Deep Learning resolves
registered providers explicitly.

## Seeds

`SeededRandom.Create(seed)` provides deterministic `Random` sequences when a
seed exists. Individual learners decide whether an algorithm option or the
environment supplies their seed. There is no framework-wide `0.5.0`
precedence contract.

## Child environments

`DefaultExecutionEnvironment.CreateChild()` inherits cancellation, scheduler,
device, backend, and logger. Its default child seed is parent seed plus one.
When an event coordinator exists, child events bubble to the parent.

## Current boundaries

Supplying an environment does not guarantee that a learner honors every field:

- schedulers are generally unused;
- cancellation often does not interrupt active fitting;
- logging and progress routing are workload-specific;
- unsupported backend requests can be ignored outside backend-aware workloads.

## Run the recipe

```bash
dotnet run cookbook/Concepts/07-execution-environments.cs
```

## Next

- [Execution backends](../backends/index.md)
- [Sync, async, and cancellation](sync-async-and-cancellation.md)
