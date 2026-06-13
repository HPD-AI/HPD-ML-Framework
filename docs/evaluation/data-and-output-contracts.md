# Data and Output Contracts

Metric transforms read named columns and generally materialize consumed values
into managed collections.

Binary, multiclass, regression, ranking, and clustering evaluators return one
row with `double` metric columns. Confusion matrices return observed
`TrueLabel`, `PredictedLabel`, and `Count` rows.

Cross-validation aggregates to `Metric`, `Mean`, and `StdDev`. Permutation
importance returns `FeatureName`, `MetricDrop`, and `MetricDropStdDev`.

## Published 0.5.0 numeric boundary

Use `double` scalar columns for directly constructed metric data. Some
`0.5.0` row implementations probe `double` before `float` and can reinterpret
boxed float storage instead of converting it.

Most evaluators return zero-filled rows for empty input. Multiclass log loss
returns zero when score vectors are unusable, and clustering returns zero when
optional metric inputs are absent. These are compatibility behaviors, not
proof of perfect quality.
