# Averaged Perceptron

The averaged perceptron updates on margin violations and returns weights
averaged across all processed rows:

```csharp
ILearner learner = ILearner.AveragedPerceptron(
    options: new AveragedPerceptronOptions
    {
        LearningRate = 1.0,
        DecreaseLearningRate = false,
        L2Regularization = 0,
        NumberOfIterations = 10
    });
```

Progress uses `ErrorRate` and zero-based epochs. Rows are processed in source
order; there is no seed or shuffle option.

## Continue from a model

This is the only `0.5.0` binary learner that consumes `InitialModel`:

```csharp
IModel continued = learner.Fit(
    new LearnerInput(secondBatch, InitialModel: firstModel));
```

The implementation starts from the earlier final weights and bias. It does not
preserve the prior running average or update count, so this is weight
initialization rather than exact continuation of optimizer state. Feature
counts must match, but `0.5.0` does not provide an actionable validation error.

## Calibration

Supplying validation data fits a Platt calibrator. Without validation data,
the shared scoring transform still emits a sigmoid `Probability`, but that
value is not a calibrated probability.

The published `0.5.0` calibrator can reverse probability orientation and does
not recompute `PredictedLabel`. See
[Calibration and validation](calibration-and-validation.md) before using it.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/03-perceptron-and-warm-start.cs
```

## Next

- [Linear SVM](linear-svm.md)
- [Calibration and validation](calibration-and-validation.md)
