# Native AOT and Trimming

HPD ML projects inherit `IsAotCompatible` for modern target frameworks.
`HPD-ML-Backends` and `HPD-ML-Backends-Abstractions` additionally enable the
AOT, trimming, and single-file analyzers.

These settings are engineering intent, not proof that every application path
publishes successfully.

## Known boundaries in 0.5.0

- `LambdaTransform` captures arbitrary delegates and is documented as not
  AOT-friendly.
- ZIP topology and unregistered parameter fallback serialize runtime types.
- ZIP inference-state saving uses runtime-type JSON serialization.
- `ZipSerializer` combines a source-generated JSON context with
  `DefaultJsonTypeInfoResolver`.
- Native backends dynamically resolve and load platform libraries.
- Published backend packages do not contain the MLX or PJRT runtime assets.

## Certification

Publish the actual application with its real serializers, transforms,
providers, runtime identifier, and native assets:

```bash
dotnet publish -c Release -r <rid> -p:PublishAot=true
```

Then run schema planning, model loading, probe inference, native startup, and
failure diagnostics on the published executable. Repeat for every supported
RID. Treat warnings as review items rather than suppressing them globally.

The source tree includes an MLX AOT smoke tool, but there is no repository-wide
0.5.0 AOT certification suite for all packages and workflows.

## Run the recipe

```bash
dotnet run cookbook/Operations/07-aot-readiness.cs
```

