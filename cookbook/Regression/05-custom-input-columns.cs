#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Train with application-specific feature and label column names.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Regression;

IDataHandle sales = InMemoryDataHandle.FromColumns(
    ("Signals", new float[][]
    {
        [800f, 1f], [1000f, 2f], [1200f, 2f],
        [1400f, 3f], [1600f, 3f], [1800f, 4f]
    }),
    ("Price", new[] { 160f, 205f, 235f, 290f, 320f, 375f }));

ILearner learner = ILearner.OrdinaryLeastSquares(
    labelColumn: "Price",
    featureColumn: "Signals",
    options: new OlsOptions
    {
        L2Regularization = 0.001f,
        MaxIterations = 120
    });

IModel model = learner.Fit(new LearnerInput(sales));
IDataHandle predictions = model.Transform.Apply(sales);

using var rows = predictions.GetCursor(["Price", "Score"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"price={rows.Current.GetValue<float>("Price"),6:F1}, " +
        $"prediction={rows.Current.GetValue<float>("Score"),7:F2}");
}

