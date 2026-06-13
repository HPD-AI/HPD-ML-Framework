# Backend Providers

Published 0.5.0 uses `BackendSpec` for backend requests:

```csharp
BackendSpec.Cpu()
BackendSpec.Mlx("gpu")
BackendSpec.Pjrt("cpu")
new BackendSpec("custom")
```

There is not yet one general framework provider interface. Deep Learning
defines the workload-specific `IDeepLearningBackendProvider`:

```csharp
bool CanHandle(BackendSpec backend);
DeepLearningBackendCapabilities GetCapabilities(BackendSpec backend);
IDeepLearningTrainer CreateTrainer(
    DeepLearningBackendContext context);
```

## Provider sequence

A provider integration should separate:

```text
package installed
provider registered
request matched
capability supported
native runtime available
trainer created
```

Installing `HPD-ML-Backends` does not register MLX or PJRT providers and does
not install their native runtime.

Validate capabilities before expensive work. Explicit backend requests should
fail rather than silently fall back.

Published 0.5.0's `NeuralNetworkLearner` does not follow that preferred order:
it fully loads the training features and labels before resolving the provider
and validating its capabilities. An unsupported backend or network can
therefore fail only after paying the data-materialization cost. Provider
implementations should still validate promptly when called; moving provider
resolution ahead of data loading requires a framework correction.

## Resource ownership

If a trainer owns native resources, implement `IDisposable`. The current
`NeuralNetworkLearner` disposes disposable trainers after success or failure.
Providers that own longer-lived runtime resources need an application-level
ownership policy.

## Abstractions package boundary

`HPD-ML-Backends-Abstractions` 0.5.0 contains trainable tensor, optimizer, and
loss contracts. It is not a general HPD ML provider registry. The Deep
Learning 0.6.0 proposal moves computation ownership toward Helium while
retaining HPD ML workflow adapters.

## Run the recipe

```bash
dotnet run cookbook/Extending/09-custom-backend-provider.cs
```
