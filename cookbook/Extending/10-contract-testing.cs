#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Run small public-contract assertions without project references.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle source = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 1.0f, 2.0f }));

ITransform transform = new AddOneTransform();
ISchema outputSchema = transform.GetOutputSchema(source.Schema);
IDataHandle output = transform.Apply(source);

Require(
    outputSchema.FindByName("Adjusted")?.Type.ClrType == typeof(float),
    "Output schema should declare Adjusted as Single.");

Require(
    output.RowCount == source.RowCount,
    "A row-preserving transform should preserve RowCount.");

using var cursor = output.GetCursor(["Adjusted"]);
Require(cursor.MoveNext(), "Expected the first output row.");
Require(
    cursor.Current.GetValue<float>("Adjusted") == 2.0f,
    "Adjusted value should equal Value + 1.");

Console.WriteLine("Contract checks passed.");

static void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

sealed class AddOneTransform : ITransform
{
    public TransformProperties Properties => new()
    {
        PreservesRowCount = true
    };

    public ISchema GetOutputSchema(ISchema inputSchema)
    {
        if (inputSchema.FindByName("Adjusted") is not null)
            throw new InvalidOperationException(
                "Column 'Adjusted' already exists.");

        return new Schema(
            inputSchema.Columns
                .Append(new Column(
                    "Adjusted",
                    FieldType.Scalar<float>()))
                .ToArray(),
            inputSchema.Level);
    }

    public IDataHandle Apply(IDataHandle input)
    {
        var schema = GetOutputSchema(input.Schema);
        var required = input.Schema.Columns.Select(column => column.Name)
            .Append("Value")
            .Distinct()
            .ToArray();

        return new CursorDataHandle(
            schema,
            _ => new MappedCursor(
                input.GetCursor(required),
                row =>
                {
                    var values = input.Schema.Columns.ToDictionary(
                        column => column.Name,
                        column => row.GetValue<object>(column.Name));
                    values["Adjusted"] =
                        row.GetValue<float>("Value") + 1;
                    return new DictionaryRow(schema, values);
                }),
            input.RowCount,
            input.Ordering);
    }
}
