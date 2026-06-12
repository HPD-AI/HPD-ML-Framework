#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// Load CSV lazily, inspect its inferred schema, and read selected columns.

using HPD.ML.Abstractions;
using HPD.ML.DataSources;

var path = Path.Combine(Path.GetTempPath(), $"hpd-ml-csv-{Guid.NewGuid():N}.csv");

try
{
    File.WriteAllText(path, """
Id,Temperature,Station
1,18.5,"North, Lake"
2,21.0,Central
3,,South
""");

    var data = IDataHandle.LoadCsv(
        path,
        new CsvOptions { MissingValuePolicy = MissingValuePolicy.NaN });

    foreach (var column in data.Schema.Columns)
        Console.WriteLine($"{column.Name}: {column.Type.ClrType.Name}");

    using var cursor = data.GetCursor(["Id", "Temperature"]);
    while (cursor.MoveNext())
    {
        var temperature = cursor.Current.GetValue<double>("Temperature");
        Console.WriteLine(
            $"{cursor.Current.GetValue<int>("Id")}: " +
            $"{(double.IsNaN(temperature) ? "missing" : temperature.ToString("F1"))}");
    }
}
finally
{
    if (File.Exists(path))
        File.Delete(path);
}
