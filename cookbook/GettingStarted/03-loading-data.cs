#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DataSources@0.5.0
#:property TargetFramework=net10.0

// This sample loads the same kind of records from memory, CSV, and JSON Lines.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DataSources;

var dataRoot = Path.Combine(Path.GetTempPath(), "hpd-ml-loading-data");
Directory.CreateDirectory(dataRoot);

var csvPath = Path.Combine(dataRoot, "readings.csv");
var jsonPath = Path.Combine(dataRoot, "readings.jsonl");

File.WriteAllText(csvPath, """
Id,Temperature,IsWarm
1,18.5,false
2,21.0,true
3,23.5,true
""");

File.WriteAllText(jsonPath, """
{"id":1,"temperature":18.5,"isWarm":false}
{"id":2,"temperature":21.0,"isWarm":true}
{"id":3,"temperature":23.5,"isWarm":true}
""");

IDataHandle memory = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Temperature", new[] { 18.5f, 21.0f, 23.5f }),
    ("IsWarm", new[] { false, true, true }));

IDataHandle csv = IDataHandle.LoadCsv(csvPath);
IDataHandle json = IDataHandle.LoadJson(
    jsonPath,
    new JsonOptions { IsJsonLines = true });

PrintSummary("Memory", memory);
PrintSummary("CSV", csv);
PrintSummary("JSONL", json);

static void PrintSummary(string name, IDataHandle data)
{
    var schema = string.Join(
        ", ",
        data.Schema.Columns.Select(column =>
            $"{column.Name}:{column.Type.ClrType.Name}"));

    var rowCount = 0;
    using var rows = data.GetCursor(data.Schema.Columns.Select(column => column.Name));
    while (rows.MoveNext())
        rowCount++;

    Console.WriteLine($"{name}: {rowCount} rows [{schema}]");
}
