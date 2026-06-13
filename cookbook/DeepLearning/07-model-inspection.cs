#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Inspect learned dense-layer weights and biases.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-1f], [0f], [1f], [2f] }),
    ("Label", new[] { -1f, 1f, 3f, 5f }));

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [
        new DenseLayerSpec(1, 3, ActivationKind.ReLU),
        new DenseLayerSpec(3, 1)
    ]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 100, LearningRate = 0.02f });

var parameters = (NeuralNetworkParameters)learner.Fit(new LearnerInput(
    data,
    Environment: new DefaultExecutionEnvironment(seed: 5))).Parameters;

for (var i = 0; i < definition.Layers.Count; i++)
{
    var layer = definition.Layers[i];
    Console.WriteLine(
        $"Layer {i}: {layer.InputSize} -> {layer.OutputSize}, " +
        $"activation={layer.Activation}, weights={parameters.Weights[i].Length}, " +
        $"biases={parameters.Biases[i].Length}");
}

Console.WriteLine("Treat exposed parameter arrays as read-only.");

