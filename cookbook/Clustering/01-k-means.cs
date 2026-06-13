#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Fit batch K-means and print assignments for three separated groups.

using HPD.ML.Abstractions;
using HPD.ML.Clustering;
using HPD.ML.Core;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Name", new[] { "A1", "A2", "A3", "B1", "B2", "B3", "C1", "C2", "C3" }),
    ("Features", new float[][]
    {
        [0.0f, 0.2f], [0.3f, -0.1f], [-0.2f, 0.1f],
        [9.8f, 10.1f], [10.2f, 9.9f], [10.0f, 10.3f],
        [20.0f, 0.2f], [19.7f, -0.1f], [20.3f, 0.0f]
    }));

ILearner learner = ILearner.KMeans(
    options: new KMeansOptions
    {
        NumberOfClusters = 3,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        MaxIterations = 100,
        Seed = 42
    });

IModel model = learner.Fit(new LearnerInput(training));
IDataHandle predictions = model.Transform.Apply(training);

using var rows = predictions.GetCursor(["Name", "PredictedLabel", "Score"]);
while (rows.MoveNext())
{
    uint label = rows.Current.GetValue<uint>("PredictedLabel");
    float[] scores = rows.Current.GetValue<float[]>("Score");
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Name")}: cluster={label}, " +
        $"squared-distance={scores[(int)label - 1]:F4}");
}
