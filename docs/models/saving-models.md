# Saving Models

Install:

```bash
dotnet add package HPD-ML-Serialization-Zip
```

`SaveContent` selects archive categories:

| Flag | Written content |
| --- | --- |
| `LearnedParameters` | registered binary parameter payload, or JSON fallback |
| `PipelineTopology` | flattened transform entries and transform properties |
| `InferenceState` | caller-supplied state serialized as JSON bytes |
| `All` | all flags |

## Neural-network parameters

0.5.0 registers `NeuralNetworkParameterWriter` automatically:

```csharp
var serializer = new ZipSerializer();
serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    destination);
```

This is a verified parameter round trip. It is not a complete pipeline round
trip because topology loading returns identity.

## Unregistered parameters

For an unregistered type, save writes
`parameters/parameters.json`. The current loader cannot reconstruct the runtime
type and throws. Treat this as an inspection artifact, not persistence.

Register a writer before both saving and loading custom parameters:

```csharp
serializer.RegisterParameterWriter(
    new MyParameterWriter());
```

See [Custom serialization](../extending/custom-serialization.md).

## Save the full inference contract

Production inference may require learned preparation, its configuration,
prediction parameters, output configuration, and exact ordering. Since 0.5.0
cannot rebuild topology, keep an application-owned, versioned reconstruction
contract.

## Run the recipes

```bash
dotnet run cookbook/Models/04-neural-network-round-trip.cs
dotnet run cookbook/Models/05-inspect-zip-archive.cs
```
