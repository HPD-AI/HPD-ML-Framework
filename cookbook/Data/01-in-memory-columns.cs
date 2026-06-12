#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Create a schema-bearing data handle from application-owned arrays.

using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 101, 102, 103 }),
    ("Temperature", new[] { 18.5f, 21.0f, 23.5f }),
    ("IsWarm", new[] { false, true, true }));

Console.WriteLine($"Rows: {data.RowCount}");

foreach (var column in data.Schema.Columns)
    Console.WriteLine($"{column.Name}: {column.Type.ClrType.Name}");

using var cursor = data.GetCursor(["Id", "Temperature"]);
while (cursor.MoveNext())
{
    Console.WriteLine(
        $"{cursor.Current.GetValue<int>("Id")}: " +
        $"{cursor.Current.GetValue<float>("Temperature"):F1}");
}
