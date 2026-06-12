# HPD ML Framework

Documentation and runnable examples for the HPD ML Framework.

## Start Here

- [Documentation](docs/index.md)
- [Getting started](docs/getting-started/index.md)
- [Framework concepts](docs/concepts/index.md)
- [Cookbook](cookbook/README.md)
- [API reference](docs/reference/api/index.md)

## Documentation Model

The documentation is organized by developer workflow rather than source project:

1. Load schema-bearing data through an `IDataHandle`.
2. Prepare data with transforms.
3. Train an `ILearner`.
4. Apply the resulting `IModel`.
5. Evaluate, serialize, and deploy the model.

Package READMEs should remain compact landing pages. Detailed explanations belong
under `docs/`, while complete executable recipes belong under `cookbook/`.

## Repository Layout

```text
docs/
  getting-started/  Guided data-to-model learning path
  concepts/         Core architecture and execution model
  data/             Data sources, schemas, and output
  transforms/       Feature preparation guides
  training/         Workload and learner guides
  evaluation/       Metrics and validation
  models/           Prediction and persistence
  backends/         Managed, MLX, and PJRT execution
  extending/        Custom framework components
  operations/       Performance and troubleshooting
  reference/        Package and generated API reference
cookbook/
  GettingStarted/
  Data/
  Transforms/
  BinaryClassification/
  Regression/
  Clustering/
  DeepLearning/
  TimeSeries/
```
