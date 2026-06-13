#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Calculate regression metrics and expose published 0.5.0 edge behavior.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle scored = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0 }),
    ("Score", new[] { 1.2, 1.8, 3.1, 3.7, 5.2, 5.9 }));

PrintMetrics("Held out rows", ITransform.RegressionMetrics(featureCount: 2)
    .Apply(scored));

IDataHandle constantPerfect = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 5.0, 5.0, 5.0 }),
    ("Score", new[] { 5.0, 5.0, 5.0 }));
PrintMetrics("Perfect constant labels", ITransform.RegressionMetrics()
    .Apply(constantPerfect));

IDataHandle empty = InMemoryDataHandle.FromColumns(
    ("Label", Array.Empty<double>()),
    ("Score", Array.Empty<double>()));
PrintMetrics("Empty input", ITransform.RegressionMetrics().Apply(empty));

static void PrintMetrics(string name, IDataHandle metrics)
{
    using var row = metrics.GetCursor(
        ["MAE", "MSE", "RMSE", "RSquared", "AdjustedRSquared"]);
    row.MoveNext();
    Console.WriteLine($"\n{name}");
    foreach (string column in new[]
    {
        "MAE", "MSE", "RMSE", "RSquared", "AdjustedRSquared"
    })
    {
        Console.WriteLine(
            $"{column,-17} {row.Current.GetValue<double>(column):F4}");
    }
}

Console.WriteLine(
    "\nIn 0.5.0, empty input returns zeros and perfect constant labels have R2=0. " +
    "Direct metric inputs use double columns to avoid the float row-coercion defect.");
