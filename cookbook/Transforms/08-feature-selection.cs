#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Select the numeric feature with the strongest mutual information.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

float[] label = [0, 0, 0, 1, 1, 1];

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 10, 11, 12, 13, 14, 15 }),
    ("Label", label),
    ("Correlated", label.ToArray()),
    ("Constant", new[] { 5f, 5f, 5f, 5f, 5f, 5f }));

ILearner learner = ILearner.MutualInfoFeatureSelection(
    labelColumn: "Label",
    featureColumns: ["Correlated", "Constant"],
    topK: 1,
    numBins: 3);

IModel model = learner.Fit(new LearnerInput(training));
var parameters = (MutualInfoParameters)model.Parameters;

foreach (var score in parameters.FeatureScores)
    Console.WriteLine($"Selected {score.Key}, MI={score.Value:F4}");

IDataHandle selected = model.Transform.Apply(training);

Console.WriteLine("Output columns:");
foreach (var column in selected.Schema.Columns)
    Console.WriteLine($"- {column.Name}");
