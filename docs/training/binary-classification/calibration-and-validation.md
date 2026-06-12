# Calibration and Validation

In published `0.5.0`, validation data has learner-specific behavior:

| Learner | Validation behavior |
| --- | --- |
| Logistic regression | Ignored |
| SDCA | Ignored |
| Averaged perceptron | Fits Platt scaling |
| Linear SVM | Fits Platt scaling |

For SVM and perceptron:

```csharp
IModel model = learner.Fit(
    new LearnerInput(
        TrainData: training,
        ValidationData: validation));
```

The learner first fits linear weights from `training`. It scores `validation`,
then fits two parameters intended to remap `Score` into `Probability`.
Use validation rows prepared with the same fitted transforms as training rows.

## Current 0.5.0 defects

Do not use the built-in SVM or perceptron calibration for production decisions
in published `0.5.0`.

Verified package behavior has two defects:

- the fitted mapping can reverse probability orientation, assigning high
  probability to negative margins and low probability to positive margins;
- calibration replaces `Probability` but does not recompute `PredictedLabel`.

Calibration also lacks robust rejection for empty or single-class validation
sets. Until `0.6.0` corrects and certifies this path, use the raw `Score` for
ranking and derive the margin decision from `Score >= 0`, or calibrate outside
the current transform with a separately verified implementation.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/06-margin-calibration.cs
```

The recipe prints both columns to make the defect visible.

## Next

- [Prediction and evaluation](prediction-and-evaluation.md)
- [Troubleshooting](troubleshooting.md)
