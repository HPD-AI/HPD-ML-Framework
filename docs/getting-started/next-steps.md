# Next Steps

You now have the complete HPD ML lifecycle:

```text
load data
  -> prepare features
  -> train a learner
  -> apply the model
  -> evaluate
  -> save and load
```

Use the following paths to move from the introductory workflow into the part
of the framework relevant to your application.

## Run the complete example

The final Getting Started recipe combines preparation, training, prediction,
and evaluation in one file-based app:

```bash
dotnet run cookbook/GettingStarted/10-complete-model.cs
```

Keep that file as a small reference while exploring workload-specific
learners.

## Choose a workload

The cookbook is organized by machine-learning task:

- `cookbook/BinaryClassification/` for two-class prediction.
- `cookbook/Regression/` for numeric prediction.
- `cookbook/Clustering/` for grouping unlabeled rows.
- `cookbook/DeepLearning/` for managed, MLX, and PJRT neural networks.
- `cookbook/TimeSeries/` for forecasting and anomaly detection.

Each recipe is intended to run independently and declare its own NuGet
packages.

See the [Cookbook index](../../cookbook/README.md) for the current recipe
catalog.

## Work with real data

Continue with the [Data guide](../data/index.md) when replacing generated
examples with application data. It covers available sources and starts with
[in-memory data](../data/in-memory-data.md).

The broader data track will cover:

- CSV and JSON input, plus current Parquet support status.
- Schema and column inspection.
- Cursor-based row access.
- Writing and materializing data.
- Custom `IDataHandle` implementations.

## Build feature pipelines

Use the transforms track for categorical encoding, missing values,
normalization, text, images, and feature selection.

The important distinction is whether a transform is:

- Stateless and can be applied directly.
- Fitted from training data and must reuse the learned transform for
  validation, test, and prediction data.

Return to [Preparing Data](preparing-data.md) for the introductory pattern.

## Explore training and evaluation

Move into workload guides when selecting an algorithm, configuring its
options, or interpreting its parameters. The evaluation track expands beyond
a single metric into validation, cross-validation, and feature importance.

Start from:

- [Training](training.md)
- [Prediction](prediction.md)
- [Evaluation](evaluation.md)

## Persist and deploy models

The models track covers prediction contracts, learned parameters,
serialization, and the ZIP model format. Begin with
[Saving And Loading](saving-and-loading.md).

Before deployment, verify:

- The serializer supports the selected parameter type.
- The prediction input schema matches the training feature schema.
- The target runtime has every required native dependency.

## Configure accelerated execution

Use [Execution Backends](execution-backends.md) when neural-network training
needs MLX or PJRT. Start with managed CPU execution, then add a native provider
only when its platform requirements and performance benefits fit the
application.

## Extend the framework

The extension track is for custom:

- Data handles.
- Transforms.
- Learners and models.
- Deep-learning backend providers.
- Serializers.

Implement against abstractions from `HPD-ML-Abstractions`, and keep workload
or runtime dependencies in separate packages where possible.

## Browse the full map

The [documentation index](../index.md) lists the complete planned structure,
including concepts, operations, package reference, and generated API
documentation.
