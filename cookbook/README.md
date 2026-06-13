# HPD ML Cookbook

Recipes are grouped by user workload:

- `GettingStarted/`: smallest complete data-to-model workflows
- `Concepts/`: schemas, handles, execution, transforms, models, environments,
  ordering, and state
- `Extending/`: custom handles, rows, transforms, learners, environments,
  stateful execution, providers, testing, and serialization
- `Data/`: in-memory construction, file loading, schemas, row reading, and output
- `Transforms/`: composition, categorical data, missing values, normalization,
  text, images, and feature selection
- `BinaryClassification/`: logistic regression, SDCA, perceptron, and SVM
- `Regression/`: OLS, SDCA regression, gradient descent, and Poisson regression
- `Clustering/`: K-means and mini-batch K-means
- `Evaluation/`: task metrics, confusion matrices, cross-validation, and
  permutation importance
- `Models/`: model contracts, prediction schemas, parameters, ZIP persistence,
  custom writers, and deployment validation
- `Operations/`: memory, reproducibility, lifecycle, diagnostics, deployment,
  native runtimes, AOT, and artifact security
- `LightGBM/`: experimental package diagnostics and managed scoring
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

## Core Concepts

Run these in order:

```bash
dotnet run cookbook/Concepts/01-schema-bearing-data.cs
dotnet run cookbook/Concepts/02-cursors-streaming-and-materialization.cs
dotnet run cookbook/Concepts/03-lazy-and-cached-execution.cs
dotnet run cookbook/Concepts/04-transform-properties.cs
dotnet run cookbook/Concepts/05-compose-transforms.cs
dotnet run cookbook/Concepts/06-learners-models-and-parameters.cs
dotnet run cookbook/Concepts/07-execution-environments.cs
dotnet run cookbook/Concepts/08-ordering-and-state.cs
dotnet run cookbook/Concepts/09-complete-workflow.cs
```

Published `0.5.0` has strong shared abstractions but does not centrally enforce
every advertised property. These recipes expose lazy and cached execution,
capability and ordering metadata, learner/model boundaries, environment
inheritance, and current cancellation and state limitations.

## Extending HPD ML

Run these in order:

```bash
dotnet run cookbook/Extending/01-custom-data-handle.cs
dotnet run cookbook/Extending/02-custom-row-and-cursor.cs
dotnet run cookbook/Extending/03-custom-transform.cs
dotnet run cookbook/Extending/04-custom-learner-and-model.cs
dotnet run cookbook/Extending/05-progress-and-cancellation.cs
dotnet run cookbook/Extending/06-execution-environment.cs
dotnet run cookbook/Extending/07-stateful-scan-transform.cs
dotnet run cookbook/Extending/08-generator-transform.cs
dotnet run cookbook/Extending/09-custom-backend-provider.cs
dotnet run cookbook/Extending/10-contract-testing.cs
dotnet run cookbook/Extending/11-custom-parameter-writer.cs
```

These recipes implement the public extension contracts directly against
published `0.5.0` packages. They also expose the current projection,
typed-row, cancellation, progress, scan/generator, backend-registration, and
short-name serialization boundaries that the 0.6.0 Core and Extensibility
proposal addresses.

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

## Regression

Run these in order:

```bash
dotnet run cookbook/Regression/01-ordinary-least-squares.cs
dotnet run cookbook/Regression/02-sdca-regression.cs
dotnet run cookbook/Regression/03-online-gradient-descent-and-warm-start.cs
dotnet run cookbook/Regression/04-poisson-regression.cs
dotnet run cookbook/Regression/05-custom-input-columns.cs
dotnet run cookbook/Regression/06-training-progress.cs
dotnet run cookbook/Regression/07-compare-learners.cs
dotnet run cookbook/Regression/08-regression-metrics.cs
dotnet run cookbook/Regression/09-complete-regression-pipeline.cs
```

Published `0.5.0` materializes all regression training data. The recipes also
demonstrate its warm-start reset semantics, iterative OLS implementation,
Poisson scoring risk, and regression-metric edge behavior.

## Clustering

Run these in order:

```bash
dotnet run cookbook/Clustering/01-k-means.cs
dotnet run cookbook/Clustering/02-mini-batch-k-means.cs
dotnet run cookbook/Clustering/03-initialization-and-seeds.cs
dotnet run cookbook/Clustering/04-custom-feature-column.cs
dotnet run cookbook/Clustering/05-inspect-centroids.cs
dotnet run cookbook/Clustering/06-prediction-distances.cs
dotnet run cookbook/Clustering/07-clustering-metrics.cs
dotnet run cookbook/Clustering/08-training-progress.cs
dotnet run cookbook/Clustering/09-complete-segmentation.cs
```

Published `0.5.0` uses squared Euclidean distances and materializes the full
training set for both learners. The guides document batch K-means
first-iteration behavior, mini-batch memory semantics, evaluation edge cases,
and recommended future contract corrections.

## Evaluation

Run these in order:

```bash
dotnet run cookbook/Evaluation/01-binary-metrics.cs
dotnet run cookbook/Evaluation/02-confusion-matrix.cs
dotnet run cookbook/Evaluation/03-regression-metrics.cs
dotnet run cookbook/Evaluation/04-multiclass-metrics.cs
dotnet run cookbook/Evaluation/05-ranking-metrics.cs
dotnet run cookbook/Evaluation/06-clustering-metrics.cs
dotnet run cookbook/Evaluation/07-cross-validation.cs
dotnet run cookbook/Evaluation/08-permutation-feature-importance.cs
dotnet run cookbook/Evaluation/09-edge-cases-and-metric-direction.cs
```

Published `0.5.0` combines binary ranking, threshold, and probability
semantics; returns ambiguous zeros for several unavailable or degenerate
metrics; always maximizes `BestModel`; and assumes higher-is-better
permutation metrics. The guides and recipes expose those boundaries directly.

## Models and Persistence

Run these in order:

```bash
dotnet run cookbook/Models/01-model-contracts.cs
dotnet run cookbook/Models/02-prediction-and-schema.cs
dotnet run cookbook/Models/03-parameter-boundaries.cs
dotnet run cookbook/Models/04-neural-network-round-trip.cs
dotnet run cookbook/Models/05-inspect-zip-archive.cs
dotnet run cookbook/Models/06-topology-loading-boundary.cs
dotnet run cookbook/Models/07-custom-parameter-writer.cs
dotnet run cookbook/Models/08-failure-boundaries.cs
dotnet run cookbook/Models/09-deployment-validation.cs
```

Published 0.5.0 round-trips neural-network parameters and custom types with
registered writers. It does not reconstruct saved topology or return saved
inference state. Unregistered JSON fallback parameters cannot be loaded.

## Operations

Run these in order:

```bash
dotnet run cookbook/Operations/01-materialization-budget.cs
dotnet run cookbook/Operations/02-reproducible-splits.cs
dotnet run cookbook/Operations/03-lifecycle-and-cursors.cs
dotnet run cookbook/Operations/04-cancellation-and-progress.cs
dotnet run cookbook/Operations/05-deployment-probe.cs
dotnet run cookbook/Operations/06-native-runtime-inventory.cs
dotnet run cookbook/Operations/07-aot-readiness.cs
dotnet run cookbook/Operations/08-archive-resource-guard.cs
dotnet run cookbook/Operations/09-production-readiness-check.cs
```

These recipes turn published `0.5.0` boundaries into deployment checks:
materialization budgets, explicit seeds, cursor ownership, honest
cancellation, progress diagnostics, scoring probes, native-runtime inventory,
AOT certification, and archive resource limits.

## Time Series

Run these in order:

```bash
dotnet run cookbook/TimeSeries/01-iid-spike-detection.cs
dotnet run cookbook/TimeSeries/02-iid-change-point-detection.cs
dotnet run cookbook/TimeSeries/03-ssa-spike-detection.cs
dotnet run cookbook/TimeSeries/04-ssa-change-point-detection.cs
dotnet run cookbook/TimeSeries/05-spectral-residual-detection.cs
dotnet run cookbook/TimeSeries/06-ssa-forecasting.cs
dotnet run cookbook/TimeSeries/07-stateful-scan-control.cs
dotnet run cookbook/TimeSeries/08-seasonality-and-decomposition.cs
dotnet run cookbook/TimeSeries/09-complete-monitoring-workflow.cs
```

Published `0.5.0` keeps state per cursor and provides no checkpoint serializer.
The guides and recipes expose warm-up placeholders, current anomaly-statistic
semantics, FFT window rounding, and the SSA forecast-bound variance fallback.

## LightGBM Preview

Run these in order:

```bash
dotnet run cookbook/LightGBM/01-native-availability.cs
dotnet run cookbook/LightGBM/02-objective-output-schemas.cs
dotnet run cookbook/LightGBM/03-managed-tree-scoring.cs
```

These are intentionally preview recipes, not a native training walkthrough.
Published `HPD-ML-LightGBM` 0.5.0 contains the managed assembly but no
`lib_lightgbm` runtime asset or runtime-package dependency. The availability
recipe reports that boundary without failing; the remaining recipes exercise
managed schema and synthetic tree-scoring behavior only.

## Deep Learning

Run these in order:

```bash
dotnet run cookbook/DeepLearning/01-single-layer-regression.cs
dotnet run cookbook/DeepLearning/02-multilayer-relu-network.cs
dotnet run cookbook/DeepLearning/03-data-contracts.cs
dotnet run cookbook/DeepLearning/04-options-and-determinism.cs
dotnet run cookbook/DeepLearning/05-multi-output-boundary.cs
dotnet run cookbook/DeepLearning/06-optional-input-boundary.cs
dotnet run cookbook/DeepLearning/07-model-inspection.cs
dotnet run cookbook/DeepLearning/08-progress-and-cancellation.cs
dotnet run cookbook/DeepLearning/09-backend-boundaries.cs
```

Published 0.5.0 provides managed dense-network training with Identity and ReLU
activations. The recipes expose current batch, multi-output, optional-input,
progress, cancellation, parameter-mutability, and native-runtime boundaries.
MLX and PJRT are not presented as turnkey execution because their native
runtimes are not included in the package.
