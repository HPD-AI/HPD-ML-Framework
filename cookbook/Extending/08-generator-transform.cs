#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Execute an IGeneratorTransform with an explicit hard output limit.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle seed = InMemoryDataHandle.FromColumns(
    ("Start", new[] { 4 }));

var generator = new CountdownGenerator();
IDataHandle generated = generator.Apply(seed);

using var cursor = generated.GetCursor(["Value"]);
while (cursor.MoveNext())
    Console.WriteLine(cursor.Current.GetValue<int>("Value"));

readonly record struct CountdownState(int Current, ISchema OutputSchema);

sealed class CountdownGenerator : IGeneratorTransform<CountdownState>
{
    public int? MaxOutputLength => 100;
    public TransformProperties Properties => new()
    {
        IsStateful = true,
        PreservesRowCount = false
    };

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        new SchemaBuilder().AddColumn<int>("Value").Build();

    public CountdownState InitializeState(IDataHandle seed)
    {
        using var cursor = seed.GetCursor(["Start"]);
        if (!cursor.MoveNext())
            throw new InvalidOperationException("A seed row is required.");

        return new CountdownState(
            cursor.Current.GetValue<int>("Start"),
            GetOutputSchema(seed.Schema));
    }

    public (CountdownState NextState, IRow Output)? Step(
        CountdownState state)
    {
        if (state.Current < 0)
            return null;

        var output = new DictionaryRow(
            state.OutputSchema,
            new Dictionary<string, object>
            {
                ["Value"] = state.Current
            });

        return (
            state with { Current = state.Current - 1 },
            output);
    }

    public IDataHandle Apply(IDataHandle input)
    {
        var state = InitializeState(input);
        var values = new List<int>();
        var hardLimit = MaxOutputLength ?? 10_000;

        for (var index = 0; index < hardLimit; index++)
        {
            var step = Step(state);
            if (step is null)
                return InMemoryDataHandle.FromColumns(
                    ("Value", values.ToArray()));

            state = step.Value.NextState;
            values.Add(step.Value.Output.GetValue<int>("Value"));
        }

        throw new InvalidOperationException(
            "Generator exceeded its output limit.");
    }
}
