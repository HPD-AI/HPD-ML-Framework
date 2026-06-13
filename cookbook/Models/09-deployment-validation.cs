#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Validate type, schema, and probe predictions after parameter restoration.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.Serialization.Zip;

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [new DenseLayerSpec(2, 1)]);
var parameters = new NeuralNetworkParameters(
    definition,
    [new[] { 2f, -1f }],
    [new[] { 0.5f }]);
IModel original = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

using var stream = new MemoryStream();
var serializer = new ZipSerializer();
serializer.Save(
    original,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    stream);

stream.Position = 0;
IModel partial = serializer.Load(new ZipFormat(), stream);
if (partial.Parameters is not NeuralNetworkParameters restoredParameters)
    throw new InvalidOperationException("Unexpected parameter type.");

IModel restored = new Model(
    new NeuralNetworkScoringTransform(restoredParameters),
    restoredParameters);

IDataHandle probe = InMemoryDataHandle.FromColumns(
    ("ProbeId", new[] { "zero", "mixed" }),
    ("Features", new float[][] { [0f, 0f], [3f, 2f] }));

ISchema schema = restored.Transform.GetOutputSchema(probe.Schema);
if (!schema.Columns.Any(column =>
        column.Name == "Score" &&
        column.Type.ClrType == typeof(float)))
{
    throw new InvalidOperationException("Expected scalar float Score.");
}

float[] expected = [0.5f, 4.5f];
using var rows = restored.Transform.Apply(probe).GetCursor(["ProbeId", "Score"]);
var index = 0;
while (rows.MoveNext())
{
    float score = rows.Current.GetValue<float>("Score");
    float difference = Math.Abs(score - expected[index]);
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("ProbeId")}: " +
        $"score={score:F3}, expected={expected[index]:F3}, diff={difference:E2}");
    if (difference > 1e-6f)
        throw new InvalidOperationException("Probe prediction mismatch.");
    index++;
}

Console.WriteLine("Deployment validation passed.");
