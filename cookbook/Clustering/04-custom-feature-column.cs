#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Train and score through a non-default feature column.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle customers = InMemoryDataHandle.FromColumns(
    ("Customer", new[] { "Ada", "Ben", "Cleo", "Dax", "Eli", "Faye" }),
    ("Signals", new float[][]
    {
        [1.0f, 1.2f], [1.1f, 0.9f], [0.8f, 1.1f],
        [8.8f, 9.1f], [9.2f, 8.9f], [9.0f, 9.3f]
    }));

ILearner learner = ILearner.KMeans(
    featureColumn: "Signals",
    options: new KMeansOptions
    {
        NumberOfClusters = 2,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    });

IDataHandle predictions =
    learner.Fit(new LearnerInput(customers)).Transform.Apply(customers);

using var rows = predictions.GetCursor(["Customer", "PredictedLabel"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Customer")}: " +
        $"cluster {rows.Current.GetValue<uint>("PredictedLabel")}");
}
