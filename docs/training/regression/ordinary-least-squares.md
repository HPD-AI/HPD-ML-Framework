# Ordinary Least Squares

Create the learner through the extension member:

```csharp
ILearner learner = ILearner.OrdinaryLeastSquares(
    options: new OlsOptions
    {
        L2Regularization = 0.01f,
        MaxIterations = 100,
        OptimizationTolerance = 1e-7
    });
```

Published `0.5.0` minimizes squared loss with the shared L-BFGS optimizer. It
is not a closed-form ordinary least-squares solver.

## Options

| Option | Default | Meaning |
| --- | ---: | --- |
| `L1Regularization` | `0` | Post-training soft threshold |
| `L2Regularization` | `1` | L2 penalty |
| `MemorySize` | `20` | L-BFGS history size |
| `OptimizationTolerance` | `1e-7` | Gradient-norm stop threshold |
| `MaxIterations` | `100` | Optimizer iteration limit |

In `0.5.0`, L2 includes the bias. L1 is applied once after optimization to all
parameters, including the bias; it is not integrated into the optimized
objective. Mathematically honest weight-only regularization is planned for
`0.6.0`.

The learner ignores `ValidationData`, `InitialModel`, and `Environment`.
`FitAsync` wraps synchronous work and cannot interrupt an active optimizer.

Progress events use `MetricName = "Loss"` with zero-based epochs.

## Run the recipe

```bash
dotnet run cookbook/Regression/01-ordinary-least-squares.cs
```

## Next

- [SDCA regression](sdca-regression.md)
- [Training progress](training-progress.md)

