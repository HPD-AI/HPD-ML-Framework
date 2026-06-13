# Extending HPD ML

HPD ML extensions implement the same small contracts used by built-in
packages:

```text
custom data
    -> IDataHandle / IRowCursor / IRow
    -> ITransform or ILearner
    -> IModel / ILearnedParameters
    -> optional backend and serialization integrations
```

Install the portable foundation:

```bash
dotnet add package HPD-ML-Core
```

Read these guides in order:

- [Custom data handles](custom-data-handles.md)
- [Custom rows and cursors](custom-rows-and-cursors.md)
- [Custom transforms](custom-transforms.md)
- [Custom learners and models](custom-learners-and-models.md)
- [Progress and events](progress-and-events.md)
- [Execution environments](execution-environments.md)
- [Stateful and generator transforms](stateful-and-generator-transforms.md)
- [Backend providers](backend-providers.md)
- [Custom serialization](custom-serialization.md)
- [Testing and packaging](testing-and-packaging.md)
- [Troubleshooting](troubleshooting.md)

These pages describe verified published `0.5.0` behavior. The framework has
useful extension points, but it does not yet centrally enforce every lifecycle,
ordering, cancellation, concurrency, and capability claim. The proposed
cross-cutting corrections are tracked in
`HPD.ML.CoreAndExtensibility.v0.6.0.Proposal.md`.

Run the parallel cookbook:

```bash
dotnet run cookbook/Extending/01-custom-data-handle.cs
```

For the underlying architecture, read [Core Concepts](../concepts/index.md).
