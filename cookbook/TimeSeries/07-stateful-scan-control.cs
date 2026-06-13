#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Keep detector state explicitly across two batches, then contrast a fresh state.

using HPD.ML.Core;
using HPD.ML.TimeSeries;

var schema = new Schema([
    new Column("Value", FieldType.Scalar<float>())
]);

var detector = new IidSpikeDetector(
    alerting: AlertingMode.RawScore,
    threshold: 5,
    scoreWindowSize: 10);

IidAnomalyState continued = detector.InitializeState();

ProcessBatch("batch A", [0.1f, 0.2f, -0.1f], ref continued);
ProcessBatch("batch B", [0.0f, 8.0f], ref continued);

IidAnomalyState fresh = detector.InitializeState();
ProcessBatch("fresh B", [0.0f, 8.0f], ref fresh);

Console.WriteLine($"Continued rows: {continued.RowCount}; fresh rows: {fresh.RowCount}");
Console.WriteLine($"Checkpoint serializer available: {detector.StateSerializer is not null}");

void ProcessBatch(string name, float[] values, ref IidAnomalyState state)
{
    foreach (float value in values)
    {
        var row = new DictionaryRow(
            schema,
            new Dictionary<string, object> { ["Value"] = value });

        var result = detector.ProcessRow(state, row);
        state = result.NextState;

        Console.WriteLine(
            $"{name}: value={value,4:F1}, " +
            $"p={result.Output.GetValue<float>("PValue"):F3}, " +
            $"alert={result.Output.GetValue<bool>("Alert")}");
    }
}

