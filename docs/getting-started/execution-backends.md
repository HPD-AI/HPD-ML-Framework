# Execution Backends

Backend configuration affects how supported learners perform training. Start
with managed CPU execution, then select a native backend only when the
application has a measured need and the target platform can provide its
runtime.

## Choose a backend

| Choice | Package | Additional setup | Best starting point |
| --- | --- | --- | --- |
| Managed CPU | `HPD-ML-DeepLearning` | None | Yes |
| MLX CPU or GPU | `HPD-ML-DeepLearning` and `HPD-ML-Backends` | MLX native library and explicit provider | Apple-oriented native training |
| PJRT CPU, CUDA, or ROCm | `HPD-ML-DeepLearning` and `HPD-ML-Backends` | Compatible PJRT plugin and explicit provider | XLA/PJRT environments |

Selecting native execution has three separate requirements:

1. **Request a backend** through `BackendSpec`.
2. **Register a provider** with the learner.
3. **Supply the native runtime** required by that provider.

Completing only one or two of these steps is not enough. In particular,
installing `HPD-ML-Backends` does not register providers or install MLX and
PJRT native libraries.

Classical learners generally use their own managed implementations. This page
primarily applies to `NeuralNetworkLearner`.

## Configure the environment

An execution environment carries runtime settings into training:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Core;

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Cpu());

var input = new LearnerInput(
    TrainData: trainingData,
    Environment: environment);
```

`BackendSpec` describes the requested backend. A learner still needs a
provider capable of handling that request.

## Use the managed backend

`HPD-ML-DeepLearning` includes a managed provider by default. It requires no
native runtime and supports CPU training with `float` tensors, dense layers,
identity activation, and ReLU activation.

```csharp
var learner = new NeuralNetworkLearner(definition, options);

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Cpu());

IModel model = await learner.FitAsync(
    new LearnerInput(trainingData, Environment: environment));
```

The managed provider accepts `default`, `cpu`, and `managed` backend kinds.
Use it for the broadest portability and for workflows that do not need a
native accelerator.

Run the matching cookbook app:

```bash
dotnet run cookbook/GettingStarted/09-execution-backends.cs
```

## Select a native backend

Install `HPD-ML-Backends` in addition to the deep-learning package when using
MLX or PJRT:

```bash
dotnet add package HPD-ML-DeepLearning
dotnet add package HPD-ML-Backends
```

Then register the provider explicitly:

```csharp
using HPD.ML.DeepLearning;
using HPD.ML.DeepLearning.Backends;

var learner = new NeuralNetworkLearner(
    definition,
    options,
    backendProviders:
    [
        new ManagedDeepLearningBackendProvider(),
        new MlxDeepLearningBackendProvider(),
        new PjrtDeepLearningBackendProvider()
    ]);
```

Register only the providers your application intends to use. Installing the
package does not automatically change the learner's provider list.

### MLX

Request MLX with a GPU or CPU device:

```csharp
var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Mlx("gpu"));
```

MLX also needs its native library. Configure it through provider options:

```csharp
using HPD.ML.Backends.Mlx;

var mlxProvider = new MlxDeepLearningBackendProvider(
    new MlxRuntimeOptions
    {
        NativeLibraryPath = "/path/to/libmlxc.dylib",
        Device = MlxDeviceKind.Gpu,
        AllowCpuFallback = true
    });
```

When no explicit path is supplied, the resolver checks
`HELIUM_MLX_LIBRARY_PATH` and prepared runtime directories beneath the
application base directory or configured `SearchRoot`.

MLX GPU execution is primarily intended for Apple platforms. Building the
native runtime with Metal support can require the Xcode Metal toolchain.

### PJRT

Request a PJRT plugin by backend name:

```csharp
var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Pjrt("cpu"));
```

Configure the plugin resolver when the library is not in a prepared runtime
directory:

```csharp
using HPD.ML.Backends.Pjrt;

var pjrtProvider = new PjrtDeepLearningBackendProvider(
    new PjrtPluginResolverOptions
    {
        ExplicitPath = "/path/to/pjrt_c_api_cpu_plugin.so",
        Backend = "cpu"
    });
```

The resolver checks explicit configuration, `HELIUM_PJRT_PLUGIN_PATH`,
`PJRT_PLUGIN_PATH`, named plugin environment variables, and then
`runtimes/<rid>/native` directories. CPU, CUDA, and ROCm plugin names are
recognized.

Preparing a PJRT plugin from source requires XLA and Bazel or Bazelisk. The
framework source repository contains runtime preparation and probing tools;
those tools are not installed as executable commands by the NuGet package.

## Pass backend options through `BackendSpec`

Applications that construct backend settings dynamically can use the
`Options` dictionary:

```csharp
var backend = new BackendSpec(
    Kind: "mlx",
    Device: "gpu",
    Options: new Dictionary<string, string>
    {
        ["nativeLibraryPath"] = "/path/to/libmlxc.dylib",
        ["allowCpuFallback"] = "true"
    });
```

MLX recognizes `nativeLibraryPath`, `searchRoot`, and `allowCpuFallback`.
PJRT recognizes `explicitPath` and `searchRoot`.

## Understand the scope

The current MLX and PJRT providers accelerate neural-network training.
The resulting `NeuralNetworkScoringTransform` performs model scoring through
the framework's managed model implementation.

Most classical learners use managed implementations and may not consume the
environment backend setting. LightGBM is selected through its workload
package and learner APIs rather than through a deep-learning provider.

## Common failures

### No provider is registered

Selecting `BackendSpec.Mlx(...)` or `BackendSpec.Pjrt(...)` without registering
the corresponding provider throws `InvalidOperationException`.

### The native library cannot be found

Set the explicit library path, the documented environment variable, or place
the library under a prepared `runtimes/<rid>/native` directory.

### The model uses an unsupported feature

Backend compatibility is checked before training. A provider rejects
unsupported data types, activations, or training capabilities with
`NotSupportedException`.

### Native platform requirements are missing

Native backends depend on their platform toolchain, driver, runtime, and
architecture. Verify the native library independently before debugging the
model definition.

## Related guides

- [Training](training.md)
- [Prediction](prediction.md)
- [Package Selection](package-selection.md)
- [Next Steps](next-steps.md)
