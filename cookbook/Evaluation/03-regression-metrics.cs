#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Calculate regression metrics and expose constant-target compatibility behavior.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle scored = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 1.0, 2.0, 3.0, 4.0, 5.0 }),
    ("Score", new[] { 1.2, 1.8, 3.1, 3.7, 5.2 }));

Print("Ordinary held-out rows",
    ITransform.RegressionMetrics(featureCount: 2).Apply(scored));

IDataHandle constant = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 5.0, 5.0, 5.0 }),
    ("Score", new[] { 5.0, 5.0, 5.0 }));

Print("Perfect constant prediction (0.5.0 reports R2=0)",
    ITransform.RegressionMetrics().Apply(constant));

static void Print(string title, IDataHandle metrics)
{
    using var row = metrics.GetCursor(
        ["MAE", "RMSE", "RSquared", "AdjustedRSquared"]);
    row.MoveNext();
    Console.WriteLine($"\n{title}");
    Console.WriteLine($"MAE:         {row.Current.GetValue<double>("MAE"):F4}");
    Console.WriteLine($"RMSE:        {row.Current.GetValue<double>("RMSE"):F4}");
    Console.WriteLine($"R-squared:   {row.Current.GetValue<double>("RSquared"):F4}");
    Console.WriteLine(
        $"Adjusted R2: {row.Current.GetValue<double>("AdjustedRSquared"):F4}");
}
