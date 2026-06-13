#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Fit a connected dense network with a ReLU hidden layer.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

var features = Enumerable.Range(-10, 21)
    .Select(i => new[] { i / 10f })
    .ToArray();
var labels = features.Select(x => MathF.Abs(x[0])).ToArray();

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", features),
    ("Label", labels));

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [
        new DenseLayerSpec(1, 6, ActivationKind.ReLU),
        new DenseLayerSpec(6, 1)
    ]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 400, LearningRate = 0.02f });

IModel model = learner.Fit(new LearnerInput(
    data,
    Environment: new DefaultExecutionEnvironment(seed: 12)));

using var rows = model.Transform.Apply(data).GetCursor(["Features", "Score"]);
while (rows.MoveNext())
{
    var x = rows.Current.GetValue<float[]>("Features")[0];
    if (MathF.Abs(x) is 0f or 0.5f or 1f)
        Console.WriteLine($"abs({x,4:F1}) ~= {rows.Current.GetValue<float>("Score"):F3}");
}

