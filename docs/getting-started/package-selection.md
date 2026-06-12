# Package Selection

HPD ML is split into focused NuGet packages. Start with one workload package,
then add data, transform, evaluation, serialization, or accelerated-backend
packages as your application needs them.

## Choose the workload

Choose a workload from the question you want the model to answer, not from an
algorithm name:

| Question | Workload | Typical label or result |
| --- | --- | --- |
| Which of exactly two outcomes applies? | Binary classification | A `bool` label and predicted label |
| What numeric value should be predicted? | Regression | A numeric label and `Score` |
| Which rows naturally belong together? | Clustering | A cluster assignment and distance |
| What comes next in an ordered series, or is this point anomalous? | Time series | Forecast or anomaly result |
| Do I need a custom neural architecture? | Deep learning | Architecture-specific output |

After choosing the workload, choose a learner based on the data shape, training
cost, prediction cost, and quality you observe on held-out data. Training is
iterative: start with a simple learner, establish a measured baseline, and only
add complexity when it improves the result you care about.

`HPD-ML-LightGBM` is an algorithm package for boosted decision trees. Use it
with a supported supervised workload when tree-based learning is a better fit
than a linear or neural model.

## Common starting points

| Goal | Install |
| --- | --- |
| Binary classification | `HPD-ML-BinaryClassification` |
| Regression | `HPD-ML-Regression` |
| Clustering | `HPD-ML-Clustering` |
| Time-series forecasting or anomaly detection | `HPD-ML-TimeSeries` |
| Neural-network training | `HPD-ML-DeepLearning` |
| LightGBM training | `HPD-ML-LightGBM` |

Workload packages reference the framework packages they need. Add
`HPD-ML-Core` explicitly when your code directly constructs schemas, in-memory
data handles, models, or core transforms.

## Supporting packages

| Package | Use it for |
| --- | --- |
| `HPD-ML-Core` | Standard schemas, data handles, models, transform composition, and execution environment |
| `HPD-ML-DataSources` | CSV, JSON, Parquet, enumerable, dictionary, and row data sources |
| `HPD-ML-Transforms` | Categorical, missing-value, normalization, text, image, conversion, and feature-selection transforms |
| `HPD-ML-Evaluation` | Metrics, confusion matrices, cross-validation, and feature importance |
| `HPD-ML-Serialization-Zip` | ZIP model archives and custom parameter writers |
| `HPD-ML-Backends` | MLX and PJRT runtime implementations and accelerated deep-learning providers |

## Abstraction packages

Most applications do not need to install these directly:

| Package | Use it for |
| --- | --- |
| `HPD-ML-Abstractions` | Implementing framework-level data handles, transforms, learners, models, execution environments, or serializers |
| `HPD-ML-Backends-Abstractions` | Implementing trainable tensor and optimizer backends |

Use the abstraction packages when extending HPD ML itself or building an
integration library. Application code normally starts with `HPD-ML-Core` and a
workload package.

## Examples

### Binary classification

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-DataSources
dotnet add package HPD-ML-BinaryClassification
dotnet add package HPD-ML-Evaluation
```

### Regression with data preparation

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-DataSources
dotnet add package HPD-ML-Transforms
dotnet add package HPD-ML-Regression
dotnet add package HPD-ML-Evaluation
```

### Managed deep learning

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-DeepLearning
```

The deep-learning package includes a managed CPU provider. Add
`HPD-ML-Backends` only when selecting MLX or PJRT. Those providers also require
their native runtime libraries.

### Model persistence

```bash
dotnet add package HPD-ML-Serialization-Zip
```

ZIP serialization has model-specific parameter-writer requirements. The
built-in serializer can reload neural-network parameters without additional
registration; other learned-parameter types may require a custom writer.

## Package names

NuGet package IDs use hyphens, while C# namespaces use dots. For example:

```text
NuGet:     HPD-ML-BinaryClassification
Namespace: HPD.ML.BinaryClassification
```

## Next

- [Build your first model](first-model.md)
- [Load data](../data/index.md)
- [Prepare features](../transforms/index.md)
- [Choose a training workload](../training/index.md)
