#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// This sample saves neural-network parameters, loads them, and rebuilds scoring.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.Serialization.Zip;

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
    layers: [new DenseLayerSpec(1, 1)]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions
    {
        Epochs = 120,
        LearningRate = 0.03f,
        BatchSize = 5
    });

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Cpu());

IModel trained = await learner.FitAsync(
    new LearnerInput(trainingData, Environment: environment));

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0.25f] }));

var before = ReadScore(trained.Transform.Apply(input));

var modelPath = Path.Combine(
    Path.GetTempPath(),
    "hpd-ml-neural-network.zip");

var serializer = new ZipSerializer();
await using (var destination = File.Create(modelPath))
{
    serializer.Save(
        trained,
        SaveContent.LearnedParameters,
        new ZipFormat(),
        destination);
}

IModel loaded;
await using (var source = File.OpenRead(modelPath))
{
    loaded = serializer.Load(new ZipFormat(), source);
}

var parameters = (NeuralNetworkParameters)loaded.Parameters;
IModel restored = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

var after = ReadScore(restored.Transform.Apply(input));

Console.WriteLine($"Saved: {modelPath}");
Console.WriteLine($"Before: {before:F6}");
Console.WriteLine($"After:  {after:F6}");
Console.WriteLine($"Match: {Math.Abs(before - after) < 1e-6f}");

static float ReadScore(IDataHandle predictions)
{
    using var row = predictions.GetCursor(["Score"]);
    row.MoveNext();
    return row.Current.GetValue<float>("Score");
}
