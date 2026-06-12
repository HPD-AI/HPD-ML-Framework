#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// This sample trains on one dataset and evaluates a held-out test dataset.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2.0f, -1.0f],
        [-1.8f, -0.7f],
        [-1.5f, -1.2f],
        [-1.0f, -0.8f],
        [1.0f, 0.8f],
        [1.4f, 1.1f],
        [1.8f, 0.7f],
        [2.0f, 1.3f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

IDataHandle testData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-1.7f, -0.9f],
        [-0.8f, -1.1f],
        [0.9f, 1.2f],
        [1.7f, 0.9f]
    }),
    ("Label", new[] { false, false, true, true }));

ILearner learner = ILearner.LogisticRegression();
IModel model = await learner.FitAsync(new LearnerInput(trainingData));
IDataHandle predictions = model.Transform.Apply(testData);

ITransform evaluator = ITransform.BinaryClassificationMetrics(
    scoreColumn: "Probability");
IDataHandle metrics = evaluator.Apply(predictions);

using (var row = metrics.GetCursor(
    ["Accuracy", "AUC", "F1Score", "LogLoss"]))
{
    row.MoveNext();
    Console.WriteLine($"Accuracy: {row.Current.GetValue<double>("Accuracy"):P1}");
    Console.WriteLine($"AUC: {row.Current.GetValue<double>("AUC"):F3}");
    Console.WriteLine($"F1: {row.Current.GetValue<double>("F1Score"):F3}");
    Console.WriteLine($"Log loss: {row.Current.GetValue<double>("LogLoss"):F3}");
}

IDataHandle confusion = ITransform.ConfusionMatrix().Apply(predictions);
Console.WriteLine();
Console.WriteLine("Confusion matrix:");
Console.WriteLine(ConfusionMatrixFormatter.Format(confusion));
