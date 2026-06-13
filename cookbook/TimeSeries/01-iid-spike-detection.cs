#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Apply a transparent raw-score threshold to an existing anomaly-score stream.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] scores = [0.2f, -0.1f, 0.3f, 0.0f, 7.5f, 0.1f, -0.2f];
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(0, scores.Length).ToArray()),
    ("Value", scores));

var detector = new IidSpikeDetector(
    alerting: AlertingMode.RawScore,
    threshold: 5,
    scoreWindowSize: 20);

IDataHandle output = detector.Apply(data);
using var rows = output.GetCursor(["Step", "Value", "Alert", "RawScore", "PValue"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"t={rows.Current.GetValue<int>("Step")}: " +
        $"score={rows.Current.GetValue<float>("RawScore"),5:F1}, " +
        $"p={rows.Current.GetValue<float>("PValue"):F3}, " +
        $"alert={rows.Current.GetValue<bool>("Alert")}");
}

