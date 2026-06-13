#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Train a forecast model, score a live continuation, and monitor raw errors.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

static float Signal(int i) =>
    (float)(50 + 0.1 * i + 3 * Math.Sin(2 * Math.PI * i / 12));

float[] history = Enumerable.Range(0, 72).Select(Signal).ToArray();
IDataHandle training = InMemoryDataHandle.FromColumns(("Value", history));

ILearner forecastLearner = ILearner.SsaForecasting(
    options: new SsaForecastOptions
    {
        WindowSize = 12,
        Horizon = 1,
        ConfidenceLevel = 0.95f
    });

IModel forecastModel = forecastLearner.Fit(new LearnerInput(training));

float[] live = Enumerable.Range(72, 24)
    .Select(i => i == 90 ? Signal(i) + 15f : Signal(i))
    .ToArray();

IDataHandle liveData = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(72, live.Length).ToArray()),
    ("Value", live));

IDataHandle forecasts = forecastModel.Transform.Apply(liveData);

var errors = new List<float>();
using (var rows = forecasts.GetCursor(["Step", "Value", "Forecast"]))
{
    float? previousForecast = null;
    while (rows.MoveNext())
    {
        int step = rows.Current.GetValue<int>("Step");
        float actual = rows.Current.GetValue<float>("Value");

        if (previousForecast is float predicted && step >= 84)
        {
            float error = actual - predicted;
            errors.Add(error);
            Console.WriteLine(
                $"t={step}: actual={actual:F2}, predicted={predicted:F2}, error={error:F2}");
        }

        previousForecast = rows.Current.GetValue<float[]>("Forecast")[0];
    }
}

IDataHandle errorData = InMemoryDataHandle.FromColumns(("Value", errors.ToArray()));
var errorDetector = new IidSpikeDetector(
    alerting: AlertingMode.RawScore,
    threshold: 8);

IDataHandle alerts = errorDetector.Apply(errorData);
using var alertRows = alerts.GetCursor(["RawScore", "Alert"]);
int errorIndex = 0;
while (alertRows.MoveNext())
{
    if (alertRows.Current.GetValue<bool>("Alert"))
    {
        Console.WriteLine(
            $"forecast-error alert at evaluated row {errorIndex}: " +
            $"{alertRows.Current.GetValue<float>("RawScore"):F2}");
    }
    errorIndex++;
}

