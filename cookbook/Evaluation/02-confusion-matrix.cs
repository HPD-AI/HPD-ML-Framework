#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Build sparse confusion counts and format them as a readable grid.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle predictions = InMemoryDataHandle.FromColumns(
    ("Label", new[] { "cat", "cat", "dog", "dog", "bird", "bird" }),
    ("PredictedLabel", new[] { "cat", "dog", "dog", "dog", "bird", "cat" }));

IDataHandle confusion = ITransform.ConfusionMatrix().Apply(predictions);

Console.WriteLine("Observed pairs:");
using (var rows = confusion.GetCursor(
    ["TrueLabel", "PredictedLabel", "Count"]))
{
    while (rows.MoveNext())
    {
        Console.WriteLine(
            $"{rows.Current.GetValue<string>("TrueLabel"),5} -> " +
            $"{rows.Current.GetValue<string>("PredictedLabel"),5}: " +
            rows.Current.GetValue<int>("Count"));
    }
}

Console.WriteLine("\nFormatted matrix:");
Console.WriteLine(ConfusionMatrixFormatter.Format(confusion));
