#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Save parameters and topology, then inspect the ZIP entries and manifest.

using System.IO.Compression;
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
IModel model = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

using var stream = new MemoryStream();
new ZipSerializer().Save(
    model,
    SaveContent.LearnedParameters | SaveContent.PipelineTopology,
    new ZipFormat(),
    stream);

stream.Position = 0;
using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
foreach (var entry in zip.Entries.OrderBy(entry => entry.FullName))
    Console.WriteLine($"{entry.FullName,-32} {entry.Length,5} bytes");

Console.WriteLine("\nmanifest.json:");
using var manifest = new StreamReader(zip.GetEntry("manifest.json")!.Open());
Console.WriteLine(await manifest.ReadToEndAsync());
