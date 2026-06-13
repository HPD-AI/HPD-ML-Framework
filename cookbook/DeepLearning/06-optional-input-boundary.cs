#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Show that ValidationData and InitialModel are ignored in published 0.5.0.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f] }),
    ("Label", new[] { 1f, 3f, 5f }));
IDataHandle validation = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [10f] }),
    ("Label", new[] { -999f }));

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(1, 1)]);
var options = new TrainingOptions { Epochs = 4, LearningRate = 0.01f };
var environment = new DefaultExecutionEnvironment(seed: 44);

var firstLearner = new NeuralNetworkLearner(definition, options);
IModel first = firstLearner.Fit(new LearnerInput(training, Environment: environment));

var secondLearner = new NeuralNetworkLearner(definition, options);
IModel second = secondLearner.Fit(new LearnerInput(
    training,
    ValidationData: validation,
    InitialModel: first,
    Environment: environment));

var a = (NeuralNetworkParameters)first.Parameters;
var b = (NeuralNetworkParameters)second.Parameters;
Console.WriteLine($"Same weights: {a.Weights[0].SequenceEqual(b.Weights[0])}");
Console.WriteLine($"Same biases:  {a.Biases[0].SequenceEqual(b.Biases[0])}");
Console.WriteLine("ValidationData and InitialModel did not alter training.");

