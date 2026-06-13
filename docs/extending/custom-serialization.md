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

Published 0.5.0 indexes writers by `TypeName`, and the built-in registry uses
short parameter class names. Registering the same name again silently replaces
the previous writer. A custom writer's `TypeName` must exactly match
`typeof(TParams).Name`; otherwise saving does not find the writer and falls
back to the unloadable JSON representation. This short-name contract means
collisions between parameter types with the same class name cannot be avoided
in 0.5.0. Register the same writer implementation on every save and load path.

Use deterministic binary encoding, validate metadata and dimensions during
loading, and version application-level pipeline contracts alongside archives.

## Run the recipe

```bash
dotnet run cookbook/Extending/11-custom-parameter-writer.cs
```

See [Models and persistence](../models/index.md) for topology, archive, and
deployment boundaries. The equivalent Models-track recipe is
`cookbook/Models/07-custom-parameter-writer.cs`.
