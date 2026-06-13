#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Backends@0.5.0
#:property TargetFramework=net10.0

// Distinguish provider registration from native runtime availability.

using HPD.ML.Abstractions;
using HPD.ML.Backends.Mlx;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.DeepLearning.Backends;

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(1, 1)]);

var defaultLearner = new NeuralNetworkLearner(definition);
try
{
    IDataHandle data = InMemoryDataHandle.FromColumns(
        ("Features", new float[][] { [0f] }),
        ("Label", new[] { 0f }));
    defaultLearner.Fit(new LearnerInput(
        data,
        Environment: new HPD.ML.Core.DefaultExecutionEnvironment(
            backend: BackendSpec.Mlx())));
}
catch (InvalidOperationException error)
{
    Console.WriteLine($"Without registration: {error.Message}");
}

var provider = new MlxDeepLearningBackendProvider(
    new MlxRuntimeOptions { NativeLibraryPath = "/definitely/missing/libmlxc.dylib" });

Console.WriteLine($"Provider handles MLX: {provider.CanHandle(BackendSpec.Mlx())}");
Console.WriteLine("The published package does not supply the MLX native library.");
