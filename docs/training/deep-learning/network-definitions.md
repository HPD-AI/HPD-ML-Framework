# Network Definitions

A network is a non-empty connected list of dense layers:

```csharp
var definition = new NeuralNetworkDefinition(
    featureColumn: "Features",
    labelColumn: "Label",
    layers:
    [
        new DenseLayerSpec(2, 8, ActivationKind.ReLU),
        new DenseLayerSpec(8, 1, ActivationKind.Identity)
    ]);
```

Each `DenseLayerSpec` declares input width, output width, and activation.
Construction rejects non-positive dimensions and disconnected adjacent
layers.

Published 0.5.0 supports:

- `Identity`
- `ReLU`

It does not expose convolution, recurrent layers, attention, dropout,
normalization, embeddings, arbitrary modules, or custom activations through
`NeuralNetworkLearner`.

The first layer determines feature-vector width. The final layer determines
label width during training, but public scoring still emits only the first
output as scalar `Score`. Prefer a final width of one in 0.5.0.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/02-multilayer-relu-network.cs
```

## Next

- [Data and output contracts](data-and-output-contracts.md)
- [Managed training](managed-training.md)

