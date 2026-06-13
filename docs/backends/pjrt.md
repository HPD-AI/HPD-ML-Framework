# PJRT Backend

Install `HPD-ML-Backends`, register
`PjrtDeepLearningBackendProvider`, and supply a compatible PJRT plugin:

```csharp
var provider = new PjrtDeepLearningBackendProvider(
    new PjrtPluginResolverOptions
    {
        ExplicitPath = "/path/to/pjrt_c_api_cpu_plugin.so",
        Backend = "cpu"
    });
```

Request it with:

```csharp
BackendSpec.Pjrt("cpu")
```

The resolver checks explicit configuration, PJRT environment variables, and
prepared runtime directories. The published 0.5.0 package contains no PJRT
plugin.

CPU, CUDA, and ROCm names may be recognized by resolution code, but support
must be established separately for each platform, plugin, driver, and
architecture. A provider type or capability record is not proof that a device
is usable.

PJRT training uses true mini-batches and returns managed
`NeuralNetworkParameters`; inference then runs through the managed scorer.

