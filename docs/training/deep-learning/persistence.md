# Persistence

Published 0.5.0 has a verified package round trip for
`NeuralNetworkParameters`, including the network definition, layer dimensions,
activations, weights, and biases.

This is a parameter round trip, not a complete model-pipeline round trip.
`ZipSerializer.Load(...)` returns an identity transform because topology
reconstruction is not implemented. Rebuild `NeuralNetworkScoringTransform`
explicitly from the loaded parameters before prediction.

A complete persistence contract needs:

```text
network definition
layer dimensions and activations
weights and biases
format and model version
output interpretation
```

Inference persistence is separate from resumable training. Exact training
continuation would additionally need optimizer state, completed epoch/batch,
and shuffle state.

The constructor copies incoming arrays, but arrays exposed through `Weights`
and `Biases` remain mutable. Treat them as read-only.

The 0.6.0 proposals require immutable parameters and a versioned complete-model
round trip. Until then, retain the application code and configuration needed
to reconstruct scoring.

## Verified recipe

```bash
dotnet run cookbook/Models/04-neural-network-round-trip.cs
```

## Related guides

- [Model inspection](prediction-and-model-inspection.md)
- [General saving and loading](../../getting-started/saving-and-loading.md)
- [Models and persistence](../../models/index.md)
