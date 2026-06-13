# Model Artifact Security

Load HPD ML ZIP archives as untrusted structured input unless provenance has
already been established.

## 0.5.0 loader boundaries

The loader:

- copies the complete input sequence into a managed byte array;
- opens it as a ZIP archive;
- validates the format identifier;
- does not validate the manifest schema version;
- has no entry-count, compressed-size, expanded-size, or ratio limits;
- has no signature, checksum, or model fingerprint;
- assumes some manifest-referenced entries exist;
- does not execute code from the archive, but does deserialize JSON metadata.

## Application safeguards

Before calling `Load`:

1. Enforce a maximum compressed artifact size.
2. Inspect ZIP entry count, paths, expanded sizes, and compression ratios.
3. Reject duplicate critical entries.
4. Require exactly one expected manifest.
5. Validate provenance with an application signature or trusted digest.
6. Load in a memory- and time-limited worker when artifacts cross trust
   boundaries.
7. Validate parameter dimensions and run deployment probes after loading.

Do not extract entries to disk merely to inspect them. If extraction is
required, enforce canonical paths and prevent traversal.

## Run the recipe

```bash
dotnet run cookbook/Operations/08-archive-resource-guard.cs
```

