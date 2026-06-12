# Custom Serialization

`ZipSerializer` reloads parameter types with a registered
`IParameterWriter<TParams>`. Register the same writer before saving and loading:

```csharp
var serializer = new ZipSerializer();
serializer.RegisterParameterWriter(new MyParameterWriter());
```

A writer provides a stable `TypeName`, binary weights, JSON metadata, and
parameter reconstruction.

Without a registered writer, saving falls back to JSON. The current loader
cannot reconstruct that fallback into its concrete runtime type, so saving may
succeed while loading later fails with `No parameter writer registered`.

Use deterministic binary encoding, validate metadata and dimensions during
loading, and version application-level pipeline contracts alongside archives.
