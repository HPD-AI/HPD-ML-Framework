# Choosing a Learner

All four learners fit linear relationships. Nonlinear behavior must be
represented through prepared features or a different model family.

| Need | Start with |
| --- | --- |
| General linear numeric baseline | Ordinary least squares |
| Seeded shuffled squared-loss training | SDCA regression |
| Ordered updates or weight warm start | Online gradient descent |
| Positive count or rate prediction | Poisson regression |

## Ordinary least squares

Despite its name, the `0.5.0` implementation is an iterative L-BFGS
least-squares learner, not a normal-equation, QR, or SVD solver. It has no seed
because it does not shuffle rows.

## SDCA regression

Shuffles rows every epoch and supports an explicit seed. It requires positive
L2 regularization in practice because its update divides by `lambda * n`, but
`0.5.0` does not validate the option.

## Online gradient descent

Processes rows in input order. It is the only regression learner that consumes
`LearnerInput.InitialModel`. Its warm start reuses weights and bias but resets
the learning-rate schedule and averaging accumulators.

## Poisson regression

Fits a log-linked expected rate and emits `exp(w.x + b)`. Labels must be
non-negative. Use it only when a positive rate model matches the target,
rather than merely because labels happen to be positive.

## Compare rather than guess

Use the same prepared train/test split. These learners have different
objectives and option scales, so equal iteration counts are not equal work.

```bash
dotnet run cookbook/Regression/07-compare-learners.cs
```

## Next

- [Ordinary least squares](ordinary-least-squares.md)
- [Prediction and evaluation](prediction-and-evaluation.md)

