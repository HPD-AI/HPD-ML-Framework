#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Detect a spectral irregularity and inspect the effective FFT window.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] values = Enumerable.Range(0, 80)
    .Select(i => i == 60 ? 8f : (float)Math.Sin(2 * Math.PI * i / 8))
    .ToArray();

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(0, values.Length).ToArray()),
    ("Value", values));

var detector = new SpectralResidualDetector(
    windowSize: 30,
    threshold: 0.5);

Console.WriteLine(
    $"Requested window: 30; effective window: {detector.InitializeState().Window.Capacity}");

IDataHandle output = detector.Apply(data);
using var rows = output.GetCursor(["Step", "RawScore", "Magnitude", "Alert"]);
while (rows.MoveNext())
{
    int step = rows.Current.GetValue<int>("Step");
    bool alert = rows.Current.GetValue<bool>("Alert");
    if (step is 30 or 31 or 32 or 59 or 60 or 61 or 62)
    {
        Console.WriteLine(
            $"t={step}: score={rows.Current.GetValue<float>("RawScore"):F3}, " +
            $"magnitude={rows.Current.GetValue<float>("Magnitude"):F3}, alert={alert}");
    }
}
