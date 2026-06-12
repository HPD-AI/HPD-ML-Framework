# HPD ML Cookbook

Recipes are grouped by user workload:

- `GettingStarted/`: smallest complete data-to-model workflows
- `Data/`: in-memory construction, file loading, schemas, row reading, and output
- `Transforms/`: composition, categorical data, missing values, normalization,
  text, images, and feature selection
- `BinaryClassification/`: logistic regression, SDCA, perceptron, and SVM
- `Regression/`: OLS, SDCA regression, gradient descent, and Poisson regression
- `Clustering/`: K-means and mini-batch K-means
- `DeepLearning/`: managed, MLX, and PJRT neural-network training
- `TimeSeries/`: forecasting, spike detection, and change-point detection

Every recipe should:

1. Run independently.
2. Declare its required packages.
3. Use a small local or generated dataset.
4. Print a meaningful prediction or metric.
5. Link to the corresponding guide under `docs/`.

## Getting Started

Run these in order:

```bash
dotnet run cookbook/GettingStarted/01-installation-check.cs
dotnet run cookbook/GettingStarted/02-core-workflow.cs
dotnet run cookbook/GettingStarted/03-loading-data.cs
dotnet run cookbook/GettingStarted/04-preparing-data.cs
dotnet run cookbook/GettingStarted/05-training.cs
dotnet run cookbook/GettingStarted/06-prediction.cs
dotnet run cookbook/GettingStarted/07-evaluation.cs
dotnet run cookbook/GettingStarted/08-model-persistence.cs
dotnet run cookbook/GettingStarted/09-execution-backends.cs
dotnet run cookbook/GettingStarted/10-complete-model.cs
```

The recipes pin `HPD-ML-*` packages to the published `0.5.0` NuGet baseline
used to verify this cookbook. The prose installation guides omit versions so
they follow the current stable release.

## Data

Run these in order:

```bash
dotnet run cookbook/Data/01-in-memory-columns.cs
dotnet run cookbook/Data/02-dictionaries-and-typed-rows.cs
dotnet run cookbook/Data/03-csv.cs
dotnet run cookbook/Data/04-json-and-json-lines.cs
dotnet run cookbook/Data/05-explicit-schema.cs
dotnet run cookbook/Data/06-cursors-and-streaming.cs
dotnet run cookbook/Data/07-materialization.cs
dotnet run cookbook/Data/08-writing-csv.cs
```

There is no Parquet recipe for `0.5.0` because its published reader and writer
throw `NotImplementedException`. See the [Parquet status
guide](../docs/data/parquet.md).

## Transforms

Run these in order:

```bash
dotnet run cookbook/Transforms/01-compose-transforms.cs
dotnet run cookbook/Transforms/02-categorical-encoding.cs
dotnet run cookbook/Transforms/03-missing-values.cs
dotnet run cookbook/Transforms/04-normalization.cs
dotnet run cookbook/Transforms/05-conversion-and-hashing.cs
dotnet run cookbook/Transforms/06-text-featurization.cs
dotnet run cookbook/Transforms/07-load-image-bytes.cs
dotnet run cookbook/Transforms/08-feature-selection.cs
dotnet run cookbook/Transforms/09-fit-once-reuse.cs
```

Image loading is the only runnable image transform in published `0.5.0`.
Resize and pixel extraction throw `NotImplementedException`; see the
[image support guide](../docs/transforms/image-support.md).

## Binary Classification

Run these in order:

```bash
dotnet run cookbook/BinaryClassification/01-logistic-regression.cs
dotnet run cookbook/BinaryClassification/02-sdca.cs
dotnet run cookbook/BinaryClassification/03-perceptron-and-warm-start.cs
dotnet run cookbook/BinaryClassification/04-linear-svm.cs
dotnet run cookbook/BinaryClassification/05-custom-input-columns.cs
dotnet run cookbook/BinaryClassification/06-margin-calibration.cs
dotnet run cookbook/BinaryClassification/07-training-progress.cs
dotnet run cookbook/BinaryClassification/08-compare-learners.cs
dotnet run cookbook/BinaryClassification/09-complete-text-classifier.cs
```

The `0.5.0` calibration recipe demonstrates two published defects: calibration
can reverse probability orientation, and it does not recompute
`PredictedLabel`. Do not use that calibrated path for production decisions.

The SDCA recipes also preserve a verified `0.5.0` regression: its update
direction can be reversed on separable data. SDCA is not recommended until its
`0.6.0` correction is package-certified.
