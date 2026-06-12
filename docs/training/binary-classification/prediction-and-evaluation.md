# Prediction and Evaluation

Apply the fitted model to compatible rows:

```csharp
IDataHandle predictions = model.Transform.Apply(testData);
```

Published `0.5.0` appends:

- `Score`: raw linear margin or logit
- `Probability`: sigmoid or validation-calibrated probability
- `PredictedLabel`: thresholded value produced by the scoring transform

## Evaluate probability explicitly

The binary evaluator defaults to `scoreColumn: "Score"`, but it uses that
column for both threshold metrics and log loss. Raw scores are not
probabilities. Select `Probability` explicitly:

```csharp
IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    labelColumn: "Label",
    scoreColumn: "Probability",
    threshold: 0.5)
    .Apply(predictions);
```

This gives sensible threshold and log-loss semantics. AUC can rank either
monotonic probabilities or raw scores, so using `Probability` remains valid
for the current combined evaluator.

Do not use built-in calibrated SVM or perceptron probabilities for production
decisions in `0.5.0`. The fitted calibration can reverse probability
orientation, and `PredictedLabel` is not recalculated afterward. Use raw
`Score` for ranking and `Score >= 0` for the current margin decision until the
calibration path is corrected.

## Confusion matrix

`ITransform.ConfusionMatrix()` reads the existing `PredictedLabel` column.
Avoid the built-in calibrated path in `0.5.0`; evaluate the uncalibrated margin
decision or use an independently verified calibrator.

The `0.6.0` proposal separates ranking score, probability, and predicted-label
inputs in the evaluator.

## Run the recipes

```bash
dotnet run cookbook/BinaryClassification/01-logistic-regression.cs
dotnet run cookbook/BinaryClassification/08-compare-learners.cs
```

## Next

- [Training progress](training-progress.md)
- [General evaluation guide](../../getting-started/evaluation.md)
