# Deployment Validation

Treat a model artifact and the code that reconstructs its scoring pipeline as
one versioned deployment unit.

## Before activation

1. Verify the artifact came from an approved source.
2. Check application and HPD ML package versions.
3. Validate the expected parameter type.
4. Reconstruct the known scoring transform when required.
5. Compare input and output schemas with a recorded contract.
6. Run deterministic probe rows.
7. Reject unexpected dimensions and non-finite outputs.
8. Verify native-runtime requirements before accepting traffic.
9. Keep the previous known-good deployment available for rollback.

In 0.5.0, ZIP topology loads as identity and saved inference state is not
returned. A successful `Load` therefore does not establish deployability.
Neural-network parameters can be reconstructed with the built-in writer, but
the scoring transform must still be rebuilt.

## Startup versus request path

Perform package inventory, native-runtime resolution, model loading, schema
checks, and probe predictions during startup or a deployment gate. Keep the
request path focused on already validated inference.

## Run the recipe

```bash
dotnet run cookbook/Operations/05-deployment-probe.cs
```

See [loading and validation](../models/loading-and-validation.md) for the
current persistence details.

