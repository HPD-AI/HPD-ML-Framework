#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Show that Score contains squared distances in cluster-label order.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0f, 0f], [0.2f, 0.1f], [-0.2f, -0.1f],
        [10f, 10f], [10.2f, 10.1f], [9.8f, 9.9f]
    }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Name", new[] { "near origin", "middle", "near ten" }),
    ("Features", new float[][]
    {
        [0.1f, 0.1f], [5f, 5f], [9.9f, 10.1f]
    }));

IModel model = ILearner.KMeans(
    options: new KMeansOptions
    {
        NumberOfClusters = 2,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    }).Fit(new LearnerInput(training));

IDataHandle predictions = model.Transform.Apply(test);
using var rows = predictions.GetCursor(["Name", "PredictedLabel", "Score"]);
while (rows.MoveNext())
{
    uint label = rows.Current.GetValue<uint>("PredictedLabel");
    float[] score = rows.Current.GetValue<float[]>("Score");
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Name"),12}: label={label}, " +
        $"squared distances=[{string.Join(", ", score.Select(x => x.ToString("F3")))}], " +
        $"minimum={score.Min():F3}");
}
