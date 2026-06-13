# Saving And Loading Models

`HPD-ML-Serialization-Zip` stores model content in an inspectable ZIP archive:

```bash
dotnet add package HPD-ML-Serialization-Zip
```

Persistence support is model-specific in the current release. Read the
limitations below before treating the archive as a complete deployable
pipeline.

## Choose what to save

`SaveContent` is a flags enum:

| Value | Archive content |
| --- | --- |
| `LearnedParameters` | Learned weights and parameter metadata |
| `PipelineTopology` | Transform type names, configuration JSON, and schema information |
| `InferenceState` | Caller-supplied state bytes |
| `All` | All three categories |

Select only the content required by the application:

```csharp
serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    destination);
```

## Save to a stream

Create a `ZipSerializer` and write the model:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Serialization.Zip;

var serializer = new ZipSerializer();

await using var destination = File.Create("model.zip");

serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    destination);
```

The stream overload is provided by `SerializerExtensions`.

## Load from a stream

Reset or reopen the stream before loading:

```csharp
await using var source = File.OpenRead("model.zip");

IModel loaded =
    serializer.Load(new ZipFormat(), source);
```

The archive format identifier is `hpd-ml-zip-v1`. Loading rejects archives with
a different format identifier or a missing manifest.

## Built-in neural-network parameter support

`ZipSerializer` registers a writer for `NeuralNetworkParameters`. Those
parameters can be saved and loaded without extra registration:

```csharp
var parameters =
    (NeuralNetworkParameters)loaded.Parameters;
```

Reconstruct neural-network scoring explicitly:

```csharp
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IModel restored = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);
```

The cookbook verifies that the reconstructed model produces the same score as
the original trained model.

## Current topology limitation

The ZIP archive can write pipeline topology, but the current loader does not rebuild
that topology during loading. The loaded model receives an identity transform.

This means:

```csharp
IModel loaded = serializer.Load(format, source);
```

can contain valid loaded parameters while `loaded.Transform` does not perform
the original model's prediction pipeline.

For supported parameter types, reconstruct the scoring transform explicitly as
shown above. Keep any preparation transforms, their parameters, and their
ordering available separately until topology reconstruction is implemented.

Do not call `loaded.Transform.Apply(...)` and assume it scores rows merely
because the archive included `PipelineTopology`.

Other parameter types require a registered parameter writer. See
[Custom serialization](../extending/custom-serialization.md). For the archive
layout, manifest fields, and inference-state behavior, see
[ZIP model format](../models/zip-format.md).

## Save preparation and prediction together

If inference requires a learned featurizer followed by a classifier, preserving
only classifier parameters is insufficient. Production scoring needs:

1. the fitted preparation parameters
2. the preparation transform configuration
3. the classifier parameters
4. the scoring transform
5. the exact transform ordering

Because automatic topology reconstruction is not implemented, keep a
versioned application-level pipeline description or rebuild the chain
explicitly from supported loaded parameters.

## Run the cookbook example

The recipe trains a small managed neural network, saves its parameters, reloads
them, reconstructs scoring, and compares predictions:

```bash
dotnet run cookbook/GettingStarted/08-model-persistence.cs
```

See
[`cookbook/GettingStarted/08-model-persistence.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/08-model-persistence.cs).

## Common problems

Reopen a file or reset `stream.Position = 0` before loading from a stream that
was just written.

Use the same serializer registrations when saving and loading custom parameter
types.

Do not assume JSON fallback parameters are reloadable. Register a parameter
writer for a real round trip.

Do not assume loaded topology reconstructs inference. Rebuild the
supported scoring transform explicitly.

Validate archive and application versions together. The manifest schema version
is currently `1`, but application-level preparation contracts can change even
when the archive format remains readable.

## Next

- [ZIP model format](../models/zip-format.md)
- [Custom serialization](../extending/custom-serialization.md)
- [Choose an execution backend](execution-backends.md)
