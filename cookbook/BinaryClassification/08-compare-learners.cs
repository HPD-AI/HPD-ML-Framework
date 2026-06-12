#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Compare all four learners on the same held-out rows.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2f, -1f], [-1.8f, -0.4f], [-1.2f, -1.1f], [-0.7f, -0.6f],
        [0.7f, 0.6f], [1.1f, 1.2f], [1.7f, 0.5f], [2f, 1.3f]
    }),
    ("Label", new[] { false, false, false, false, true, true, true, true }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-1.5f, -0.8f], [-0.4f, -0.7f], [0.5f, 0.8f], [1.4f, 0.9f]
    }),
    ("Label", new[] { false, false, true, true }));

(string Name, ILearner Learner)[] learners =
[
    ("Logistic", ILearner.LogisticRegression(
        options: new LogisticRegressionOptions
        {
            L2Regularization = 0.1f,
            MaxIterations = 50
        })),
    ("SDCA", ILearner.Sdca(
        options: new SdcaOptions
        {
            L2Regularization = 1.0,
            NumberOfIterations = 20,
            ConvergenceTolerance = 0,
            Seed = 42
        })),
    ("Perceptron", ILearner.AveragedPerceptron(
        options: new AveragedPerceptronOptions { NumberOfIterations = 20 })),
    ("Linear SVM", ILearner.LinearSvm(
        options: new LinearSvmOptions
        {
            Lambda = 0.01,
            NumberOfIterations = 20,
            Seed = 42
        }))
];

foreach (var (name, learner) in learners)
{
    IModel model = learner.Fit(new LearnerInput(training));
    IDataHandle predictions = model.Transform.Apply(test);
    IDataHandle metrics = ITransform.BinaryClassificationMetrics(
        scoreColumn: "Probability").Apply(predictions);

    using var row = metrics.GetCursor(["Accuracy", "AUC"]);
    row.MoveNext();
    Console.WriteLine(
        $"{name,-12} accuracy={row.Current.GetValue<double>("Accuracy"):P1} " +
        $"auc={row.Current.GetValue<double>("AUC"):F3}");
}

Console.WriteLine(
    "\nSDCA's reversed result is a known published 0.5.0 correctness defect.");
