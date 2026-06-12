#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:property TargetFramework=net10.0

// This sample shows the core HPD ML lifecycle without extra pipeline steps.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2.0f, -1.0f],
        [-1.0f, -0.5f],
        [1.0f, 0.5f],
        [2.0f, 1.0f]
    }),
    ("Label", new[] { false, false, true, true }));

ILearner learner = ILearner.LogisticRegression();
IModel model = await learner.FitAsync(new LearnerInput(trainingData));
IDataHandle predictions = model.Transform.Apply(trainingData);

Console.WriteLine($"Model parameters: {model.Parameters.GetType().Name}");
Console.WriteLine("Prediction columns:");

foreach (var column in predictions.Schema.Columns)
    Console.WriteLine($"- {column.Name}");
