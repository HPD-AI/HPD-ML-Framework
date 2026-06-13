# Core Concepts

HPD ML is built from a small set of contracts that remain consistent across
data preparation, training, prediction, evaluation, and persistence:

```text
schema-bearing IDataHandle
        |
        v
ITransform or ILearner
        |
        v
IModel
        |
        v
schema-bearing IDataHandle
```

Install the portable foundation:

```bash
dotnet add package HPD-ML-Core
```

Read these guides in order:

- [Architecture and packages](architecture-and-packages.md)
- [Schemas and field types](schemas-and-field-types.md)
- [Data handles, rows, and cursors](data-handles-rows-and-cursors.md)
- [Lazy, eager, and cached execution](lazy-eager-and-cached-execution.md)
- [Transforms and composition](transforms-and-composition.md)
- [Learners, models, and parameters](learners-models-and-parameters.md)
- [Execution environments](execution-environments.md)
- [Ordering and stateful execution](ordering-and-stateful-execution.md)
- [Sync, async, and cancellation](sync-async-and-cancellation.md)
- [Contract boundaries and troubleshooting](contract-boundaries-and-troubleshooting.md)

These pages describe verified published `0.5.0` behavior. Where the public
surface is broader than its implementation, the boundary is stated explicitly.

Run the parallel cookbook:

```bash
dotnet run cookbook/Concepts/01-schema-bearing-data.cs
```

For a shorter introduction, start with the [core workflow](../getting-started/core-workflow.md).
