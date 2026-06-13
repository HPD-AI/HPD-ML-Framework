#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Expose zero-filled empty results and explain metric optimization direction.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle emptyRegression = InMemoryDataHandle.FromColumns(
    ("Label", Array.Empty<double>()),
    ("Score", Array.Empty<double>()));

IDataHandle emptyMetrics =
    ITransform.RegressionMetrics().Apply(emptyRegression);
using (var row = emptyMetrics.GetCursor(["MAE", "RMSE", "RSquared"]))
{
    row.MoveNext();
    Console.WriteLine("Published 0.5.0 empty regression result:");
    Console.WriteLine($"MAE={row.Current.GetValue<double>("MAE")}");
    Console.WriteLine($"RMSE={row.Current.GetValue<double>("RMSE")}");
    Console.WriteLine($"R2={row.Current.GetValue<double>("RSquared")}");
}

IDataHandle noVectors = InMemoryDataHandle.FromColumns(
    ("Label", new[] { 0, 1 }),
    ("PredictedLabel", new[] { 0, 1 }),
    ("Score", new[] { 0, 0 }));

IDataHandle multiclass = ITransform.MulticlassMetrics().Apply(noVectors);
using (var row = multiclass.GetCursor(["MicroAccuracy", "LogLoss"]))
{
    row.MoveNext();
    Console.WriteLine("\nUnavailable multiclass score vectors:");
    Console.WriteLine(
        $"Accuracy={row.Current.GetValue<double>("MicroAccuracy"):P0}");
    Console.WriteLine(
        $"LogLoss={row.Current.GetValue<double>("LogLoss")} (unavailable, not perfect)");
}

Console.WriteLine("\nDirection:");
Console.WriteLine("maximize: Accuracy, AUC, F1, R2, NDCG, NMI");
Console.WriteLine("minimize: LogLoss, MAE, MSE, RMSE, distance, DBI");
