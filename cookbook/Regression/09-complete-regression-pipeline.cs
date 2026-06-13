#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:package HPD-ML-Regression@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Fit text features on training rows, reuse them, train, predict, and evaluate.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;
using HPD.ML.Regression;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Description", new[]
    {
        "small basic apartment",
        "small renovated apartment",
        "large basic house",
        "large renovated house",
        "large luxury house",
        "compact basic studio",
        "compact luxury studio",
        "spacious luxury apartment"
    }),
    ("Price", new[] { 120f, 155f, 240f, 290f, 360f, 95f, 170f, 310f }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Description", new[]
    {
        "small luxury apartment",
        "large renovated apartment",
        "compact basic apartment"
    }),
    ("Price", new[] { 210f, 275f, 115f }));

IModel preparation = ILearner.TextFeaturize(
    columnName: "Description",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        NgramMin = 1,
        NgramMax = 2,
        MaxFeatures = 48
    })
    .Fit(new LearnerInput(training));

IDataHandle preparedTraining = preparation.Transform.Apply(training);
IDataHandle preparedTest = preparation.Transform.Apply(test);
int featureCount =
    ((TextFeaturizeParameters)preparation.Parameters).FeatureIndex.Count;

IModel regressor = ILearner.OrdinaryLeastSquares(
    labelColumn: "Price",
    featureColumn: "Features",
    options: new OlsOptions
    {
        L2Regularization = 0.001f,
        MaxIterations = 120
    }).Fit(new LearnerInput(preparedTraining));

IDataHandle predictions = regressor.Transform.Apply(preparedTest);
using (var rows = predictions.GetCursor(["Description", "Price", "Score"]))
{
    while (rows.MoveNext())
    {
        Console.WriteLine(
            $"{rows.Current.GetValue<string>("Description"),28}: " +
            $"actual={rows.Current.GetValue<float>("Price"),6:F1}, " +
            $"predicted={rows.Current.GetValue<float>("Score"),7:F2}");
    }
}

IDataHandle metrics = ITransform.RegressionMetrics(
    labelColumn: "Price",
    featureCount: featureCount)
    .Apply(predictions);

using var metric = metrics.GetCursor(["RMSE", "RSquared"]);
metric.MoveNext();
Console.WriteLine($"RMSE: {metric.Current.GetValue<double>("RMSE"):F3}");
Console.WriteLine($"R-squared: {metric.Current.GetValue<double>("RSquared"):F3}");
