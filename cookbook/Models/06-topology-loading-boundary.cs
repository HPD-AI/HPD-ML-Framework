#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Show that saved topology loads as identity in published 0.5.0.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.Serialization.Zip;

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [new DenseLayerSpec(1, 1)]);
var parameters = new NeuralNetworkParameters(
    definition,
    [new[] { 2f }],
    [new[] { 1f }]);
IModel original = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

using var stream = new MemoryStream();
var serializer = new ZipSerializer();
serializer.Save(original, SaveContent.All, new ZipFormat(), stream);

stream.Position = 0;
IModel loaded = serializer.Load(new ZipFormat(), stream);

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [3f] }));
ISchema loadedSchema = loaded.Transform.GetOutputSchema(input.Schema);

Console.WriteLine($"Loaded transform: {loaded.Transform.GetType().Name}");
Console.WriteLine(
    $"Loaded transform adds Score: {loadedSchema.Columns.Any(c => c.Name == "Score")}");
Console.WriteLine(
    $"Parameters survived: {loaded.Parameters is NeuralNetworkParameters}");
Console.WriteLine("Rebuild NeuralNetworkScoringTransform before inference.");
