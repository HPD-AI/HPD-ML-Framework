#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Read the same handle synchronously with a cursor and asynchronously as rows.

using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Value", new[] { 10.0f, 20.0f, 30.0f }));

using (var cursor = data.GetCursor(["Id"]))
{
    while (cursor.MoveNext())
        Console.WriteLine($"cursor: {cursor.Current.GetValue<int>("Id")}");
}

await foreach (var row in data.StreamRows())
    Console.WriteLine($"stream: {row.GetValue<float>("Value"):F1}");
