#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Verify seed determinism and expose managed BatchSize semantics.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [-1f], [0f], [1f], [2f] }),
    ("Label", new[] { -1f, 1f, 3f, 5f }));

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(1, 1)]);

NeuralNetworkParameters Train(int batchSize)
{
    var learner = new NeuralNetworkLearner(
        definition,
        new TrainingOptions
        {
            Epochs = 8,
            LearningRate = 0.01f,
            BatchSize = batchSize
        });
    return (NeuralNetworkParameters)learner.Fit(new LearnerInput(
        data,
        Environment: new DefaultExecutionEnvironment(seed: 123))).Parameters;
}

var batch1 = Train(1);
var batch64 = Train(64);

Console.WriteLine($"Same weights: {batch1.Weights[0].SequenceEqual(batch64.Weights[0])}");
Console.WriteLine($"Same biases:  {batch1.Biases[0].SequenceEqual(batch64.Biases[0])}");
Console.WriteLine("Managed 0.5.0 ignores BatchSize.");

