# Models and Persistence Troubleshooting

## Loaded predictions equal the input

The 0.5.0 topology loader returns identity. Reconstruct the scoring transform
from loaded parameters.

## Save succeeds but load says no writer is registered

The parameter type used JSON fallback. Register the same
`IParameterWriter<TParameters>` before save and load.

## Cast from `loaded.Parameters` fails

The archive did not contain learned parameters, used another parameter type,
or the expected writer was not registered.

## Loading from a just-written stream fails

Set `stream.Position = 0` or reopen the file.

## Predictions changed after inspecting parameters

Some 0.5.0 parameter APIs expose mutable arrays. Copy them before manipulation.

## Clustering results became inconsistent

Changing `Centroids` does not recompute `CentroidNormsSquared`. Do not mutate
fitted clustering parameters.

## Saved state is unavailable after load

0.5.0 does not return `state/state.bin`. Manage state restoration separately.

## Saving inference state fails in a file-based or Native AOT app

The 0.5.0 state path uses reflection-based JSON serialization for the runtime
object type. Reflection-disabled applications can reject it before saving.

## An archive with schema version greater than 1 loads

0.5.0 writes but does not validate the manifest schema version. Validate
deployment metadata at the application boundary.

## A malformed archive throws an unexpected exception

Some component entries are opened without deliberate missing-entry checks.
Treat any load failure as artifact rejection and log the original exception.

## Run the failure recipe

```bash
dotnet run cookbook/Models/08-failure-boundaries.cs
```
