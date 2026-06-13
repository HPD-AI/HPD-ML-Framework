#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Read one handle by cursor, async stream, and materialized column batch.

using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Value", new[] { 10f, 20f, 30f }));

using (var cursor = data.GetCursor(["Id"]))
{
    while (cursor.MoveNext())
        Console.WriteLine($"cursor: {cursor.Current.GetValue<int>("Id")}");
}

await foreach (var row in data.StreamRows())
    Console.WriteLine($"stream: {row.GetValue<float>("Value"):F1}");

var materialized = data.Materialize();
if (materialized.TryGetColumnBatch<float>("Value", 0, 3, out var batch))
    Console.WriteLine($"batch length: {batch.FlattenedLength}");

Console.WriteLine($"row count: {materialized.RowCount}");
Console.WriteLine($"capabilities: {materialized.Capabilities}");

// Guide: docs/concepts/data-handles-rows-and-cursors.md
