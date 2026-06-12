#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// This sample learns text features, applies them, and trains a classifier.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle rawData = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "excellent fast service",
        "friendly helpful support",
        "wonderful reliable product",
        "terrible slow service",
        "rude unhelpful support",
        "broken unreliable product"
    }),
    ("Label", new[] { true, true, true, false, false, false }));

ILearner featurizer = ILearner.TextFeaturize(
    columnName: "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        NgramMin = 1,
        NgramMax = 1,
        MaxFeatures = 32
    });

IModel featureModel = await featurizer.FitAsync(new LearnerInput(rawData));
IDataHandle preparedData = featureModel.Transform.Apply(rawData);

ILearner classifier = ILearner.LogisticRegression();
IModel classifierModel = await classifier.FitAsync(new LearnerInput(preparedData));
IDataHandle predictions = classifierModel.Transform.Apply(preparedData);

Console.WriteLine("Prepared columns:");
foreach (var column in preparedData.Schema.Columns)
    Console.WriteLine($"- {column.Name}");

using var rows = predictions.GetCursor(["Text", "Probability", "PredictedLabel"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Text")} -> " +
        $"{rows.Current.GetValue<float>("Probability"):F3}, " +
        $"{rows.Current.GetValue<bool>("PredictedLabel")}");
}
