#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Train a seeded PEGASOS linear SVM and inspect its margin scores.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f, -1f], [-1.5f, -0.5f], [-1f, -1f],
        [1f, 1f], [1.5f, 0.5f], [2f, 1.5f]
    }),
    ("Label", new[] { false, false, false, true, true, true }));

ILearner learner = ILearner.LinearSvm(
    options: new LinearSvmOptions
    {
        Lambda = 0.01,
        NumberOfIterations = 20,
        PerformProjection = true,
        Seed = 42
    });

IModel model = learner.Fit(new LearnerInput(training));
var parameters = (LinearModelParameters)model.Parameters;

double norm = Math.Sqrt(parameters.Weights.Sum(weight => weight * weight));
Console.WriteLine($"Weight norm: {norm:F3}");
Console.WriteLine($"Projection bound: {1.0 / Math.Sqrt(0.01):F3}");

IDataHandle predictions = model.Transform.Apply(training);
using var rows = predictions.GetCursor(["Score", "PredictedLabel"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"margin={rows.Current.GetValue<float>("Score"),8:F3} " +
        $"predicted={rows.Current.GetValue<bool>("PredictedLabel")}");
}

