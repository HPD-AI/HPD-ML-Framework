#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Learn TF-IDF features, train a classifier, and evaluate unseen text.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "fast helpful reliable service",
        "excellent quality and support",
        "works perfectly and feels solid",
        "friendly team solved the problem",
        "slow broken unreliable product",
        "terrible quality and poor support",
        "failed immediately and feels cheap",
        "rude team ignored the problem"
    }),
    ("Label", new[] { true, true, true, true, false, false, false, false }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "helpful support and excellent quality",
        "broken product with terrible service",
        "friendly reliable team",
        "cheap unreliable failure"
    }),
    ("Label", new[] { true, false, true, false }));

IModel featurizer = ILearner.TextFeaturize(
    columnName: "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        NgramMin = 1,
        NgramMax = 2,
        MaxFeatures = 64
    }).Fit(new LearnerInput(training));

IDataHandle preparedTraining = featurizer.Transform.Apply(training);
IDataHandle preparedTest = featurizer.Transform.Apply(test);

IModel classifier = ILearner.LogisticRegression(
    options: new LogisticRegressionOptions
    {
        L2Regularization = 0.1f,
        MaxIterations = 80
    }).Fit(new LearnerInput(preparedTraining));

IDataHandle predictions = classifier.Transform.Apply(preparedTest);
using (var rows = predictions.GetCursor(
    ["Text", "Probability", "PredictedLabel"]))
{
    while (rows.MoveNext())
    {
        Console.WriteLine(
            $"{rows.Current.GetValue<float>("Probability"):P1} " +
            $"{rows.Current.GetValue<bool>("PredictedLabel"),5}  " +
            rows.Current.GetValue<string>("Text"));
    }
}

IDataHandle metrics = ITransform.BinaryClassificationMetrics(
    scoreColumn: "Probability").Apply(predictions);
using var metric = metrics.GetCursor(["Accuracy", "AUC"]);
metric.MoveNext();
Console.WriteLine($"Accuracy: {metric.Current.GetValue<double>("Accuracy"):P1}");
Console.WriteLine($"AUC: {metric.Current.GetValue<double>("AUC"):F3}");

