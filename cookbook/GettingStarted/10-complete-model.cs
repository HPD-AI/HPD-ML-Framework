#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// This complete sample trains a model and evaluates it on separate test rows.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

var trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2.0f, -1.0f],
        [-1.8f, -0.7f],
        [-1.5f, -0.5f],
        [-1.0f, -1.5f],
        [1.0f, 1.0f],
        [1.5f, 0.5f],
        [1.8f, 0.8f],
        [2.0f, 1.5f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

var testData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-1.7f, -0.9f],
        [-0.8f, -1.1f],
        [0.9f, 1.2f],
        [1.7f, 0.9f]
    }),
    ("Label", new[] { false, false, true, true }));

var learner = ILearner.LogisticRegression();
var model = await learner.FitAsync(new LearnerInput(trainingData));
var predictions = model.Transform.Apply(testData);

using (var rows = predictions.GetCursor(["Probability", "PredictedLabel"]))
{
    while (rows.MoveNext())
    {
        var probability = rows.Current.GetValue<float>("Probability");
        var predicted = rows.Current.GetValue<bool>("PredictedLabel");
        Console.WriteLine($"Probability: {probability:F3}, predicted: {predicted}");
    }
}

var metrics = ITransform.BinaryClassificationMetrics().Apply(predictions);
using var metricRow = metrics.GetCursor(["Accuracy", "AUC"]);
metricRow.MoveNext();

Console.WriteLine($"Accuracy: {metricRow.Current.GetValue<double>("Accuracy"):P0}");
Console.WriteLine($"AUC: {metricRow.Current.GetValue<double>("AUC"):F2}");
