#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Compare the three squared-loss learners on the same held-out rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;
using HPD.ML.Regression;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f, 1f], [-1.5f, -1f], [-1f, 2f], [0f, 0f],
        [1f, -2f], [2f, 1f], [3f, -1f], [4f, 2f]
    }),
    ("Label", new[] { -6f, -1f, -4f, 2f, 7f, 7f, 12f, 12f }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-0.5f, 1f], [0.5f, -1f], [2.5f, 0f], [3.5f, 1f]
    }),
    ("Label", new[] { -0.5f, 4.5f, 9.5f, 11.5f }));

(string Name, ILearner Learner)[] learners =
[
    ("OLS", ILearner.OrdinaryLeastSquares(
        options: new OlsOptions
        {
            L2Regularization = 0.001f,
            MaxIterations = 100
        })),
    ("SDCA", ILearner.SdcaRegression(
        options: new SdcaRegressionOptions
        {
            L2Regularization = 0.1,
            NumberOfIterations = 80,
            ConvergenceTolerance = 0,
            Seed = 42
        })),
    ("Online GD", ILearner.OnlineGradientDescent(
        options: new OnlineGradientDescentOptions
        {
            LearningRate = 0.01,
            NumberOfIterations = 80,
            AverageWeights = true
        }))
];

foreach (var (name, learner) in learners)
{
    IModel model = learner.Fit(new LearnerInput(training));
    IDataHandle predictions = model.Transform.Apply(test);
    IDataHandle metrics = ITransform.RegressionMetrics(featureCount: 2)
        .Apply(predictions);

    using var row = metrics.GetCursor(["RMSE", "RSquared", "AdjustedRSquared"]);
    row.MoveNext();
    Console.WriteLine(
        $"{name,-10} rmse={row.Current.GetValue<double>("RMSE"),7:F3} " +
        $"r2={row.Current.GetValue<double>("RSquared"),7:F3} " +
        $"adjusted={row.Current.GetValue<double>("AdjustedRSquared"),7:F3}");
}

