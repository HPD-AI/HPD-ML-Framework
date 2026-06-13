#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Run deterministic random-fold binary cross-validation.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

float[] values = Enumerable.Range(-20, 41).Select(i => i / 10f).ToArray();
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", values.Select(x => new[] { x }).ToArray()),
    ("Label", values.Select(x => x >= 0).ToArray()));

CrossValidationResult result = await CrossValidator.EvaluateAsync(
    data,
    ILearner.LogisticRegression(
        options: new LogisticRegressionOptions
        {
            L2Regularization = 0.1f,
            MaxIterations = 40
        }),
    ITransform.BinaryClassificationMetrics(scoreColumn: "Probability"),
    folds: 5,
    seed: 42);

Console.WriteLine($"Fold models: {result.Folds.Count}");
using var rows = result.AggregateMetrics.GetCursor(["Metric", "Mean", "StdDev"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Metric"),-22} " +
        $"mean={rows.Current.GetValue<double>("Mean"),8:F4} " +
        $"std={rows.Current.GetValue<double>("StdDev"),8:F4}");
}

Console.WriteLine("\nRefit the selected configuration on all training rows.");
