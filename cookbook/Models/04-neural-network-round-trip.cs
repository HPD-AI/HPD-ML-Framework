#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Round-trip neural-network parameters and rebuild scoring explicitly.

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
    weights: [new[] { 2f }],
    biases: [new[] { 1f }]);

IModel original = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

using var archive = new MemoryStream();
var serializer = new ZipSerializer();
serializer.Save(
    original,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    archive);

archive.Position = 0;
IModel loaded = serializer.Load(new ZipFormat(), archive);
var loadedParameters = (NeuralNetworkParameters)loaded.Parameters;
IModel restored = new Model(
    new NeuralNetworkScoringTransform(loadedParameters),
    loadedParameters);

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [3f] }));

float before = ReadScore(original.Transform.Apply(input));
float after = ReadScore(restored.Transform.Apply(input));

Console.WriteLine($"Before: {before:F3}");
Console.WriteLine($"After:  {after:F3}");
Console.WriteLine($"Match: {before == after}");

static float ReadScore(IDataHandle data)
{
    using var rows = data.GetCursor(["Score"]);
    rows.MoveNext();
    return rows.Current.GetValue<float>("Score");
}
