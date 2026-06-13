#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Evaluation@0.5.0
#:property TargetFramework=net10.0

// Rank whole vector feature columns with a deterministic model.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Evaluation;

var rng = new Random(42);
float[][] signal = Enumerable.Range(0, 30)
    .Select(i => new[] { (float)i })
    .ToArray();
float[][] noise = Enumerable.Range(0, 30)
    .Select(_ => new[] { (float)rng.NextDouble() })
    .ToArray();

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Label", signal.Select(x => (double)x[0]).ToArray()),
    ("Signal", signal),
    ("Noise", noise));

IDataHandle importance = PermutationFeatureImportance.Compute(
    new SignalModel(),
    data,
    ITransform.RegressionMetrics(),
    metricName: "RSquared",
    featureColumns: ["Signal", "Noise"],
    permutations: 5,
    seed: 42);

using var rows = importance.GetCursor(
    ["FeatureName", "MetricDrop", "MetricDropStdDev"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("FeatureName"),-8} " +
        $"drop={rows.Current.GetValue<double>("MetricDrop"),8:F4} " +
        $"std={rows.Current.GetValue<double>("MetricDropStdDev"),8:F4}");
}

sealed class SignalModel : IModel
{
    public ITransform Transform { get; } = new SignalTransform();
    public ILearnedParameters Parameters { get; } = new EmptyParameters();
}

sealed class EmptyParameters : ILearnedParameters;

sealed class SignalTransform : ITransform
{
    public TransformProperties Properties => new();

    public ISchema GetOutputSchema(ISchema inputSchema) =>
        inputSchema.MergeHorizontal(
            new SchemaBuilder().AddColumn<double>("Score").Build(),
            ConflictPolicy.LastWriterWins);

    public IDataHandle Apply(IDataHandle input)
    {
        var labels = new List<double>();
        var scores = new List<double>();
        using var rows = input.GetCursor(["Label", "Signal"]);
        while (rows.MoveNext())
        {
            labels.Add(rows.Current.GetValue<double>("Label"));
            scores.Add(rows.Current.GetValue<float[]>("Signal")[0]);
        }

        return InMemoryDataHandle.FromColumns(
            ("Label", labels.ToArray()),
            ("Score", scores.ToArray()));
    }
}
