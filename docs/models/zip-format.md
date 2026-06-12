# ZIP Model Format

Depending on `SaveContent`, an HPD ML ZIP archive can contain:

```text
manifest.json
parameters/
  weights.bin
  metadata.json
topology/
  pipeline.json
  schema.json
state/
  state.bin
```

An unregistered parameter type uses `parameters/parameters.json` instead of
writer-specific binary entries.

The manifest records format and schema versions, saved-content flags, UTC save
time, parameter type, transform entries, and whether inference state was
included.

## Inference state

Caller-owned state can be included while saving:

```csharp
serializer.Save(
    model,
    SaveContent.All,
    new ZipFormat(),
    destination,
    inferenceState: state);
```

The current `ISerializer.Load(...)` contract returns only `IModel`; it does not
return the saved state object. Applications must manage restoration separately.

The current topology loader also returns an identity transform. See
[Saving and Loading Models](../getting-started/saving-and-loading.md) before
using an archive as a deployable inference pipeline.
