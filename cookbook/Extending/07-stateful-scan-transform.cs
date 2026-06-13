#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Adapt an IScanTransform with independent state for every cursor.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 1.0f, 2.0f, 3.0f }));

ITransform runningTotal = new RunningTotalScan("Value");
IDataHandle result = runningTotal.Apply(source);

using var cursor = result.GetCursor(["RunningTotal"]);
while (cursor.MoveNext())
    Console.WriteLine(cursor.Current.GetValue<float>("RunningTotal"));

sealed class RunningTotalScan(string columnName)
    : IScanTransform<float>
{
    public TransformProperties Properties => new()
    {
        IsStateful = true,
        RequiresOrdering = true,
        PreservesRowCount = true
    };

    public IStateSerializer<float>? StateSerializer => null;
    public float InitializeState() => 0;

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        new Schema(
            inputSchema.Columns
                .Append(new Column(
                    "RunningTotal",
                    FieldType.Scalar<float>()))
                .ToArray(),
            inputSchema.Level);

    public (float NextState, IRow Output) ProcessRow(
        float state,
        IRow input)
    {
        var next = state + input.GetValue<float>(columnName);
        var schema = GetOutputSchema(input.Schema);
        var values = input.Schema.Columns.ToDictionary(
            column => column.Name,
            column => input.GetValue<object>(column.Name));
        values["RunningTotal"] = next;
        return (next, new DictionaryRow(schema, values));
    }

    public IDataHandle Apply(IDataHandle input)
    {
        if (input.Ordering == OrderingPolicy.Unordered)
            throw new InvalidOperationException(
                "Running totals require ordered input.");

        var schema = GetOutputSchema(input.Schema);
        var required = input.Schema.Columns.Select(column => column.Name)
            .Append(columnName)
            .Distinct()
            .ToArray();

        return new CursorDataHandle(
            schema,
            _ => new ScanCursor(
                this,
                input.GetCursor(required)),
            input.RowCount,
            input.Ordering);
    }
}

sealed class ScanCursor(
    RunningTotalScan scan,
    IRowCursor inner) : IRowCursor
{
    private float _state = scan.InitializeState();
    private IRow? _current;

    public IRow Current => _current
        ?? throw new InvalidOperationException("Call MoveNext first.");

    public bool MoveNext()
    {
        if (!inner.MoveNext())
        {
            _current = null;
            return false;
        }

        (_state, _current) = scan.ProcessRow(_state, inner.Current);
        return true;
    }

    public void Dispose() => inner.Dispose();
}
