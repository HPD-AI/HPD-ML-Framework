#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Verify that an explicit split seed reproduces the same selected rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Id", Enumerable.Range(1, 20).ToArray()),
    ("Value", Enumerable.Range(1, 20).Select(value => (float)value).ToArray()));

var first = DataHandleSplitter.TrainTestSplit(data, testFraction: 0.25, seed: 42);
var second = DataHandleSplitter.TrainTestSplit(data, testFraction: 0.25, seed: 42);
var third = DataHandleSplitter.TrainTestSplit(data, testFraction: 0.25, seed: 7);

int[] firstIds = ReadIds(first.Test);
int[] secondIds = ReadIds(second.Test);
int[] thirdIds = ReadIds(third.Test);

Console.WriteLine($"seed 42 test: {string.Join(", ", firstIds)}");
Console.WriteLine($"seed 42 repeatable: {firstIds.SequenceEqual(secondIds)}");
Console.WriteLine($"seed 7 differs: {!firstIds.SequenceEqual(thirdIds)}");

static int[] ReadIds(IDataHandle handle)
{
    var values = new List<int>();
    using var cursor = handle.GetCursor(["Id"]);
    while (cursor.MoveNext())
        values.Add(cursor.Current.GetValue<int>("Id"));
    return values.ToArray();
}

// Guide: docs/operations/reproducibility.md

