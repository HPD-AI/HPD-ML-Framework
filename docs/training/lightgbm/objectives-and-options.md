# Objectives and Options

`LightGbmLearner` uses one options record for every task:

```csharp
var options = new LightGbmOptions
{
    Objective = LightGbmObjective.Regression,
    NumberOfIterations = 100,
    LearningRate = 0.1,
    NumberOfLeaves = 31
};
```

Named extensions override the objective:

```csharp
ILearner.LightGbmBinaryClassification(...)
ILearner.LightGbmRegression(...)
ILearner.LightGbmMulticlass(numberOfClasses, ...)
ILearner.LightGbmRanking(...)
```

## Objective mapping

| HPD ML objective | Native parameter | Output columns |
| --- | --- | --- |
| `Binary` | `binary` | `Score`, `Probability`, `PredictedLabel` |
| `Multiclass` | `multiclass` | vector `Score`, `PredictedLabel` |
| `Regression` | `regression` | `Score` |
| `RegressionMae` | `regression_l1` | `Score` |
| `Huber` | `huber` | `Score` |
| `Poisson` | `poisson` | `Score` |
| `Tweedie` | `tweedie` | `Score` |
| `Ranking` | `lambdarank` | `Score` |

Binary managed output treats `Score` as a raw margin, applies sigmoid for
`Probability`, and predicts `true` at probability `>= 0.5`.

Multiclass managed output applies softmax and stores probabilities in `Score`.
`PredictedLabel` is the zero-based `uint` argmax.

Regression-family and ranking objectives share the scalar managed scoring
path. Their exact response-scale fidelity has not been compared against native
prediction.

## Defaults

Important published defaults include:

```text
Objective              Regression
Boosting               Gbdt
NumberOfIterations     100
LearningRate           0.1
NumberOfLeaves         31
MaxDepth               -1
MinDataInLeaf          20
FeatureFraction        1.0
BaggingFraction        1.0
BaggingFrequency       0
MaxBin                 255
HandleMissingValue     true
UseZeroAsMissing       false
EarlyStoppingRounds    0
Seed                    null, forwarded as 0
NumberOfThreads        0
Verbosity              -1
```

Published 0.5.0 forwards options without comprehensive HPD-side range or
combination validation. Invalid settings generally fail through native
LightGBM after the data has been materialized.

## Boosting modes

The API exposes:

```text
Gbdt
RandomForest
Dart
Goss
```

Random-forest mode normally requires compatible bagging and feature-sampling
settings. HPD ML does not validate those combinations before native training.

## Multiclass

Use the named extension so the class count is provided:

```csharp
ILearner learner = ILearner.LightGbmMulticlass(numberOfClasses: 3);
```

The generic extension can select `Multiclass` without a class count. Published
0.5.0 does not reject that configuration before native training.

## Determinism

`Seed` is forwarded to LightGBM, defaulting to zero when null. Thread count,
boosting mode, sampling, platform, and native LightGBM build can affect
reproducibility. `LearnerInput.Environment` does not override the seed.

## Inspect schemas without training

This complete file-based example uses only published packages:

```csharp
#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.LightGBM;

ISchema input = new SchemaBuilder()
    .AddVectorColumn<float>("Features", 4)
    .AddColumn<float>("Label")
    .Build();

var learners = new (string Name, ILearner Learner)[]
{
    ("Regression", ILearner.LightGbmRegression()),
    ("Binary", ILearner.LightGbmBinaryClassification()),
    ("Multiclass", ILearner.LightGbmMulticlass(3)),
    ("Ranking", ILearner.LightGbmRanking())
};

foreach (var (name, learner) in learners)
{
    ISchema output = learner.GetOutputSchema(input);
    string added = string.Join(
        ", ",
        output.Columns
            .Where(column => input.FindByName(column.Name) is null)
            .Select(column => column.Name));

    Console.WriteLine($"{name}: {added}");
}
```

## Next

- [Training runtime](training-runtime.md)
- [Managed scoring and model inspection](managed-scoring-and-model-inspection.md)
