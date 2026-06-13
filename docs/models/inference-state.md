# Inference State

Stateful time-series transforms update state as ordered rows are consumed.
Learned parameters and inference state are different:

```text
learned parameters  = fitted, reusable model knowledge
inference state     = mutable continuation position
training checkpoint = parameters plus optimizer/training continuation data
```

## ZIP behavior in 0.5.0

Save accepts an arbitrary state object:

```csharp
serializer.Save(
    model,
    SaveContent.All,
    new ZipFormat(),
    destination,
    inferenceState: state);
```

The object is serialized as JSON bytes to `state/state.bin`. The manifest only
records that state exists.

`Load` returns `IModel` and never returns the state payload. Built-in
time-series state serializers also return no usable checkpoint in 0.5.0.

The save path calls reflection-based `System.Text.Json` directly for the
runtime state type. In Native AOT and reflection-disabled file-based apps this
can throw `InvalidOperationException` before an archive is produced.

Do not claim stream continuation from a ZIP archive. Keep checkpointing
application-owned until a typed, versioned restore contract is implemented.

The Time Series and Models and Persistence 0.6.0 proposals require state type,
version, model identity, complete buffer content, and compatibility validation.
