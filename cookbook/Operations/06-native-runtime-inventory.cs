#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Backends@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

// Inventory provider registration separately from native runtime files.

using System.Runtime.InteropServices;
using HPD.ML.Abstractions;
using HPD.ML.Backends.Mlx;
using HPD.ML.DeepLearning.Backends;

Console.WriteLine($"RID: {RuntimeInformation.RuntimeIdentifier}");
Console.WriteLine($"application directory: {AppContext.BaseDirectory}");

var mlx = new MlxDeepLearningBackendProvider(
    new MlxRuntimeOptions
    {
        NativeLibraryPath = "/deployment/runtime/libmlxc.dylib"
    });

Console.WriteLine($"MLX provider matches request: {mlx.CanHandle(BackendSpec.Mlx())}");

string[] expectedFiles =
[
    "lib_lightgbm.dylib", "lib_lightgbm.so", "lib_lightgbm.dll",
    "libmlxc.dylib", "libmlxc.so", "mlxc.dll",
    "pjrt_c_api_cpu_plugin.so", "pjrt_c_api_cpu_plugin.dll"
];

foreach (string name in expectedFiles)
{
    string path = Path.Combine(AppContext.BaseDirectory, name);
    if (File.Exists(path))
        Console.WriteLine($"found native candidate: {path}");
}

Console.WriteLine(
    "Published 0.5.0 packages contain managed provider code but no MLX, PJRT, or LightGBM runtime.");

// Guide: docs/operations/native-runtime-operations.md

