# Getting Started

HPD ML provides data loading, preparation, model training, prediction,
evaluation, persistence, and optional accelerated execution for .NET
applications.

This guide introduces the framework's main workflow. It assumes basic
familiarity with machine-learning concepts such as features, labels, fitting,
prediction, and held-out evaluation.

## Fast path

For the shortest route to a working model:

1. [Install HPD ML](installation.md).
2. [Train your first model](first-model.md).
3. [Understand the core workflow](core-workflow.md).
4. [Prepare application data](preparing-data.md).
5. [Evaluate held-out predictions](evaluation.md).

The first-model tutorial intentionally trains and evaluates on separate data.
Metrics calculated from training rows do not describe performance on unseen
data.

## Complete learning path

| Step | Page | What you learn |
| --- | --- | --- |
| 1 | [Installation](installation.md) | Install packages and verify HPD ML can create data. |
| 2 | [First Model](first-model.md) | Train, predict, and evaluate one complete model. |
| 3 | [Core Workflow](core-workflow.md) | Understand data handles, learners, models, and transforms. |
| 4 | [Package Selection](package-selection.md) | Choose the packages for a workload. |
| 5 | [Loading Data](loading-data.md) | Load memory, CSV, JSON, and Parquet sources. |
| 6 | [Preparing Data](preparing-data.md) | Transform columns into learner-ready features. |
| 7 | [Training](training.md) | Configure learners, validation data, progress, and execution. |
| 8 | [Prediction](prediction.md) | Apply models and read prediction columns. |
| 9 | [Evaluation](evaluation.md) | Calculate and interpret metrics. |
| 10 | [Saving And Loading](saving-and-loading.md) | Persist supported models with ZIP serialization. |
| 11 | [Execution Backends](execution-backends.md) | Choose managed, MLX, or PJRT execution. |
| 12 | [Next Steps](next-steps.md) | Continue into workload and extension guides. |

## Core lifecycle

```text
IDataHandle
  -> transforms
  -> ILearner.Fit/FitAsync
  -> IModel.Transform.Apply
  -> evaluation
  -> serialization
```

Not every workflow uses every stage. A learner can train directly from an
already prepared data handle, and prediction does not require evaluation or
serialization.

Learned preparation must be fitted from training data and then reused for
validation, test, and production rows. This keeps feature spaces consistent
and prevents test information from leaking into training.

## Parallel cookbook

| Guide topic | Runnable file |
| --- | --- |
| Installation | `01-installation-check.cs` |
| Core workflow | `02-core-workflow.cs` |
| Loading data | `03-loading-data.cs` |
| Preparing data | `04-preparing-data.cs` |
| Training | `05-training.cs` |
| Prediction | `06-prediction.cs` |
| Evaluation | `07-evaluation.cs` |
| Model persistence | `08-model-persistence.cs` |
| Execution backends | `09-execution-backends.cs` |
| Complete model | `10-complete-model.cs` |

Cookbook files pin `HPD-ML-*` packages to the published `0.5.0` NuGet baseline
used to verify these examples. Prose installation commands omit versions so
they follow the current stable release.
