# Troubleshooting

## Learner extension is unavailable

Install `HPD-ML-BinaryClassification`, target .NET 10, and import:

```csharp
using HPD.ML.BinaryClassification;
```

## Feature read fails

The learner reads a scalar `float` or `float[]`. Convert `double`, integer,
text, category, or image columns before fitting.

## Training fails inside an optimizer loop

Check for empty data, mixed vector lengths, null vectors, `NaN`, infinity, and
invalid option values. Published `0.5.0` does not consistently validate these
at the boundary.

## Evaluation reports extreme log loss

Pass `scoreColumn: "Probability"` to
`ITransform.BinaryClassificationMetrics(...)`. Its default `Score` column is a
raw margin or logit.

## Calibrated probability disagrees with predicted label

This is a known `0.5.0` behavior for SVM and perceptron. The fitted calibration
can also reverse probability orientation. Do not use that calibrated output
for production decisions; use the raw margin or an independently verified
calibrator. A framework fix is planned for `0.6.0`.

## Validation or initial model seems ineffective

Validation is consumed only by SVM and averaged perceptron. `InitialModel` is
consumed only by averaged perceptron. Other learners silently ignore those
values in `0.5.0`.

## Cancellation does not stop training

The token passed to `FitAsync` does not reach active optimizer loops in
`0.5.0`. Use small iteration limits while developing. Cooperative cancellation
is planned for `0.6.0`.

## Results vary between runs

Set `Seed` for SDCA and SVM. Logistic regression and perceptron do not shuffle
rows. Also keep source ordering and preparation deterministic.

## SDCA learns the opposite labels

This is a verified published `0.5.0` defect, not an expected tuning outcome.
Use logistic regression, averaged perceptron, or linear SVM until SDCA is
corrected and recertified for `0.6.0`.

## Next

- [Data contracts](data-contracts.md)
- [Choosing a learner](choosing-a-learner.md)
