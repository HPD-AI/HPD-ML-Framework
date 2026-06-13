#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Demonstrate deliberate and incidental 0.5.0 load boundaries.

using System.IO.Compression;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Serialization.Zip;

var serializer = new ZipSerializer();
IModel model = new Model(
    new IdentityTransform(),
    new UnregisteredParameters([1.0, 2.0]));

using var fallback = new MemoryStream();
serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    fallback);

fallback.Position = 0;
TryLoad("JSON fallback", serializer, fallback);

using var wrongFormat = new MemoryStream();
using (var zip = new ZipArchive(wrongFormat, ZipArchiveMode.Create, leaveOpen: true))
{
    var entry = zip.CreateEntry("manifest.json");
    using var writer = new StreamWriter(entry.Open());
    writer.Write("""{"formatId":"not-hpd","schemaVersion":1,"content":0}""");
}

wrongFormat.Position = 0;
TryLoad("Wrong format", serializer, wrongFormat);

static void TryLoad(string name, ZipSerializer serializer, Stream stream)
{
    try
    {
        serializer.Load(new ZipFormat(), stream);
        Console.WriteLine($"{name}: unexpectedly loaded");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"{name}: {exception.GetType().Name}");
        Console.WriteLine($"  {exception.Message}");
    }
}

sealed record UnregisteredParameters(double[] Values) : ILearnedParameters;

sealed class IdentityTransform : ITransform
{
    public TransformProperties Properties => new() { PreservesRowCount = true };
    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;
    public IDataHandle Apply(IDataHandle input) => input;
}
