# Deep Learning

Published `HPD-ML-DeepLearning` 0.5.0 provides dense feed-forward neural
networks with `Identity` and `ReLU` activations. Managed CPU training is the
portable baseline. Optional MLX and PJRT providers require
`HPD-ML-Backends`, explicit registration, and external native runtimes.

Install the managed path:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-DeepLearning
```

## Learning path

1. [Network definitions](network-definitions.md)
2. [Data and output contracts](data-and-output-contracts.md)
3. [Managed training](managed-training.md)
4. [Training options](training-options.md)
5. [Prediction and model inspection](prediction-and-model-inspection.md)
6. [Validation and warm start](validation-and-warm-start.md)
7. [Training progress and cancellation](training-progress-and-cancellation.md)
8. [Persistence](persistence.md)
9. [Execution backends](../../backends/deep-learning.md)
10. [Troubleshooting](troubleshooting.md)

## Current scope

| Capability | Published 0.5.0 |
| --- | --- |
| Layers | Dense only |
| Activations | `Identity`, `ReLU` |
| Loss | Squared error |
| Optimizer | SGD |
| Managed batching | `BatchSize` is ignored |
| Validation | Ignored |
| Warm start | Ignored |
| Scoring | Managed scalar `float` named `Score` |
| Native training | Optional MLX/PJRT providers, runtime supplied separately |

This is an experimental dense-network foundation, not a transformer or
general-purpose deep-learning framework. The versioned 0.6.0 proposal focuses
on making these contracts predictable before expanding the model family.

