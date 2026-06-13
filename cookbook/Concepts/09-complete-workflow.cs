#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Run the complete schema -> learner -> model -> predictions -> metrics flow.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Evaluation;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-3f, -1f],
        [-2f, -1f],
        [-1f, -0.5f],
        [1f, 0.5f],
        [2f, 1f],
        [3f, 1f]
    }),
    ("Label", new[] { false, false, false, true, true, true }));

var environment = new DefaultExecutionEnvironment(seed: 42);
ILearner learner = ILearner.LogisticRegression();

IModel model = await learner.FitAsync(
    new LearnerInput(TrainData: data, Environment: environment));

IDataHandle predictions = model.Transform.Apply(data);
IDataHandle metrics = ITransform.BinaryClassificationMetrics().Apply(predictions);

Console.WriteLine($"input rows: {data.RowCount}");
Console.WriteLine($"prediction ordering: {predictions.Ordering}");
Console.WriteLine($"model parameters: {model.Parameters.GetType().Name}");

using var metric = metrics.GetCursor(metrics.Schema.Columns.Select(c => c.Name));
metric.MoveNext();
Console.WriteLine($"accuracy: {metric.Current.GetValue<double>("Accuracy"):F3}");
Console.WriteLine($"AUC: {metric.Current.GetValue<double>("AUC"):F3}");

// Guide: docs/concepts/architecture-and-packages.md
