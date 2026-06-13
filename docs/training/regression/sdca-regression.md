# SDCA Regression

Configure seeded shuffled training:

```csharp
ILearner learner = ILearner.SdcaRegression(
    options: new SdcaRegressionOptions
    {
        L2Regularization = 0.1,
        NumberOfIterations = 30,
        ConvergenceTolerance = 1e-4,
        Seed = 42
    });
```

Each epoch shuffles materialized rows, updates dual variables and primal
weights, then reports the regularized squared loss.

## Options

| Option | Default |
| --- | ---: |
| `L2Regularization` | `1e-4` |
| `NumberOfIterations` | `20` |
| `ConvergenceTolerance` | `1e-4` |
| `Seed` | `null` |

Set a seed for repeatable shuffling. With no seed, the learner uses
`Random.Shared`.

`L2Regularization` must be finite and greater than zero in practice. Published
`0.5.0` does not validate this and can divide by zero or produce non-finite
parameters.

The learner ignores validation data, initial models, and execution
environments. Cancellation cannot interrupt active training. Progress uses
`SquaredLoss` and zero-based epochs.

## Run the recipe

```bash
dotnet run cookbook/Regression/02-sdca-regression.cs
```

## Next

- [Online gradient descent](online-gradient-descent.md)
- [Troubleshooting](troubleshooting.md)

