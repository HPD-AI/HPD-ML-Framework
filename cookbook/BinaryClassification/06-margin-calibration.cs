#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// Calibrate an SVM on separate validation rows.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle training = CreateData(0f);
IDataHandle validation = CreateData(0.15f);
IDataHandle test = CreateData(-0.1f);

IModel model = ILearner.LinearSvm(
    options: new LinearSvmOptions
    {
        Lambda = 0.01,
        NumberOfIterations = 20,
        Seed = 42
    }).Fit(new LearnerInput(training, ValidationData: validation));

IDataHandle predictions = model.Transform.Apply(test);
using var rows = predictions.GetCursor(
    ["Score", "Probability", "PredictedLabel"]);

while (rows.MoveNext())
{
    float score = rows.Current.GetValue<float>("Score");
    float probability = rows.Current.GetValue<float>("Probability");
    bool storedLabel = rows.Current.GetValue<bool>("PredictedLabel");
    bool marginLabel = score >= 0f;

    Console.WriteLine(
        $"score={score,8:F3} probability={probability:F3} " +
        $"stored={storedLabel} fromMargin={marginLabel}");
}

Console.WriteLine(
    "The 0.5.0 calibration is reversed here; do not use it for decisions.");

static IDataHandle CreateData(float offset) =>
    InMemoryDataHandle.FromColumns(
        ("Features", new float[][]
        {
            [-2f + offset], [-1.5f + offset], [-1f + offset], [-0.5f + offset],
            [0.5f + offset], [1f + offset], [1.5f + offset], [2f + offset]
        }),
        ("Label", new[] { false, false, false, false, true, true, true, true }));
