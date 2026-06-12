#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Use an explicit CSV schema to avoid inference and attach vector metadata.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DataSources;

var path = Path.Combine(Path.GetTempPath(), $"hpd-ml-schema-{Guid.NewGuid():N}.csv");

try
{
    File.WriteAllText(path, """
Id,Score
1,0.25
2,0.75
""");

    var schema = new SchemaBuilder()
        .AddColumn<int>("Id")
        .AddColumn<float>("Score")
        .Build();

    var data = IDataHandle.LoadCsv(path, schema);
    Console.WriteLine(data.Schema.FindByName("Score")!.Type.ClrType.Name);

    var modelSchema = new SchemaBuilder()
        .AddVectorColumn<float>("Features", 2)
        .AddColumn<bool>("Label", role: "Label")
        .Build();

    var label = modelSchema.FindByName("Label")!;
    Console.WriteLine($"Features dimensions: {string.Join("x", modelSchema.FindByName("Features")!.Type.Dimensions!)}");
    Console.WriteLine($"Label role: {label.Annotations.TryGetValue<bool>("role:Label", out var isLabel) && isLabel}");
}
finally
{
    if (File.Exists(path))
        File.Delete(path);
}
