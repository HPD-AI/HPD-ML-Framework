#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// This sample trains on labeled text and predicts unlabeled text.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
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
    "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        NgramMin = 1,
        NgramMax = 1,
        MaxFeatures = 32
    });

IModel featureModel = await featurizer.FitAsync(new LearnerInput(trainingData));
IDataHandle preparedTraining = featureModel.Transform.Apply(trainingData);

ILearner classifier = ILearner.LogisticRegression();
IModel classifierModel = await classifier.FitAsync(
    new LearnerInput(preparedTraining));

IDataHandle newData = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "helpful reliable service",
        "slow broken product"
    }));

IDataHandle preparedNewData = featureModel.Transform.Apply(newData);
IDataHandle predictions = classifierModel.Transform.Apply(preparedNewData);

using var rows = predictions.GetCursor(
    ["Text", "Score", "Probability", "PredictedLabel"]);

while (rows.MoveNext())
{
    Console.WriteLine(rows.Current.GetValue<string>("Text"));
    Console.WriteLine($"  Score: {rows.Current.GetValue<float>("Score"):F3}");
    Console.WriteLine($"  Probability: {rows.Current.GetValue<float>("Probability"):P1}");
    Console.WriteLine($"  Predicted label: {rows.Current.GetValue<bool>("PredictedLabel")}");
}
