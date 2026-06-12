#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Initialize a second averaged-perceptron fit from an earlier linear model.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle firstBatch = CreateData(-0.2f);
IDataHandle secondBatch = CreateData(0.2f);

IModel first = ILearner.AveragedPerceptron(
    options: new AveragedPerceptronOptions
    {
        NumberOfIterations = 5,
        LearningRate = 0.5
    }).Fit(new LearnerInput(firstBatch));

IModel continued = ILearner.AveragedPerceptron(
    options: new AveragedPerceptronOptions
    {
        NumberOfIterations = 5,
        LearningRate = 0.5
    }).Fit(new LearnerInput(secondBatch, InitialModel: first));

Print("First", (LinearModelParameters)first.Parameters);
Print("Continued", (LinearModelParameters)continued.Parameters);

IDataHandle predictions = continued.Transform.Apply(secondBatch);
using var rows = predictions.GetCursor(["Score", "PredictedLabel"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"score={rows.Current.GetValue<float>("Score"),7:F3} " +
        $"predicted={rows.Current.GetValue<bool>("PredictedLabel")}");
}

static IDataHandle CreateData(float offset) =>
    InMemoryDataHandle.FromColumns(
        ("Features", new float[][]
        {
            [-2f + offset], [-1f + offset], [-0.5f + offset],
            [0.5f + offset], [1f + offset], [2f + offset]
        }),
        ("Label", new[] { false, false, false, true, true, true }));

static void Print(string name, LinearModelParameters parameters) =>
    Console.WriteLine(
        $"{name}: weight={parameters.Weights[0]:F3}, bias={parameters.Bias:F3}");

