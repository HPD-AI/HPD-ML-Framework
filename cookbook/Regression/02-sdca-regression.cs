#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Demonstrate seeded SDCA regression and repeatable parameters.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Regression;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f, 1f], [-1f, -1f], [0f, 2f], [1f, 0f],
        [2f, -2f], [3f, 1f], [4f, -1f], [5f, 2f]
    }),
    ("Label", new[] { -6f, 0f, -1f, 4f, 10f, 8f, 14f, 13f }));

SdcaRegressionOptions options = new()
{
    L2Regularization = 0.1,
    NumberOfIterations = 60,
    ConvergenceTolerance = 0,
    Seed = 42
};

IModel first = ILearner.SdcaRegression(options: options)
    .Fit(new LearnerInput(data));
IModel second = ILearner.SdcaRegression(options: options)
    .Fit(new LearnerInput(data));

var a = (LinearModelParameters)first.Parameters;
var b = (LinearModelParameters)second.Parameters;

Console.WriteLine($"First weights:  {string.Join(", ", a.Weights.Select(x => x.ToString("F6")))}");
Console.WriteLine($"Second weights: {string.Join(", ", b.Weights.Select(x => x.ToString("F6")))}");
Console.WriteLine($"First bias: {a.Bias:F6}; second bias: {b.Bias:F6}");
bool repeatable = a.Weights.SequenceEqual(b.Weights) && a.Bias == b.Bias;
Console.WriteLine(
    "Repeatable: " +
    repeatable +
    " (positive L2 is required in practice)");
