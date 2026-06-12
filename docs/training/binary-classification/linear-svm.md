# Linear SVM

Linear SVM uses PEGASOS updates for hinge loss:

```csharp
ILearner learner = ILearner.LinearSvm(
    options: new LinearSvmOptions
    {
        Lambda = 0.001,
        NumberOfIterations = 10,
        PerformProjection = true,
        NoBias = false,
        Seed = 42
    });
```

## Options

`Lambda` controls regularization and the PEGASOS learning-rate schedule.
`PerformProjection` constrains the weight norm to `1 / sqrt(Lambda)`.
`NoBias` keeps the bias at zero. `Seed` controls epoch shuffling.

Progress uses `HingeLoss` and zero-based epochs. The reported value excludes
the regularization term.

`Lambda` must be positive in practice. Published `0.5.0` does not validate
zero or negative values before division and square-root operations.

## Probabilities

An SVM learns a margin, not a probability. Without validation data, `0.5.0`
still exposes `Probability = sigmoid(Score)`. Treat `Score` as the primary
uncalibrated output. Supplying validation data invokes a calibration path, but
that path has verified correctness defects in `0.5.0`. See
[Calibration and validation](calibration-and-validation.md).

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/04-linear-svm.cs
```

## Next

- [Calibration and validation](calibration-and-validation.md)
- [Prediction and evaluation](prediction-and-evaluation.md)
