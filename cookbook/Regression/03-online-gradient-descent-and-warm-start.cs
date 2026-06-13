#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Warm-start online gradient descent on a second batch.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Regression;

IDataHandle firstBatch = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-2f], [-1f], [0f], [1f] }),
    ("Label", new[] { -3f, -1f, 1f, 3f }));

IDataHandle secondBatch = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [2f], [3f], [4f], [5f] }),
    ("Label", new[] { 5f, 7f, 9f, 11f }));

OnlineGradientDescentOptions options = new()
{
    LearningRate = 0.01,
    NumberOfIterations = 20,
    AverageWeights = true
};

IModel first = ILearner.OnlineGradientDescent(options: options)
    .Fit(new LearnerInput(firstBatch));

IModel continued = ILearner.OnlineGradientDescent(options: options)
    .Fit(new LearnerInput(secondBatch, InitialModel: first));

IModel fresh = ILearner.OnlineGradientDescent(options: options)
    .Fit(new LearnerInput(secondBatch));

var firstParameters = (LinearModelParameters)first.Parameters;
var continuedParameters = (LinearModelParameters)continued.Parameters;
var freshParameters = (LinearModelParameters)fresh.Parameters;

Console.WriteLine($"First batch weight: {firstParameters.Weights[0]:F4}");
Console.WriteLine($"Warm-start weight: {continuedParameters.Weights[0]:F4}");
Console.WriteLine($"Fresh weight:      {freshParameters.Weights[0]:F4}");
Console.WriteLine(
    "Warm start restores weights and bias, but averaging and learning-rate " +
    "state restart for the new fit.");

