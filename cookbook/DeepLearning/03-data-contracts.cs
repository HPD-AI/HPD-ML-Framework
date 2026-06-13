#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Show vector features, custom widths, and scalar-label conversion.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Signals", new float[][]
    {
        [0f, 0f], [0f, 1f], [1f, 0f], [1f, 1f]
    }),
    ("Target", new[] { 0, 1, 1, 2 }));

var definition = new NeuralNetworkDefinition(
    "Signals",
    "Target",
    [new DenseLayerSpec(2, 1)]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 250, LearningRate = 0.04f });

IModel model = learner.Fit(new LearnerInput(
    data,
    Environment: new DefaultExecutionEnvironment(seed: 3)));

using var rows = model.Transform.Apply(data).GetCursor(["Signals", "Target", "Score"]);
while (rows.MoveNext())
{
    var x = rows.Current.GetValue<float[]>("Signals");
    Console.WriteLine(
        $"[{x[0]:F0}, {x[1]:F0}] target={rows.Current.GetValue<int>("Target")} " +
        $"score={rows.Current.GetValue<float>("Score"):F2}");
}

