# Loading and Validation

Reopen a file or reset a writable stream to position zero before loading:

```csharp
IModel loaded =
    serializer.Load(new ZipFormat(), source);
```

## What load means in 0.5.0

- Registered learned parameters are reconstructed.
- Missing learned parameters produce an internal empty-parameter sentinel.
- Saved topology is read but reconstructed as an identity transform.
- Saved inference state is not returned.

Therefore a successful `Load` does not prove that `loaded.Transform` performs
the original prediction.

For neural networks, rebuild scoring:

```csharp
var parameters =
    (NeuralNetworkParameters)loaded.Parameters;

IModel restored = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);
```

## Validate after loading

At minimum:

1. Check the concrete parameter type.
2. Reconstruct the expected scoring transform.
3. Compare the output schema with a recorded contract.
4. Run deterministic probe rows.
5. Compare predictions with stored expected values and tolerances.
6. Confirm the application and package versions accepted by deployment.

0.5.0 validates the format identifier but does not reject unknown manifest
schema versions.

## Run the recipe

```bash
dotnet run cookbook/Models/06-topology-loading-boundary.cs
```
