# Image Support

Published `HPD-ML-Transforms` `0.5.0` can load file bytes from paths:

```csharp
ITransform load = ITransform.LoadImage(
    pathColumn: "Path",
    outputColumn: "Image");

IDataHandle images = load.Apply(data);
```

The source column must contain `string` paths. The output is a scalar
`byte[]` field. Files are read lazily with `File.ReadAllBytes(...)` when rows
are consumed. Missing, inaccessible, or invalid paths surface normal file I/O
exceptions.

## Unsupported operations

These APIs exist but throw `NotImplementedException` from `Apply(...)`:

```csharp
ITransform.ResizeImage(...);
ITransform.ExtractPixels(...);
```

`ExtractPixels(...).GetOutputSchema(...)` advertises a `float` vector with CHW
dimensions `[channels, height, width]`, but no pixels are produced.

There is intentionally no resize or pixel-extraction cookbook recipe for
`0.5.0`. Applications needing those operations must decode and prepare images
with an application-owned image library, then construct an `IDataHandle` from
the resulting arrays.

## Run the supported recipe

```bash
dotnet run cookbook/Transforms/07-load-image-bytes.cs
```

## Next

- [Conversion and hashing](conversion-and-hashing.md)
- [Data: in-memory vectors](../data/in-memory-data.md)

