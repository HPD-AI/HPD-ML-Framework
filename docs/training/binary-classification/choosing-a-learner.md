# Choosing a Learner

All four learners fit linear decision boundaries. Feature preparation usually
matters more than switching between them when the relationship is nonlinear.

| Need | Start with |
| --- | --- |
| Straightforward probabilistic linear baseline | Logistic regression |
| Seeded shuffled logistic-style training | Wait for the SDCA `0.6.0` correction |
| Weight initialization from an earlier linear model | Averaged perceptron |
| Margin-based classifier | Linear SVM; avoid its built-in `0.5.0` validation calibration |

## Logistic regression

Uses batch L-BFGS optimization and directly maps its linear score through a
sigmoid. It has no seed because the implementation does not shuffle rows.

## SDCA

Shuffles rows each epoch and supports an explicit seed. Despite the
memory-oriented description commonly associated with SDCA, the `0.5.0`
implementation loads every feature vector and label into memory first.

Published `0.5.0` also has a verified update-direction defect: on simple
linearly separable datasets it can learn scores ordered opposite to the
labels. Do not select it for production training until the implementation is
corrected and package-certified.

## Averaged perceptron

Processes rows in input order and averages weights across updates. It is the
only binary learner that reads `LearnerInput.InitialModel` in `0.5.0`.
Validation data fits a Platt calibrator after training.

## Linear SVM

Uses PEGASOS updates and optional weight projection. It supports seeded row
shuffling. Validation data fits a Platt calibrator.

## Compare rather than guess

Use the same prepared train/test split, fixed seeds where available, and an
appropriate held-out metric. These implementations have different option
scales, so do not compare default iteration counts as equivalent work.

```bash
dotnet run cookbook/BinaryClassification/08-compare-learners.cs
```

## Next

- [Logistic regression](logistic-regression.md)
- [Prediction and evaluation](prediction-and-evaluation.md)
