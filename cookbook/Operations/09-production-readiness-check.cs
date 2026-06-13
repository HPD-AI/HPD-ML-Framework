#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Run a compact deterministic schema, training, and prediction readiness check.

using System.Reflection;
using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f], [-1f], [-0.5f], [0.5f], [1f], [2f]
    }),
    ("Label", new[] { false, false, false, true, true, true }));

Console.WriteLine($"rows={data.RowCount}, ordering={data.Ordering}");
Console.WriteLine(
    $"core version={typeof(InMemoryDataHandle).Assembly.GetName().Version}");

ILearner learner = ILearner.LogisticRegression(
    options: new LogisticRegressionOptions { MaxIterations = 20 });
IModel model = learner.Fit(new LearnerInput(data));

ISchema output = model.Transform.GetOutputSchema(data.Schema);
foreach (string required in new[] { "Score", "Probability", "PredictedLabel" })
{
    if (!output.Columns.Any(column => column.Name == required))
        throw new InvalidOperationException($"Missing output column '{required}'.");
}

using var cursor = model.Transform.Apply(data)
    .GetCursor(["Score", "Probability", "PredictedLabel"]);
var count = 0;
while (cursor.MoveNext())
{
    float score = cursor.Current.GetValue<float>("Score");
    float probability = cursor.Current.GetValue<float>("Probability");
    if (!float.IsFinite(score) || !float.IsFinite(probability))
        throw new InvalidOperationException("Non-finite prediction.");
    count++;
}

if (count != data.RowCount)
    throw new InvalidOperationException("Prediction row count changed.");

Console.WriteLine($"validated predictions={count}");
Console.WriteLine("Production readiness check passed for this managed path.");

// Guide: docs/operations/production-checklist-and-troubleshooting.md

