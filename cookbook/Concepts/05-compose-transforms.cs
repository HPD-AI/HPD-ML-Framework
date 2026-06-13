#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Compose Core transforms left to right and inspect the resulting schema.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Value", new[] { 10f, 20f, 30f }),
    ("Unused", new[] { "a", "b", "c" }));

ITransform pipeline = TransformComposer.Compose(
    new ColumnCopyTransform("Value", "OriginalValue"),
    ColumnSelectTransform.Keep("Id", "Value", "OriginalValue"));

IDataHandle output = pipeline.Apply(data);

Console.WriteLine($"preserves rows: {pipeline.Properties.PreservesRowCount}");
Console.WriteLine($"columns: {string.Join(", ", output.Schema.Columns.Select(c => c.Name))}");

using var rows = output.GetCursor(["Id", "Value", "OriginalValue"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<int>("Id")}: " +
        $"{rows.Current.GetValue<float>("Value")} / " +
        $"{rows.Current.GetValue<float>("OriginalValue")}");
}

// Guide: docs/concepts/transforms-and-composition.md
