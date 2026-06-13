#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Register a matching parameter writer on both save and load paths.

using System.Text;
using System.Text.Json;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Serialization.Zip;

var serializer = new ZipSerializer();
serializer.RegisterParameterWriter(new OffsetWriter());

IModel model = new Model(
    new IdentityTransform(),
    new OffsetParameters(12.5));

using var stream = new MemoryStream();
serializer.Save(
    model,
    SaveContent.LearnedParameters,
    new ZipFormat(),
    stream);

stream.Position = 0;
IModel loaded = serializer.Load(new ZipFormat(), stream);

Console.WriteLine(
    $"Restored offset: {((OffsetParameters)loaded.Parameters).Value:F1}");

sealed record OffsetParameters(double Value) : ILearnedParameters;

sealed class OffsetWriter : IParameterWriter<OffsetParameters>
{
    public string TypeName => nameof(OffsetParameters);

    public void WriteWeights(
        OffsetParameters parameters,
        Stream destination)
    {
        using var writer = new BinaryWriter(
            destination,
            Encoding.UTF8,
            leaveOpen: true);
        writer.Write(parameters.Value);
    }

    public void WriteMetadata(
        OffsetParameters parameters,
        Stream destination,
        JsonSerializerOptions options) =>
        destination.Write("""{"version":1}"""u8);

    public OffsetParameters ReadModel(
        Stream weights,
        Stream metadata,
        JsonSerializerOptions options)
    {
        using var reader = new BinaryReader(
            weights,
            Encoding.UTF8,
            leaveOpen: true);
        return new OffsetParameters(reader.ReadDouble());
    }
}

sealed class IdentityTransform : ITransform
{
    public TransformProperties Properties => new()
    {
        PreservesRowCount = true
    };

    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;
    public IDataHandle Apply(IDataHandle input) => input;
}
