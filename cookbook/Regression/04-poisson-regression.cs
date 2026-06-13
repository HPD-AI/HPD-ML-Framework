#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Fit positive count-like targets with a log-linked Poisson model.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Regression;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0f], [0.25f], [0.5f], [0.75f], [1f],
        [1.25f], [1.5f], [1.75f], [2f]
    }),
    ("Label", new[] { 1f, 1f, 2f, 2f, 2f, 3f, 3f, 4f, 5f }));

IModel model = ILearner.PoissonRegression(
    options: new PoissonRegressionOptions
    {
        L2Regularization = 0.1f,
        MaxIterations = 80
    }).Fit(new LearnerInput(training));

IDataHandle predictionInput = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0.2f], [1f], [1.8f] }));

IDataHandle predictions = model.Transform.Apply(predictionInput);
using var rows = predictions.GetCursor(["Features", "Score"]);
while (rows.MoveNext())
{
    float x = rows.Current.GetValue<float[]>("Features")[0];
    float rate = rows.Current.GetValue<float>("Score");
    Console.WriteLine($"x={x:F1}, predicted rate={rate:F3}, finite={float.IsFinite(rate)}");
}

Console.WriteLine(
    "Published 0.5.0 clamps exp() during training but not during scoring.");

