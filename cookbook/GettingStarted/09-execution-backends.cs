#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// The managed CPU backend is portable and needs no native runtime.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-1.0f],
        [-0.5f],
        [0.0f],
        [0.5f],
        [1.0f]
    }),
    ("Label", new[] { -1.0f, 0.0f, 1.0f, 2.0f, 3.0f }));

var definition = new NeuralNetworkDefinition(
    featureColumn: "Features",
    labelColumn: "Label",
    layers:
    [
        new DenseLayerSpec(1, 4, ActivationKind.ReLU),
        new DenseLayerSpec(4, 1)
    ]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions
    {
        Epochs = 160,
        LearningRate = 0.03f,
        BatchSize = 5
    });

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Cpu());

IModel model = await learner.FitAsync(
    new LearnerInput(trainingData, Environment: environment));

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0.25f] }));

IDataHandle predictions = model.Transform.Apply(input);

using var cursor = predictions.GetCursor(["Score"]);
cursor.MoveNext();

Console.WriteLine($"Backend: {environment.Backend}");
Console.WriteLine($"Input: 0.25");
Console.WriteLine($"Score: {cursor.Current.GetValue<float>("Score"):F4}");

