#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Fit an L-BFGS least-squares model and evaluate held-out rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;
using HPD.ML.Regression;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-2f], [-1f], [0f], [1f], [2f], [3f] }),
    ("Label", new[] { -3f, -1f, 1f, 3f, 5f, 7f }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-1.5f], [0.5f], [2.5f] }),
    ("Label", new[] { -2f, 2f, 6f }));

ILearner learner = ILearner.OrdinaryLeastSquares(
    options: new OlsOptions
    {
        L2Regularization = 0.001f,
        MaxIterations = 100
    });

IModel model = learner.Fit(new LearnerInput(training));
IDataHandle predictions = model.Transform.Apply(test);

using (var rows = predictions.GetCursor(["Features", "Label", "Score"]))
{
    while (rows.MoveNext())
    {
        Console.WriteLine(
            $"x={rows.Current.GetValue<float[]>("Features")[0],5:F1} " +
            $"actual={rows.Current.GetValue<float>("Label"),5:F1} " +
            $"predicted={rows.Current.GetValue<float>("Score"),6:F2}");
    }
}

IDataHandle metrics = ITransform.RegressionMetrics().Apply(predictions);
using var metric = metrics.GetCursor(["RMSE", "RSquared"]);
metric.MoveNext();
Console.WriteLine($"RMSE: {metric.Current.GetValue<double>("RMSE"):F4}");
Console.WriteLine($"R-squared: {metric.Current.GetValue<double>("RSquared"):F4}");

