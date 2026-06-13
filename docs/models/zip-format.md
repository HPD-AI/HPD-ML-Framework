# ZIP Model Format

Published 0.5.0 uses format identifier `hpd-ml-zip-v1` and manifest schema
version `1`.

Depending on `SaveContent`, an archive can contain:

```text
manifest.json
parameters/
  weights.bin
  metadata.json
  parameters.json
topology/
  pipeline.json
  schema.json
state/
  state.bin
```

Only one parameter representation is used. Registered writers create
`weights.bin` and `metadata.json`; unregistered types create
`parameters.json`.

## Manifest

The manifest records:

- format and schema versions
- saved-content flags
- UTC save time
- short parameter type name
- flattened transform entries
- whether inference state was supplied

Short type names are not globally stable identifiers and can collide.

## Topology entries

Each transform entry contains its short runtime type name and optional JSON
configuration. Serialization exceptions are swallowed and produce a null
configuration. Nested `ComposedTransform` instances are flattened.

Loading does not reconstruct these entries in 0.5.0.

## `schema.json`

Despite its name, this file does not store input and output columns. It stores:

```text
IsStateful
RequiresOrdering
PreservesRowCount
```

Keep an application-level schema contract when deployment requires validation.

## Malformed archives

Missing manifests and wrong format identifiers produce deliberate errors.
Some missing component entries can produce incidental exceptions because
0.5.0 assumes manifest-referenced files exist.

## Run the recipes

```bash
dotnet run cookbook/Models/05-inspect-zip-archive.cs
dotnet run cookbook/Models/08-failure-boundaries.cs
```
