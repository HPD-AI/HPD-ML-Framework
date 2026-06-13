# Troubleshooting

## Index errors during training or prediction

Check every feature vector length. In `0.5.0`, the final training row defines
`FeatureCount`; inconsistent rows can be truncated or indexed past their end.

Use `float[]`, even for one feature. The scalar fallback is unreliable with
current row coercion and can throw while probing the value as `float[]`.

## SDCA returns NaN or infinity

Use a finite, positive `L2Regularization`. Zero or negative values are not
rejected before the implementation divides by `lambda * rowCount`.
Normalize large features and labels.

## Online gradient descent diverges

Reduce `LearningRate`, normalize features, or enable
`DecreaseLearningRate`. The default rate of `0.1` can be too large for
unscaled data. Row order also affects the fitted model.

## Warm start behaves unlike uninterrupted training

`InitialModel` restores only weights and bias. The learning-rate position,
weight-average accumulators, and sample count restart. Feature-count
mismatches are not safely validated in `0.5.0`.

## Poisson predictions are infinity

Training clamps its exponential input but scoring does not. Scale features,
increase regularization cautiously, and reject non-finite `Score` values.
Finite training progress does not guarantee finite prediction.

## OLS intercept is smaller than expected

The shared `0.5.0` L-BFGS optimizer applies L2 to the bias as well as weights.
L1 also thresholds the bias after training.

## Metrics show perfect zeros on empty data

`0.5.0` returns zero for all regression metrics when no rows are present.
Verify the prediction row count before treating the metric handle as valid.

For hand-built metric inputs, prefer `double[]` label and score columns.
Direct in-memory `float` values can be reinterpreted incorrectly when the
metric helper first probes them as `double`.

## Cancellation does not stop an active fit

The asynchronous methods schedule synchronous training. Cooperative
inner-loop cancellation is planned for `0.6.0`; use bounded iteration counts
in `0.5.0`.

## Run the complete pipeline

```bash
dotnet run cookbook/Regression/09-complete-regression-pipeline.cs
```

## Next

- [Regression overview](index.md)
- [Data contracts](data-contracts.md)
