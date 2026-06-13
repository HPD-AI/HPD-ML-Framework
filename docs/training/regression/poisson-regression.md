# Poisson Regression

Poisson regression models a positive expected rate:

```text
Score = exp(w.x + b)
```

Configure it through:

```csharp
ILearner learner = ILearner.PoissonRegression(
    options: new PoissonRegressionOptions
    {
        L2Regularization = 0.1f,
        MaxIterations = 100
    });
```

Labels less than zero throw. Zero and fractional non-negative labels are
accepted in published `0.5.0`.

## Options

The options and defaults match ordinary least squares:

```text
L1Regularization       0
L2Regularization       1
MemorySize             20
OptimizationTolerance  1e-7
MaxIterations          100
```

The same `0.5.0` regularization caveats apply: L2 includes the bias and L1 is a
post-training threshold rather than part of optimization.

Training clamps the linear score to `[-50, 50]` before exponentiation.
Prediction does not clamp it, then casts to `float`. Large fitted scores can
therefore become positive infinity during prediction. Use scaled features and
inspect outputs for finiteness. A shared finite scoring policy is planned for
`0.6.0`.

The learner ignores validation data, initial models, and execution
environments. Progress uses zero-based `Loss` events.

## Run the recipe

```bash
dotnet run cookbook/Regression/04-poisson-regression.cs
```

## Next

- [Prediction and evaluation](prediction-and-evaluation.md)
- [Troubleshooting](troubleshooting.md)

