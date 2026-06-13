#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Restore parameters, rebuild scoring, validate schema, and run probe rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.Serialization.Zip;

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(2, 1)]);
var parameters = new NeuralNetworkParameters(
    definition,
    [new[] { 2f, -1f }],
    [new[] { 0.5f }]);
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
IModel partial = serializer.Load(new ZipFormat(), archive);
if (partial.Parameters is not NeuralNetworkParameters restoredParameters)
    throw new InvalidOperationException("Unexpected parameter type.");

IModel deployed = new Model(
    new NeuralNetworkScoringTransform(restoredParameters),
    restoredParameters);

IDataHandle probes = InMemoryDataHandle.FromColumns(
    ("Probe", new[] { "zero", "mixed" }),
    ("Features", new float[][] { [0f, 0f], [3f, 2f] }));

ISchema output = deployed.Transform.GetOutputSchema(probes.Schema);
if (!output.Columns.Any(column =>
        column.Name == "Score" && column.Type.ClrType == typeof(float)))
    throw new InvalidOperationException("Deployment output schema changed.");

float[] expected = [0.5f, 4.5f];
using var cursor = deployed.Transform.Apply(probes).GetCursor(["Probe", "Score"]);
var index = 0;
while (cursor.MoveNext())
{
    float actual = cursor.Current.GetValue<float>("Score");
    if (!float.IsFinite(actual) || Math.Abs(actual - expected[index]) > 1e-6f)
        throw new InvalidOperationException("Deployment probe failed.");
    Console.WriteLine(
        $"{cursor.Current.GetValue<string>("Probe")}: {actual:F3}");
    index++;
}

Console.WriteLine("Deployment probe passed.");

// Guide: docs/operations/deployment-validation.md

