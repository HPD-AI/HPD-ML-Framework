#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Train the portable managed dense-network baseline.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-2f], [-1f], [0f], [1f], [2f], [3f] }),
    ("Label", new[] { -3f, -1f, 1f, 3f, 5f, 7f }));

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [new DenseLayerSpec(1, 1)]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 160, LearningRate = 0.03f });

var environment = new DefaultExecutionEnvironment(
    seed: 7,
    backend: BackendSpec.Cpu());

IModel model = learner.Fit(new LearnerInput(training, Environment: environment));
IDataHandle predictions = model.Transform.Apply(training);

using var rows = predictions.GetCursor(["Features", "Label", "Score"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"x={rows.Current.GetValue<float[]>("Features")[0],4:F1} " +
        $"actual={rows.Current.GetValue<float>("Label"),5:F1} " +
        $"score={rows.Current.GetValue<float>("Score"),6:F2}");
}

