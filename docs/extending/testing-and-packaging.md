# Testing and Packaging Extensions

Test extension behavior through both source tests and packed-package examples.

## Contract tests

For a data handle, test:

- schema and row-count claims;
- cursor lifecycle;
- active columns;
- exact and incompatible typed reads;
- cursor, stream, materialization, and batch agreement;
- cancellation and disposal.

For a transform, test:

- schema computation without row access;
- output schema and row agreement;
- ordering and row-count properties;
- repeated enumeration;
- independent state per cursor;
- output-name collisions.

For a learner, test:

- input and option validation;
- optional-input behavior;
- output schema agreement;
- cancellation during active work;
- progress completion and errors;
- repeated and concurrent fitting policy;
- cleanup after failure.

For a backend provider, test request matching, capability rejection, missing
runtime diagnostics, and trainer disposal.

## Package-only recipes

Every published extension should have at least one file-based app:

```csharp
#:package HPD-ML-Core@0.5.0
#:package Example.HPD.ML.Extension@1.0.0
#:property TargetFramework=net10.0
```

Run it from outside the extension project. This catches missing transitive
dependencies, internal API usage, namespace discovery problems, and omitted
runtime assets.

## Discovery

C# 14 extension members require both:

1. the extension package reference;
2. the namespace containing the extension declaration.

Keep public constructors available as a direct path and avoid hidden global
registration during package installation.

## Native AOT

Do not claim Native AOT support from abstraction design alone. Publish a
trimmed/AOT smoke test for the actual extension path, including serializers and
native runtime loading.

## Run the recipe

```bash
dotnet run cookbook/Extending/10-contract-testing.cs
```
