#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Demonstrate that 0.5.0 trains two outputs but publishes scalar Score only.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f], [3f] }),
    ("Label", new float[][] { [0f, 0f], [1f, 2f], [2f, 4f], [3f, 6f] }));

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [new DenseLayerSpec(1, 2)]);

var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions { Epochs = 120, LearningRate = 0.02f });

IModel model = learner.Fit(new LearnerInput(
    data,
    Environment: new DefaultExecutionEnvironment(seed: 9)));

var parameters = (NeuralNetworkParameters)model.Parameters;
Console.WriteLine($"Final layer parameter outputs: {parameters.Biases[0].Length}");

IDataHandle scored = model.Transform.Apply(data);
var scoreColumn = scored.Schema.Columns.First(column => column.Name == "Score");
Console.WriteLine($"Public Score CLR type: {scoreColumn.Type.ClrType.Name}");
Console.WriteLine("Only output zero is exposed by the 0.5.0 scoring transform.");
