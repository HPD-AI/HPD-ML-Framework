#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Inspect archive resource limits before passing bytes to ZipSerializer.

using System.IO.Compression;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.Serialization.Zip;

var definition = new NeuralNetworkDefinition(
    "Features", "Label", [new DenseLayerSpec(1, 1)]);
var parameters = new NeuralNetworkParameters(
    definition,
    [new[] { 2f }],
    [new[] { 1f }]);
IModel model = new Model(
    new NeuralNetworkScoringTransform(parameters),
    parameters);

using var stream = new MemoryStream();
var serializer = new ZipSerializer();
serializer.Save(model, SaveContent.All, new ZipFormat(), stream);

const int maxEntries = 16;
const long maxExpandedBytes = 1_000_000;
const double maxRatio = 100;

stream.Position = 0;
using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true))
{
    if (zip.Entries.Count > maxEntries)
        throw new InvalidOperationException("Too many archive entries.");

    long expanded = 0;
    foreach (ZipArchiveEntry entry in zip.Entries)
    {
        expanded += entry.Length;
        double ratio = entry.CompressedLength == 0
            ? entry.Length
            : (double)entry.Length / entry.CompressedLength;
        if (expanded > maxExpandedBytes || ratio > maxRatio)
            throw new InvalidOperationException("Archive resource limit exceeded.");
        Console.WriteLine(
            $"{entry.FullName}: compressed={entry.CompressedLength}, expanded={entry.Length}");
    }
}

stream.Position = 0;
_ = serializer.Load(new ZipFormat(), stream);
Console.WriteLine("Archive passed the application resource guard.");

// Guide: docs/operations/model-artifact-security.md

