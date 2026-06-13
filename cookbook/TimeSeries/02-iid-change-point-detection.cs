#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Detect a large shift and show the published post-alert martingale reset.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] values =
[
    0.1f, -0.1f, 0.2f, 0.0f, -0.2f,
    0.1f, 0.0f, 25.0f, 24.0f, 26.0f
];

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(0, values.Length).ToArray()),
    ("Value", values));

var detector = new IidChangePointDetector(
    alerting: AlertingMode.RawScore,
    threshold: 10,
    martingale: MartingaleType.Power);

IDataHandle output = detector.Apply(data);
using var rows = output.GetCursor(
    ["Step", "RawScore", "PValue", "MartingaleScore", "Alert"]);
while (rows.MoveNext())
{
    bool alert = rows.Current.GetValue<bool>("Alert");
    Console.WriteLine(
        $"t={rows.Current.GetValue<int>("Step")}: " +
        $"score={rows.Current.GetValue<float>("RawScore"),5:F1}, " +
        $"martingale={rows.Current.GetValue<float>("MartingaleScore"):F3}, " +
        $"alert={alert}");
}

Console.WriteLine(
    "In published 0.5.0, an alert row reports the martingale after reset.");

