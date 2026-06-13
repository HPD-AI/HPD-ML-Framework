#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Treat each requested cursor as a caller-owned disposable resource.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Id", new[] { 1, 2, 3 }),
    ("Value", new[] { 10f, 20f, 30f }));

IRowCursor first = data.GetCursor(["Id"]);
IRowCursor second = data.GetCursor(["Id"]);

first.MoveNext();
first.MoveNext();
second.MoveNext();

Console.WriteLine($"first cursor id={first.Current.GetValue<int>("Id")}");
Console.WriteLine($"second cursor id={second.Current.GetValue<int>("Id")}");
Console.WriteLine("The two cursors have independent positions.");

first.Dispose();
second.Dispose();
Console.WriteLine("Both caller-owned cursors were disposed.");
Console.WriteLine("Do not retain cursor rows after advancing or disposal.");

// Guide: docs/operations/lifecycle-and-concurrency.md

