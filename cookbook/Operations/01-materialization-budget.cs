#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Estimate a dense training representation before deliberately caching it.

using HPD.ML.Abstractions;
using HPD.ML.Core;

const int rows = 1_000;
const int width = 32;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Features", Enumerable.Range(0, rows)
        .Select(row => Enumerable.Range(0, width)
            .Select(column => (float)(row + column))
            .ToArray())
        .ToArray()),
    ("Label", Enumerable.Range(0, rows).Select(value => (float)value).ToArray()));

long featureBytes = (long)rows * width * sizeof(float);
long labelBytes = (long)rows * sizeof(float);
Console.WriteLine($"rows={rows}, width={width}");
Console.WriteLine($"dense payload estimate={featureBytes + labelBytes:N0} bytes");
Console.WriteLine($"source capabilities={source.Capabilities}");

IDataHandle cached = new CachedDataHandle(source);
Console.WriteLine($"cached before access={cached.Capabilities}");

using (var cursor = cached.GetCursor(["Features", "Label"]))
{
    cursor.MoveNext();
    Console.WriteLine(
        $"first vector width={cursor.Current.GetValue<float[]>("Features").Length}");
}

Console.WriteLine($"cached after access={cached.Capabilities}");
Console.WriteLine("Budget estimates exclude array, object, model, and backend overhead.");

// Guide: docs/operations/performance-and-memory.md

