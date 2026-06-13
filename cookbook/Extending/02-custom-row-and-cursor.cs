#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Implement exact typed access without unsafe numeric reinterpretation.

using System.Runtime.CompilerServices;
using HPD.ML.Abstractions;
using HPD.ML.Core;

var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddColumn<float>("Value")
    .Build();

using var cursor = new SampleCursor(
    schema,
    [(1, 1.5f), (2, 2.5f), (3, 3.5f)]);

while (cursor.MoveNext())
{
    Console.WriteLine(
        $"{cursor.Current.GetValue<int>("Id")} -> " +
        $"{cursor.Current.GetValue<float>("Value"):F1}");
}

sealed class SampleCursor(
    ISchema schema,
    IReadOnlyList<(int Id, float Value)> rows) : IRowCursor
{
    private int _position = -1;
    private SampleRow? _current;

    public IRow Current => _current
        ?? throw new InvalidOperationException("Call MoveNext first.");

    public bool MoveNext()
    {
        _position++;
        if (_position >= rows.Count)
        {
            _current = null;
            return false;
        }

        var row = rows[_position];
        _current = new SampleRow(schema, row.Id, row.Value);
        return true;
    }

    public void Dispose() => _current = null;
}

sealed class SampleRow(ISchema schema, int id, float value) : IRow
{
    public ISchema Schema { get; } = schema;

    public T GetValue<T>(string columnName) where T : allows ref struct
    {
        if (TryGetValue<T>(columnName, out var result))
            return result;

        if (Schema.FindByName(columnName) is null)
            throw new KeyNotFoundException($"Column '{columnName}' was not found.");

        throw new InvalidCastException(
            $"Column '{columnName}' cannot be read as {typeof(T).Name}.");
    }

    public bool TryGetValue<T>(
        string columnName,
        out T result) where T : allows ref struct
    {
        if (columnName == "Id" && typeof(T) == typeof(int))
        {
            var copy = id;
            result = Unsafe.As<int, T>(ref copy);
            return true;
        }

        if (columnName == "Value" && typeof(T) == typeof(float))
        {
            var copy = value;
            result = Unsafe.As<float, T>(ref copy);
            return true;
        }

        result = default!;
        return false;
    }
}
