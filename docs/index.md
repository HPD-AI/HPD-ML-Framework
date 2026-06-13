# HPD ML Documentation

## Getting Started

- [Learning path](getting-started/index.md)
- [Installation](getting-started/installation.md)
- [Package selection](getting-started/package-selection.md)
- [First model](getting-started/first-model.md)
- [Core workflow](getting-started/core-workflow.md)
- [Loading data](getting-started/loading-data.md)
- [Preparing data](getting-started/preparing-data.md)
- [Training](getting-started/training.md)
- [Prediction](getting-started/prediction.md)
- [Evaluation](getting-started/evaluation.md)
- [Saving and loading](getting-started/saving-and-loading.md)
- [Execution backends](getting-started/execution-backends.md)
- [Next steps](getting-started/next-steps.md)

## Guide Tracks

- [Core concepts](concepts/index.md)
- [Data](data/index.md)
- [Transforms](transforms/index.md)
- [Training](training/index.md)
- [Evaluation](evaluation/index.md)
- [Models](models/index.md)
- [Execution backends](backends/index.md)
- [Extending HPD ML](extending/index.md)
- [Operations](operations/index.md)
- [Reference](reference/index.md)

## Runnable Examples

The
[cookbook](https://github.com/HPD-AI/HPD-ML-Framework/tree/main/cookbook)
contains numbered, single-file C# apps
that declare their own NuGet dependencies and run directly with `dotnet run`.

## Documentation Status

Getting Started, Core Concepts, Data, Transforms, Binary Classification,
Regression, Clustering, Evaluation, Models and Persistence, Time Series, and
[Deep Learning](training/deep-learning/index.md), plus
[Extending HPD ML](extending/index.md) and
[Operations](operations/index.md) are verified against published
`HPD-ML-*` version `0.5.0`.

[LightGBM](training/lightgbm/index.md) is documented as an experimental
`0.5.0` preview. Its managed package is published, but it does not ship the
native `lib_lightgbm` runtime required for training. Deep Learning documents
managed dense networks as the portable baseline and MLX/PJRT as explicit
runtime-dependent providers.

## Writing Rules

Each guide should state:

1. What problem it solves.
2. Which NuGet packages it requires.
3. A minimal executable example or cookbook link.
4. Configuration and behavior details.
5. Failure modes and platform constraints.
6. Links to related guides and API reference.
