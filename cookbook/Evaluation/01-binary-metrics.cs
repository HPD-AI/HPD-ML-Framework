#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Compare probability-based binary metrics with raw-margin behavior.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle probabilities = InMemoryDataHandle.FromColumns(
    ("Label", new[] { true, true, false, false }),
    ("Probability", new[] { 0.90, 0.70, 0.20, 0.10 }));

IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    scoreColumn: "Probability").Apply(probabilities);

Print(metrics, "Probability input");

IDataHandle margins = InMemoryDataHandle.FromColumns(
    ("Label", new[] { true, true, false, false }),
    ("Score", new[] { 2.2, 0.8, -0.7, -2.0 }));

Print(ITransform.BinaryClassificationMetrics().Apply(margins),
    "Raw margin input (log loss is not meaningful)");

static void Print(IDataHandle metrics, string title)
{
    using var row = metrics.GetCursor(
        ["Accuracy", "AUC", "F1Score", "LogLoss"]);
    row.MoveNext();
    Console.WriteLine($"\n{title}");
    Console.WriteLine($"Accuracy: {row.Current.GetValue<double>("Accuracy"):P1}");
    Console.WriteLine($"AUC:      {row.Current.GetValue<double>("AUC"):F3}");
    Console.WriteLine($"F1:       {row.Current.GetValue<double>("F1Score"):F3}");
    Console.WriteLine($"Log loss: {row.Current.GetValue<double>("LogLoss"):F3}");
}
