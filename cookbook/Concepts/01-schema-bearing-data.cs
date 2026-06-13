#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Build data with an explicit vector schema and inspect its contract.

using HPD.ML.Abstractions;
using HPD.ML.Core;

var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddVectorColumn<float>("Features", 2)
    .AddColumn<bool>("Label", role: "Label")
    .Build();

var data = new InMemoryDataHandle(
    (Schema)schema,
    new Dictionary<string, Array>
    {
        ["Id"] = new[] { 1, 2, 3 },
        ["Features"] = new float[][]
        {
            [1f, 0f],
            [0f, 1f],
            [1f, 1f]
        },
        ["Label"] = new[] { false, false, true }
    });

foreach (var column in data.Schema.Columns)
{
    string dimensions = column.Type.Dimensions is null
        ? "-"
        : string.Join("x", column.Type.Dimensions);

    Console.WriteLine(
        $"{column.Name}: CLR={column.Type.ClrType.Name}, " +
        $"vector={column.Type.IsVector}, dimensions={dimensions}");
}

var label = data.Schema.FindByName("Label")!;
label.Annotations.TryGetValue<bool>("role:Label", out bool hasLabelRole);
Console.WriteLine($"Label role annotation: {hasLabelRole}");

// Guide: docs/concepts/schemas-and-field-types.md
