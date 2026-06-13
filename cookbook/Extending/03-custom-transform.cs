#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Add a derived column lazily while keeping schema and row behavior aligned.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 1.0f, 2.0f, 3.0f }));

ITransform transform = new AddConstantTransform(
    sourceColumn: "Value",
    outputColumn: "Adjusted",
    amount: 10.0f);

IDataHandle result = transform.Apply(source);

using var cursor = result.GetCursor(["Adjusted"]);
while (cursor.MoveNext())
    Console.WriteLine(cursor.Current.GetValue<float>("Adjusted"));

sealed class AddConstantTransform(
    string sourceColumn,
    string outputColumn,
    float amount) : ITransform
{
    public TransformProperties Properties => new()
    {
        PreservesRowCount = true
    };

    public ISchema GetOutputSchema(ISchema inputSchema)
    {
        var source = inputSchema.FindByName(sourceColumn)
            ?? throw new InvalidOperationException(
                $"Column '{sourceColumn}' was not found.");

        if (source.Type.ClrType != typeof(float))
            throw new InvalidOperationException(
                $"Column '{sourceColumn}' must be Single.");

        if (inputSchema.FindByName(outputColumn) is not null)
            throw new InvalidOperationException(
                $"Column '{outputColumn}' already exists.");

        return new Schema(
            inputSchema.Columns
                .Append(new Column(outputColumn, FieldType.Scalar<float>()))
                .ToArray(),
            inputSchema.Level);
    }

    public IDataHandle Apply(IDataHandle input)
    {
        var outputSchema = GetOutputSchema(input.Schema);
        var required = input.Schema.Columns.Select(column => column.Name)
            .Append(sourceColumn)
            .Distinct()
            .ToArray();

        return new CursorDataHandle(
            outputSchema,
            _ => new MappedCursor(
                input.GetCursor(required),
                row =>
                {
                    var values = input.Schema.Columns.ToDictionary(
                        column => column.Name,
                        column => row.GetValue<object>(column.Name));

                    values[outputColumn] =
                        row.GetValue<float>(sourceColumn) + amount;

                    return new DictionaryRow(outputSchema, values);
                }),
            input.RowCount,
            input.Ordering);
    }
}
