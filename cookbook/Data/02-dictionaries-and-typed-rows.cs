#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Create handles from dynamic dictionaries and reflection-free typed records.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DataSources;

IReadOnlyDictionary<string, object>[] rows =
[
    new Dictionary<string, object> { ["City"] = "Chicago", ["Temperature"] = 21.0f },
    new Dictionary<string, object> { ["City"] = "Austin", ["Temperature"] = 29.5f }
];

var dynamicData = IDataHandle.FromDictionaries(rows);
Console.WriteLine($"Dictionary rows: {dynamicData.RowCount}");

var schema = new SchemaBuilder()
    .AddColumn("City", new FieldType(typeof(string)))
    .AddColumn<float>("Temperature")
    .Build();

var readings = new[]
{
    new Reading("Chicago", 21.0f),
    new Reading("Austin", 29.5f)
};

var typedData = IDataHandle.FromEnumerable(
    readings,
    schema,
    reading => new Dictionary<string, object>
    {
        ["City"] = reading.City,
        ["Temperature"] = reading.Temperature
    });

using var cursor = typedData.GetCursor(["City", "Temperature"]);
while (cursor.MoveNext())
{
    Console.WriteLine(
        $"{cursor.Current.GetValue<string>("City")}: " +
        $"{cursor.Current.GetValue<float>("Temperature"):F1}");
}

public sealed record Reading(string City, float Temperature);
