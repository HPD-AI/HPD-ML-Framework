# MLX Backend

Install both packages and register `MlxDeepLearningBackendProvider`:

```bash
dotnet add package HPD-ML-DeepLearning
dotnet add package HPD-ML-Backends
```

```csharp
var provider = new MlxDeepLearningBackendProvider(
    new MlxRuntimeOptions
    {
        NativeLibraryPath = "/path/to/libmlxc.dylib",
        Device = MlxDeviceKind.Gpu,
        AllowCpuFallback = false
    });
```

Request the backend with `BackendSpec.Mlx("gpu")`.

The resolver can use explicit options, `HELIUM_MLX_LIBRARY_PATH`, or prepared
runtime directories. The published 0.5.0 NuGet package does not contain the
MLX native library.

Provider capability records advertise float32 dense training with Identity
and ReLU, but that does not prove the runtime exists or initializes on the
current machine. Treat availability as a deployment concern.

MLX uses true mini-batches, unlike the managed 0.5.0 trainer. Parameters are
materialized back to managed arrays and inference is managed.

