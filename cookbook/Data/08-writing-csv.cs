#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Write any data handle to CSV, then load the result again.

using HPD.ML.Core;
using HPD.ML.DataSources;

var path = Path.Combine(Path.GetTempPath(), $"hpd-ml-write-{Guid.NewGuid():N}.csv");

try
{
    var source = InMemoryDataHandle.FromColumns(
        ("Id", new[] { 1, 2 }),
        ("Message", new[] { "hello, world", "she said \"hi\"" }));

    await CsvWriter.WriteAsync(source, path);
    Console.WriteLine(File.ReadAllText(path));

    var roundTrip = CsvDataHandle.Create(path);
    Console.WriteLine($"Round-trip rows: {roundTrip.Materialize().RowCount}");
}
finally
{
    if (File.Exists(path))
        File.Delete(path);
}
