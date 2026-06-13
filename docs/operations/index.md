# Operations

This track turns HPD ML's runtime contracts into practical production checks.
It documents published `0.5.0` behavior separately from safeguards an
application should add and changes proposed for `0.6.0`.

## Guides

1. [Performance and memory](performance-and-memory.md)
2. [Reproducibility](reproducibility.md)
3. [Lifecycle and concurrency](lifecycle-and-concurrency.md)
4. [Cancellation, progress, and diagnostics](cancellation-progress-and-diagnostics.md)
5. [Deployment validation](deployment-validation.md)
6. [Native runtime operations](native-runtime-operations.md)
7. [Native AOT and trimming](native-aot-and-trimming.md)
8. [Model artifact security](model-artifact-security.md)
9. [Production checklist and troubleshooting](production-checklist-and-troubleshooting.md)

## Operational posture for 0.5.0

- A streaming-looking `IDataHandle` does not guarantee bounded-memory
  training.
- `FitAsync` commonly schedules synchronous work and does not stop active
  training after cancellation.
- Seeds and execution-environment fields are workload-specific.
- Repeated and concurrent fitting or inference has no framework-wide safety
  contract.
- Installing a backend package is distinct from registering a provider and
  installing a native runtime.
- A successful ZIP load does not prove that the original scoring pipeline was
  reconstructed.
- Package AOT metadata is not a substitute for publishing and running the
  exact application path.

Use the
[Operations cookbook](https://github.com/HPD-AI/HPD-ML-Framework/tree/main/cookbook/Operations)
as a set of
small startup and deployment checks.
