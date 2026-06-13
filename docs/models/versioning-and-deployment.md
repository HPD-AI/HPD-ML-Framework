# Versioning and Deployment

Treat an HPD ML archive and its application reconstruction code as one
versioned deployment contract in 0.5.0.

## Record together

- HPD ML package versions
- archive format and manifest schema version
- concrete parameter type
- feature names, types, order, and vector dimensions
- preparation transform configuration and ordering
- output columns and meanings
- threshold, calibration, objective, or task configuration
- deterministic probe inputs and expected outputs
- platform and native-runtime requirements

## Deployment checklist

1. Load only archives from a trusted source.
2. Confirm expected format and application version.
3. Reconstruct the known scoring pipeline.
4. Check output schema.
5. Run probe predictions.
6. Reject non-finite outputs and unexpected dimensions.
7. Confirm native libraries are unnecessary or available as documented.
8. Keep the last known-good model for rollback.

The 0.5.0 ZIP loader has no archive resource limits, signature verification, or
complete compatibility metadata. Apply ordinary untrusted-file controls before
loading externally supplied archives.

## Run the recipe

```bash
dotnet run cookbook/Models/09-deployment-validation.cs
```
