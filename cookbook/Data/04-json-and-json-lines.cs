#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Compare JSON Lines streaming with JSON-array loading and property mapping.

using HPD.ML.Abstractions;
using HPD.ML.DataSources;

var root = Path.Combine(Path.GetTempPath(), $"hpd-ml-json-{Guid.NewGuid():N}");
Directory.CreateDirectory(root);

var linesPath = Path.Combine(root, "readings.jsonl");
var arrayPath = Path.Combine(root, "readings.json");

try
{
    File.WriteAllText(linesPath, """
{"device":{"id":1},"temperature":18.5}
{"device":{"id":2},"temperature":21.0}
""");

    File.WriteAllText(arrayPath, """
[
  {"id": 1, "active": true},
  {"id": 2, "active": false}
]
""");

    var lines = IDataHandle.LoadJson(
        linesPath,
        new JsonOptions
        {
            IsJsonLines = true,
            MaxFlattenDepth = 1,
            PropertyMapping = new Dictionary<string, string>
            {
                ["device.id"] = "DeviceId"
            }
        });

    await foreach (var row in lines.StreamRows())
        Console.WriteLine($"Device {row.GetValue<int>("DeviceId")}");

    var array = IDataHandle.LoadJson(
        arrayPath,
        new JsonOptions { IsJsonLines = false });

    using var cursor = array.GetCursor(["id", "active"]);
    while (cursor.MoveNext())
        Console.WriteLine($"{cursor.Current.GetValue<int>("id")}: {cursor.Current.GetValue<bool>("active")}");
}
finally
{
    Directory.Delete(root, recursive: true);
}
