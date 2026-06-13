#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Fit a mean value, expose it as learned parameters, and center future rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 2.0f, 4.0f, 6.0f }));

ILearner learner = new CenteringLearner("Value");
IModel model = learner.Fit(new LearnerInput(training));

var parameters = (CenteringParameters)model.Parameters;
Console.WriteLine($"Learned mean: {parameters.Mean:F1}");

IDataHandle scored = model.Transform.Apply(training);
using var cursor = scored.GetCursor(["Centered"]);
while (cursor.MoveNext())
    Console.WriteLine(cursor.Current.GetValue<float>("Centered"));

sealed record CenteringParameters(float Mean) : ILearnedParameters;

sealed class CenteringLearner(string columnName) : ILearner
{
    private readonly ProgressSubject _progress = new();

    public IObservable<ProgressEvent> Progress => _progress;

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        BuildOutputSchema(inputSchema, columnName);

    public IModel Fit(LearnerInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.ValidationData is not null || input.InitialModel is not null)
            throw new NotSupportedException(
                "This learner does not support validation data or warm starts.");

        var values = new List<float>();
        using var cursor = input.TrainData.GetCursor([columnName]);
        while (cursor.MoveNext())
        {
            input.Environment?.CancellationToken.ThrowIfCancellationRequested();
            values.Add(cursor.Current.GetValue<float>(columnName));
        }

        if (values.Count == 0)
            throw new InvalidOperationException("Training data is empty.");

        var parameters = new CenteringParameters(values.Average());
        _progress.OnNext(new ProgressEvent
        {
            Epoch = 0,
            MetricName = "Mean",
            MetricValue = parameters.Mean
        });
        _progress.OnCompleted();

        return new Model(
            new CenteringTransform(columnName, parameters),
            parameters);
    }

    public Task<IModel> FitAsync(
        LearnerInput input,
        CancellationToken ct = default) =>
        Task.Run(() =>
        {
            ct.ThrowIfCancellationRequested();
            return Fit(input);
        }, ct);

    internal static ISchema BuildOutputSchema(
        ISchema inputSchema,
        string inputColumn)
    {
        var column = inputSchema.FindByName(inputColumn)
            ?? throw new InvalidOperationException(
                $"Column '{inputColumn}' was not found.");

        if (column.Type.ClrType != typeof(float))
            throw new InvalidOperationException(
                $"Column '{inputColumn}' must be Single.");

        return new Schema(
            inputSchema.Columns
                .Append(new Column("Centered", FieldType.Scalar<float>()))
                .ToArray(),
            inputSchema.Level);
    }
}

sealed class CenteringTransform(
    string columnName,
    CenteringParameters parameters) : ITransform
{
    public TransformProperties Properties => new()
    {
        PreservesRowCount = true
    };

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        CenteringLearner.BuildOutputSchema(inputSchema, columnName);

    public IDataHandle Apply(IDataHandle input)
    {
        var schema = GetOutputSchema(input.Schema);
        var required = input.Schema.Columns.Select(column => column.Name)
            .Append(columnName)
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
                    values["Centered"] =
                        row.GetValue<float>(columnName) - parameters.Mean;
                    return new DictionaryRow(schema, values);
                }),
            input.RowCount,
            input.Ordering);
    }
}
