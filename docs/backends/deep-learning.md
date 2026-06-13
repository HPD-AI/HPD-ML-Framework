# Deep Learning Backends

Deep Learning 0.5.0 separates backend request, provider registration, and
native runtime availability.

| Backend | Provider | Runtime |
| --- | --- | --- |
| Managed CPU | Included and registered by default | None |
| MLX | Register from `HPD-ML-Backends` | External MLX native library |
| PJRT | Register from `HPD-ML-Backends` | External PJRT plugin |

```csharp
var learner = new NeuralNetworkLearner(
    definition,
    options,
    backendProviders:
    [
        new ManagedDeepLearningBackendProvider(),
        new MlxDeepLearningBackendProvider(),
        new PjrtDeepLearningBackendProvider()
    ]);
```

Installing `HPD-ML-Backends` does not register providers and does not install
native runtimes. Scoring remains managed after any provider trains.

Managed execution is the dependable published-package baseline. MLX and PJRT
should be treated as experimental until a real runtime is installed and the
target environment is independently certified.

## Related guides

- [MLX](mlx.md)
- [PJRT](pjrt.md)
- [Deep Learning training](../training/deep-learning/index.md)
- [Execution backend overview](../getting-started/execution-backends.md)

