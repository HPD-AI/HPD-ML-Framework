# Binary Classification

Binary classification predicts one of two labels from a scalar `float` or
`float[]` feature column.

Install:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-BinaryClassification
dotnet add package HPD-ML-Evaluation
```

Import `HPD.ML.BinaryClassification` to discover the C# 14 learner extension
members on `ILearner`.

## Learning path

1. [Data contracts](data-contracts.md)
2. [Choosing a learner](choosing-a-learner.md)
3. [Logistic regression](logistic-regression.md)
4. [SDCA](sdca.md)
5. [Averaged perceptron](averaged-perceptron.md)
6. [Linear SVM](linear-svm.md)
7. [Calibration and validation](calibration-and-validation.md)
8. [Prediction and evaluation](prediction-and-evaluation.md)
9. [Training progress](training-progress.md)
10. [Troubleshooting](troubleshooting.md)

## Learners

| Learner | Main configuration | Current special behavior |
| --- | --- | --- |
| Logistic regression | L1/L2, L-BFGS memory and tolerance | Native logistic probability |
| SDCA | L2, iterations, tolerance, seed | Known `0.5.0` update-direction defect |
| Averaged perceptron | learning rate, L2, iterations | Accepts `InitialModel`; validation calibrates |
| Linear SVM | lambda, iterations, projection, seed | Validation calibrates |

All four learners load their complete training data into memory in published
`0.5.0`. They return `LinearModelParameters` and append `Score`,
`Probability`, and `PredictedLabel` during scoring.

Do not use published `0.5.0` SDCA for production training. Verification on
separable datasets shows its updates can learn the opposite label direction.

The guides and recipes describe verified published `0.5.0` behavior. Known
contract corrections planned for `0.6.0` are identified as planned work, not
current functionality.
