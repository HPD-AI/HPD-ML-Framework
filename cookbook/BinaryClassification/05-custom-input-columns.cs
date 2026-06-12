#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Train and score with application-specific feature and label column names.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("TransactionId", new[] { "t1", "t2", "t3", "t4", "t5", "t6" }),
    ("Signals", new float[][]
    {
        [0.1f, 0.2f], [0.2f, 0.1f], [0.3f, 0.4f],
        [1.5f, 1.2f], [1.8f, 1.4f], [2.0f, 1.7f]
    }),
    ("IsFraud", new[] { false, false, false, true, true, true }));

ILearner learner = ILearner.LogisticRegression(
    labelColumn: "IsFraud",
    featureColumn: "Signals",
    options: new LogisticRegressionOptions
    {
        L2Regularization = 0.1f,
        MaxIterations = 50
    });

IModel model = learner.Fit(new LearnerInput(training));
IDataHandle predictions = model.Transform.Apply(training);

using var rows = predictions.GetCursor(
    ["TransactionId", "IsFraud", "Probability", "PredictedLabel"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("TransactionId")}: " +
        $"actual={rows.Current.GetValue<bool>("IsFraud")}, " +
        $"probability={rows.Current.GetValue<float>("Probability"):F3}, " +
        $"predicted={rows.Current.GetValue<bool>("PredictedLabel")}");
}

