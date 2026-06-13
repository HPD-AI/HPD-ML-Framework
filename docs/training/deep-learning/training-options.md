# Training Options

Published 0.5.0 exposes:

| Option | Default | Validation | Managed meaning |
| --- | ---: | --- | --- |
| `Epochs` | `100` | Positive | Complete ordered passes |
| `LearningRate` | `0.01f` | Finite and positive | Per-row SGD step |
| `BatchSize` | `32` | Positive | Ignored |

The same `BatchSize` is used by MLX and PJRT providers to form actual
mini-batches. This provider-dependent meaning is a known 0.5.0 contract gap.

There is no high-level option for:

- loss selection;
- optimizer selection;
- Adam, despite lower-level trainable APIs existing;
- shuffling;
- regularization;
- momentum;
- learning-rate schedules;
- gradient clipping;
- early stopping.

The environment seed controls initialization. Reusing the same data,
definition, options, backend, and seed produces repeatable managed parameters.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/04-options-and-determinism.cs
```

