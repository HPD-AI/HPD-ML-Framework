#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Fit a learner, inspect its model, and consume lazy predictions.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f],
        [-1f],
        [1f],
        [2f]
    }),
    ("Label", new[] { false, false, true, true }));

ILearner learner = ILearner.LogisticRegression();
Console.WriteLine(
    $"declared output: {string.Join(", ", learner.GetOutputSchema(trainingData.Schema).Columns.Select(c => c.Name))}");

IModel model = await learner.FitAsync(new LearnerInput(trainingData));
Console.WriteLine($"transform: {model.Transform.GetType().Name}");
Console.WriteLine($"parameters: {model.Parameters.GetType().Name}");

IDataHandle predictions = model.Transform.Apply(trainingData);
using var rows = predictions.GetCursor(["Score", "Probability", "PredictedLabel"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"score={rows.Current.GetValue<float>("Score"):F3}, " +
        $"p={rows.Current.GetValue<float>("Probability"):F3}, " +
        $"label={rows.Current.GetValue<bool>("PredictedLabel")}");
}

// Guide: docs/concepts/learners-models-and-parameters.md
