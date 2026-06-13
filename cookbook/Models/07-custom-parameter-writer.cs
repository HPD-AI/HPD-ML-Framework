#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Register the same custom parameter writer for save and load.

using System.Text;
using System.Text.Json;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Serialization.Zip;

var serializer = new ZipSerializer();
serializer.RegisterParameterWriter(new ScaleParameterWriter());

var parameters = new ScaleParameters(3.5);
IModel model = new Model(new IdentityTransform(), parameters);

using var stream = new MemoryStream();
serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    stream);

stream.Position = 0;
IModel loaded = serializer.Load(new ZipFormat(), stream);
var restored = (ScaleParameters)loaded.Parameters;

Console.WriteLine($"Restored scale: {restored.Scale:F1}");

sealed record ScaleParameters(double Scale) : ILearnedParameters;

sealed class ScaleParameterWriter : IParameterWriter<ScaleParameters>
{
    public string TypeName => nameof(ScaleParameters);

    public void WriteWeights(ScaleParameters parameters, Stream destination)
    {
        using var writer = new BinaryWriter(destination, Encoding.UTF8, leaveOpen: true);
        writer.Write(parameters.Scale);
    }

    public void WriteMetadata(
        ScaleParameters parameters,
        Stream destination,
        JsonSerializerOptions options)
    {
        destination.Write("""{"version":1}"""u8);
    }

    public ScaleParameters ReadModel(
        Stream weights,
        Stream metadata,
        JsonSerializerOptions options)
    {
        using var reader = new BinaryReader(weights, Encoding.UTF8, leaveOpen: true);
        return new ScaleParameters(reader.ReadDouble());
    }
}

sealed class IdentityTransform : ITransform
{
    public TransformProperties Properties => new() { PreservesRowCount = true };
    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;
    public IDataHandle Apply(IDataHandle input) => input;
}
