#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// A filter stays lazy; CachedDataHandle materializes it once on first access.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Id", Enumerable.Range(1, 6).ToArray()),
    ("Value", new[] { -3f, -2f, -1f, 1f, 2f, 3f }));

IDataHandle filtered = new FilteredDataHandle(
    source,
    row => row.GetValue<float>("Value") > 0);

Console.WriteLine($"filtered row count before reading: {filtered.RowCount?.ToString() ?? "unknown"}");
Console.WriteLine($"filtered capabilities: {filtered.Capabilities}");

IDataHandle cached = new CachedDataHandle(filtered);
using (var rows = cached.GetCursor(["Id", "Value"]))
{
    while (rows.MoveNext())
        Console.WriteLine($"{rows.Current.GetValue<int>("Id")}: {rows.Current.GetValue<float>("Value")}");
}

Console.WriteLine($"cached row count after first read: {cached.RowCount}");
Console.WriteLine($"cached capabilities after first read: {cached.Capabilities}");

// Guide: docs/concepts/lazy-eager-and-cached-execution.md
