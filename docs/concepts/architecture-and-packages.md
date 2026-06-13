# Architecture and Packages

HPD ML separates contracts from implementations and workload packages.

## Package layers

| Package | Responsibility |
| --- | --- |
| `HPD-ML-Abstractions` | Interfaces and records such as `IDataHandle`, `ITransform`, `ILearner`, and `IModel` |
| `HPD-ML-Core` | Schemas, in-memory and wrapper handles, execution environments, basic transforms, and `Model` |
| `HPD-ML-DataSources` | CSV, JSON, in-memory adapters, and advertised Parquet APIs |
| `HPD-ML-Transforms` | Feature preparation |
| Workload packages | Learners and scoring transforms |
| `HPD-ML-Evaluation` | Metric transforms and validation workflows |
| `HPD-ML-Serialization-Zip` | Model archive support |

Most applications reference `HPD-ML-Core` plus the packages for their data,
transforms, workload, evaluation, and persistence needs. `HPD-ML-Core`
transitively supplies the abstractions package.

## Stable workflow

```csharp
IDataHandle data = ...;
ITransform preparation = ...;
IDataHandle prepared = preparation.Apply(data);

ILearner learner = ...;
IModel model = learner.Fit(new LearnerInput(prepared));

IDataHandle predictions = model.Transform.Apply(prepared);
```

`IDataHandle` keeps data and schema together. `ITransform` applies known
behavior. `ILearner` reads rows and returns an `IModel`. The model contains
executable inference plus learned parameters.

## Extension-member discovery

Workload and transform packages expose factories through C# 14 extension
members:

```csharp
using HPD.ML.BinaryClassification;
using HPD.ML.Transforms;

ILearner learner = ILearner.LogisticRegression();
ITransform transform = ITransform.Hash("Category");
```

Importing the package namespace is required for discovery.

## Published 0.5.0 boundary

The architecture is modular, but the contracts are not all centrally enforced:

- optional learner inputs may be ignored;
- execution-environment fields are supported per learner;
- transform ordering and resource metadata can be advisory;
- a reported capability does not always prove the fast path works;
- persistence does not yet reconstruct every complete model.

Use the workload guides for algorithm-specific guarantees.

## Run the recipe

```bash
dotnet run cookbook/Concepts/09-complete-workflow.cs
```

## Next

- [Schemas and field types](schemas-and-field-types.md)
- [Learners, models, and parameters](learners-models-and-parameters.md)
