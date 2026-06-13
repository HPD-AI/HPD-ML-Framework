#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

// Verify the managed package, then report whether native training is available.

using System.Runtime.InteropServices;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.LightGBM;

Console.WriteLine($"Runtime: {RuntimeInformation.RuntimeIdentifier}");

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f], [3f] }),
    ("Label", new[] { 0f, 1f, 2f, 3f }));

ILearner learner = ILearner.LightGbmRegression(
    options: new LightGbmOptions
    {
        NumberOfIterations = 2,
        MinDataInLeaf = 1
    });

Console.WriteLine("Managed LightGBM API loaded.");

try
{
    learner.Fit(new LearnerInput(data));
    Console.WriteLine("Native training is available.");
}
catch (DllNotFoundException)
{
    Console.WriteLine("Native training is unavailable: lib_lightgbm was not found.");
    Console.WriteLine("HPD-ML-LightGBM 0.5.0 does not ship native runtime assets.");
}

