#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Show how wrapper handles preserve or weaken ordering guarantees.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Sequence", Enumerable.Range(1, 8).ToArray()));

IDataHandle filtered = new FilteredDataHandle(
    source,
    row => row.GetValue<int>("Sequence") % 2 == 0);

IDataHandle shuffled = new ShuffledDataHandle(source, seed: 42);

Console.WriteLine($"source: {source.Ordering}");
Console.WriteLine($"filtered: {filtered.Ordering}");
Console.WriteLine($"shuffled: {shuffled.Ordering}");

static string ReadSequence(IDataHandle data)
{
    var values = new List<int>();
    using var rows = data.GetCursor(["Sequence"]);
    while (rows.MoveNext())
        values.Add(rows.Current.GetValue<int>("Sequence"));
    return string.Join(", ", values);
}

Console.WriteLine($"filtered values: {ReadSequence(filtered)}");
Console.WriteLine($"shuffled pass 1: {ReadSequence(shuffled)}");
Console.WriteLine($"shuffled pass 2: {ReadSequence(shuffled)}");

// The shuffled handle caches one materialized permutation.
// Guide: docs/concepts/ordering-and-stateful-execution.md
