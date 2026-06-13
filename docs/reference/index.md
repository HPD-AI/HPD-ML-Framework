# Reference

- [Package selection](../getting-started/package-selection.md)
- [Generated API reference](api/index.md)
- [API compatibility manifest](api/api-manifest.json)

The generated reference describes the public surface shipped by the published
`HPD-ML-*` version `0.5.0` packages. It is organized by package, namespace, and
type, and labels members that do not have published XML documentation.

Use the authored guides for behavioral contracts, supported workflows, and
known 0.5.0 limitations. Public visibility alone does not establish that a
low-level implementation type is a recommended application API.

## Regenerate

```bash
dotnet run --project tools/api-reference -- generate \
  --packages ~/.nuget/packages \
  --version 0.5.0 \
  --output docs/reference/api
```

Check that committed output is current:

```bash
dotnet run --project tools/api-reference -- check \
  --packages ~/.nuget/packages \
  --version 0.5.0 \
  --output docs/reference/api
```

Generation uses assembly metadata and package XML files without loading HPD ML
assemblies or optional native runtimes.
