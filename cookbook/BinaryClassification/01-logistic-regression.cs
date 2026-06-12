#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Train logistic regression and evaluate held-out probability predictions.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle training = CreateData(
    [-2.0f, -1.0f, -0.5f, -1.5f, 0.7f, 1.0f, 1.5f, 2.0f],
    [false, false, false, false, true, true, true, true]);

IDataHandle test = CreateData(
    [-1.2f, -0.2f, 0.4f, 1.3f],
    [false, false, true, true]);

ILearner learner = ILearner.LogisticRegression(
    options: new LogisticRegressionOptions
    {
        L2Regularization = 0.1f,
        MaxIterations = 50
    });

IModel model = await learner.FitAsync(new LearnerInput(training));
IDataHandle predictions = model.Transform.Apply(test);

using (var rows = predictions.GetCursor(
    ["Features", "Probability", "PredictedLabel"]))
{
    while (rows.MoveNext())
    {
        float x = rows.Current.GetValue<float[]>("Features")[0];
        float probability = rows.Current.GetValue<float>("Probability");
        bool predicted = rows.Current.GetValue<bool>("PredictedLabel");
        Console.WriteLine(
            $"x={x,5:F1} probability={probability:F3} predicted={predicted}");
    }
}

IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    scoreColumn: "Probability").Apply(predictions);

using var metric = metrics.GetCursor(["Accuracy", "AUC", "LogLoss"]);
metric.MoveNext();
Console.WriteLine($"Accuracy: {metric.Current.GetValue<double>("Accuracy"):P1}");
Console.WriteLine($"AUC: {metric.Current.GetValue<double>("AUC"):F3}");
Console.WriteLine($"Log loss: {metric.Current.GetValue<double>("LogLoss"):F3}");

static IDataHandle CreateData(float[] values, bool[] labels) =>
    InMemoryDataHandle.FromColumns(
        ("Features", values.Select(x => new[] { x }).ToArray()),
        ("Label", labels));

