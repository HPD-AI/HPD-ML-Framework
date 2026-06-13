#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Evaluate zero-based multiclass predictions and score vectors.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle predictions = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 0, 1, 2, 0, 1, 2 }),
    ("PredictedLabel", new[] { 0, 1, 1, 0, 2, 2 }),
    ("Score", new float[][]
    {
        [0.90f, 0.05f, 0.05f],
        [0.10f, 0.80f, 0.10f],
        [0.10f, 0.60f, 0.30f],
        [0.70f, 0.20f, 0.10f],
        [0.10f, 0.35f, 0.55f],
        [0.05f, 0.10f, 0.85f]
    }));

IDataHandle metrics = ITransform.MulticlassMetrics().Apply(predictions);
using var row = metrics.GetCursor(
    ["MicroAccuracy", "MacroAccuracy", "LogLoss", "LogLossReduction"]);
row.MoveNext();

Console.WriteLine(
    $"Micro accuracy: {row.Current.GetValue<double>("MicroAccuracy"):P1}");
Console.WriteLine(
    $"Macro accuracy: {row.Current.GetValue<double>("MacroAccuracy"):P1}");
Console.WriteLine($"Log loss:       {row.Current.GetValue<double>("LogLoss"):F4}");
Console.WriteLine(
    $"Loss reduction: {row.Current.GetValue<double>("LogLossReduction"):P1}");
