#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// This sample verifies that HPD ML restores and can create schema-bearing data.

using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Temperature", new float[] { 18.5f, 21.0f, 23.5f }));

var column = data.Schema.Columns[0];

Console.WriteLine("HPD ML is ready.");
Console.WriteLine($"Rows: {data.RowCount}");
Console.WriteLine($"Column: {column.Name} ({column.Type.ClrType.Name})");
