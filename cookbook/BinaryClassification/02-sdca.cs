#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Demonstrate seeded repeatability and the published 0.5.0 direction defect.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f, -1f], [-1.5f, -0.5f], [-1f, -1.5f], [-0.5f, -1f],
        [0.5f, 1f], [1f, 0.5f], [1.5f, 1f], [2f, 1.5f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

var options = new SdcaOptions
{
    L2Regularization = 1.0,
    NumberOfIterations = 20,
    ConvergenceTolerance = 0,
    Seed = 42
};

IModel first = ILearner.Sdca(options: options).Fit(new LearnerInput(data));
IModel second = ILearner.Sdca(options: options).Fit(new LearnerInput(data));

var firstParameters = (LinearModelParameters)first.Parameters;
var secondParameters = (LinearModelParameters)second.Parameters;

Console.WriteLine(
    $"Weights: {string.Join(", ", firstParameters.Weights.Select(x => x.ToString("F4")))}");
Console.WriteLine($"Bias: {firstParameters.Bias:F4}");
Console.WriteLine(
    $"Repeatable: {firstParameters.Weights.SequenceEqual(secondParameters.Weights) &&
                    firstParameters.Bias == secondParameters.Bias}");

IDataHandle predictions = first.Transform.Apply(data);
IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    scoreColumn: "Probability").Apply(predictions);
using var metric = metrics.GetCursor(["Accuracy", "AUC"]);
metric.MoveNext();
Console.WriteLine($"Accuracy: {metric.Current.GetValue<double>("Accuracy"):P1}");
Console.WriteLine($"AUC: {metric.Current.GetValue<double>("AUC"):F3}");
Console.WriteLine("Do not use SDCA 0.5.0 for production training.");
