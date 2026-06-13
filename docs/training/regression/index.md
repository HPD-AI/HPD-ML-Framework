# Regression

Regression predicts a numeric value from a `float[]` feature column. The
published `0.5.0` loader contains a scalar fallback, but current row coercion
makes scalar `float` inputs unreliable; use one-element vectors instead.

Install:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-Regression
dotnet add package HPD-ML-Evaluation
```

Import `HPD.ML.Regression` to discover the C# 14 learner extension members on
`ILearner`.

## Learning path

1. [Data contracts](data-contracts.md)
2. [Choosing a learner](choosing-a-learner.md)
3. [Ordinary least squares](ordinary-least-squares.md)
4. [SDCA regression](sdca-regression.md)
5. [Online gradient descent](online-gradient-descent.md)
6. [Poisson regression](poisson-regression.md)
7. [Prediction and evaluation](prediction-and-evaluation.md)
8. [Training progress](training-progress.md)
9. [Troubleshooting](troubleshooting.md)

## Learners

| Learner | Main configuration | Current special behavior |
| --- | --- | --- |
| Ordinary least squares | L1/L2, L-BFGS memory and tolerance | Iterative L-BFGS, not a closed-form solver |
| SDCA regression | L2, iterations, tolerance, seed | Requires positive L2 in practice |
| Online gradient descent | learning rate, L2, averaging | Accepts `InitialModel` |
| Poisson regression | L1/L2, L-BFGS memory and tolerance | Positive exponential predictions |

All four published `0.5.0` learners materialize the complete training dataset,
return `LinearModelParameters`, and append a scalar `float` column named
`Score`.

The guides and recipes describe verified published `0.5.0` behavior. Contract
corrections planned for `0.6.0` are identified as planned work, not current
functionality.
