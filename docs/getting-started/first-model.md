# First Model

This tutorial trains a binary-classification model, predicts labels for
held-out rows, and evaluates the result.

## What you will build

By the end of this tutorial, you will have a console application that:

- creates separate training and test datasets;
- trains a logistic-regression model;
- scores rows the model did not see during training; and
- reports accuracy and area under the ROC curve (AUC).

## Before you begin

You need the .NET 10 SDK and a terminal. No external dataset, native runtime,
or accelerator is required; the example generates its data in memory and runs
on the CPU.

## Install the packages

Create a console application and add the core, learner, and evaluation packages:

```bash
dotnet new console -n HpdMlFirstModel
cd HpdMlFirstModel
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-BinaryClassification
dotnet add package HPD-ML-Evaluation
```

## Add the model workflow

Replace `Program.cs` with:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

var trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2.0f, -1.0f],
        [-1.8f, -0.7f],
        [-1.5f, -0.5f],
        [-1.0f, -1.5f],
        [1.0f, 1.0f],
        [1.5f, 0.5f],
        [1.8f, 0.8f],
        [2.0f, 1.5f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

var testData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-1.7f, -0.9f],
        [-0.8f, -1.1f],
        [0.9f, 1.2f],
        [1.7f, 0.9f]
    }),
    ("Label", new[] { false, false, true, true }));

var learner = ILearner.LogisticRegression();
var model = await learner.FitAsync(new LearnerInput(trainingData));
var predictions = model.Transform.Apply(testData);

var metrics = ITransform.BinaryClassificationMetrics().Apply(predictions);
using var row = metrics.GetCursor(["Accuracy", "AUC"]);
row.MoveNext();

Console.WriteLine($"Accuracy: {row.Current.GetValue<double>("Accuracy"):P0}");
Console.WriteLine($"AUC: {row.Current.GetValue<double>("AUC"):F2}");
```

Run the application:

```bash
dotnet run
```

You should see perfect accuracy and AUC for this deliberately small,
linearly separable example:

```text
Accuracy: 100%
AUC: 1.00
```

## What happens

`InMemoryDataHandle.FromColumns(...)` creates separate training and test
`IDataHandle` instances. A data handle carries the columns, schema, row count,
ordering, and available materialization capabilities.

`ILearner.LogisticRegression()` discovers the logistic-regression learner
through a C# 14 extension member. Its default column names are `Features` and
`Label`.

`FitAsync(...)` learns model parameters from `trainingData` and returns an
`IModel`. The test rows are not used during fitting.

`model.Transform.Apply(testData)` applies the model's scoring transform to
unseen rows. The result keeps the input columns and adds `Score`,
`Probability`, and `PredictedLabel`.

`ITransform.BinaryClassificationMetrics()` creates an evaluation transform.
Applying it produces a one-row data handle containing metrics such as
`Accuracy`, `AUC`, `F1Score`, `Precision`, `Recall`, and `LogLoss`.

Evaluating held-out rows matters even in a first example. Metrics calculated
from the same rows used for fitting are usually optimistic and should not be
treated as an estimate of production performance.

## Run the cookbook version

The file-based cookbook recipe includes pinned package versions and prints every
prediction before displaying the metrics:

```bash
dotnet run cookbook/GettingStarted/10-complete-model.cs
```

See
[`cookbook/GettingStarted/10-complete-model.cs`](../../cookbook/GettingStarted/10-complete-model.cs).

## Common problems

If `ILearner.LogisticRegression()` is unavailable, install
`HPD-ML-BinaryClassification` and import `HPD.ML.BinaryClassification`.

If `ITransform.BinaryClassificationMetrics()` is unavailable, install
`HPD-ML-Evaluation` and import `HPD.ML.Evaluation`.

The learner expects a `Features` column containing `float[]` values and a
`Label` column convertible to `bool`. Pass different column names to
`LogisticRegression(...)` when your schema uses other names.

## Next

- [Understand the core workflow](core-workflow.md)
- [Load application data](loading-data.md)
- [Prepare features](preparing-data.md)
- [Evaluate models](evaluation.md)
- [Save and load models](saving-and-loading.md)
