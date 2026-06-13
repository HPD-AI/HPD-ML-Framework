#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Build the smallest IModel and apply its transform.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IModel model = new Model(
    new AddOneTransform(),
    new AddOneParameters(1f));

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 1f, 4f, 9f }));

Console.WriteLine($"Transform: {model.Transform.GetType().Name}");
Console.WriteLine($"Parameters: {model.Parameters.GetType().Name}");
Console.WriteLine($"Preserves rows: {model.Transform.Properties.PreservesRowCount}");

using var rows = model.Transform.Apply(input).GetCursor(["Value", "Score"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<float>("Value"):F1} -> " +
        $"{rows.Current.GetValue<float>("Score"):F1}");
}

sealed record AddOneParameters(float Amount) : ILearnedParameters;

sealed class AddOneTransform : ITransform
{
    public TransformProperties Properties => new() { PreservesRowCount = true };

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        inputSchema.MergeHorizontal(
            new SchemaBuilder().AddColumn<float>("Score").Build(),
            ConflictPolicy.LastWriterWins);

    public IDataHandle Apply(IDataHandle input)
    {
        var values = new List<float>();
        var scores = new List<float>();
        using var rows = input.GetCursor(["Value"]);
        while (rows.MoveNext())
        {
            var value = rows.Current.GetValue<float>("Value");
            values.Add(value);
            scores.Add(value + 1f);
        }

        return InMemoryDataHandle.FromColumns(
            ("Value", values.ToArray()),
            ("Score", scores.ToArray()));
    }
}
