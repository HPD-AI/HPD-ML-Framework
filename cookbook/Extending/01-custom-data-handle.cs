#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0
#:property NoWarn=SYSLIB5001

// Wrap an existing source behind a custom IDataHandle contract.

using System.Numerics;
using System.Numerics.Tensors;
using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle data = new NamedDataHandle(
    "sensor-feed",
    InMemoryDataHandle.FromColumns(
        ("Id", new[] { 1, 2, 3 }),
        ("Temperature", new[] { 20.5f, 21.0f, 22.25f })));

Console.WriteLine($"Name: {((NamedDataHandle)data).Name}");
Console.WriteLine($"Rows: {data.RowCount}");
Console.WriteLine($"Ordering: {data.Ordering}");

using var cursor = data.GetCursor(["Id", "Temperature"]);
while (cursor.MoveNext())
{
    Console.WriteLine(
        $"{cursor.Current.GetValue<int>("Id")}: " +
        $"{cursor.Current.GetValue<float>("Temperature"):F2}");
}

sealed class NamedDataHandle(string name, IDataHandle inner) : IDataHandle
{
    public string Name { get; } = name;
    public ISchema Schema => inner.Schema;
    public long? RowCount => inner.RowCount;
    public OrderingPolicy Ordering => inner.Ordering;
    public MaterializationCapabilities Capabilities => inner.Capabilities;

    public IRowCursor GetCursor(IEnumerable<string> columnsNeeded) =>
        inner.GetCursor(columnsNeeded);

    public IDataHandle Materialize() => inner.Materialize();

    public IAsyncEnumerable<IRow> StreamRows(CancellationToken ct = default) =>
        inner.StreamRows(ct);

    public bool TryGetColumnBatch<T>(
        string columnName,
        int startRow,
        int rowCount,
        out ReadOnlyTensorSpan<T> batch)
        where T : unmanaged, INumber<T> =>
        inner.TryGetColumnBatch(columnName, startRow, rowCount, out batch);
}
