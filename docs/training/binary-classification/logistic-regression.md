# Logistic Regression

Logistic regression fits a linear logit with an L-BFGS optimizer:

```csharp
ILearner learner = ILearner.LogisticRegression(
    options: new LogisticRegressionOptions
    {
        L1Regularization = 0f,
        L2Regularization = 0.1f,
        MemorySize = 20,
        OptimizationTolerance = 1e-7,
        MaxIterations = 100
    });
```

## Defaults

| Option | Default |
| --- | ---: |
| `L1Regularization` | `0` |
| `L2Regularization` | `1` |
| `MemorySize` | `20` |
| `OptimizationTolerance` | `1e-7` |
| `MaxIterations` | `100` |

Progress events use metric name `Loss` and zero-based iteration numbers.
Training is deterministic for deterministic input because this learner does
not shuffle rows.

The returned `Probability` is the sigmoid of the learned logit. `ValidationData`,
`InitialModel`, and `Environment` are ignored in `0.5.0`.

## Current implementation details

L2 regularization is applied to every optimized parameter, including the bias.
L1 is applied as soft thresholding after L-BFGS completes rather than as part
of the optimized objective. Invalid option ranges are not guarded consistently.

`LinearModelParameters.Statistics` exists but logistic regression does not
populate weight statistics.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/01-logistic-regression.cs
```

## Next

- [SDCA](sdca.md)
- [Prediction and evaluation](prediction-and-evaluation.md)

