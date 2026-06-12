#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Materialize a lazy CSV handle for known row count and repeated reads.

using HPD.ML.Abstractions;
using HPD.ML.DataSources;

var path = Path.Combine(Path.GetTempPath(), $"hpd-ml-materialize-{Guid.NewGuid():N}.csv");

try
{
    File.WriteAllText(path, "Id,Value\n1,10\n2,20\n3,30\n");

    var lazy = IDataHandle.LoadCsv(path);
    Console.WriteLine($"Before: RowCount={lazy.RowCount?.ToString() ?? "unknown"}, {lazy.Capabilities}");

    var memory = lazy.Materialize();
    Console.WriteLine($"After: RowCount={memory.RowCount}, {memory.Capabilities}");

    using var cursor = memory.GetCursor(["Value"]);
    var total = 0;
    while (cursor.MoveNext())
        total += cursor.Current.GetValue<int>("Value");

    Console.WriteLine($"Total: {total}");
}
finally
{
    if (File.Exists(path))
        File.Delete(path);
}
