#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Train an SSA forecaster and print its fixed-horizon vectors.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] trainingValues = Enumerable.Range(0, 72)
    .Select(i => (float)(10 + 0.05 * i + 2 * Math.Sin(2 * Math.PI * i / 12)))
    .ToArray();

IDataHandle training = InMemoryDataHandle.FromColumns(("Value", trainingValues));

ILearner learner = ILearner.SsaForecasting(
    options: new SsaForecastOptions
    {
        WindowSize = 12,
        Horizon = 4,
        ConfidenceLevel = 0.95f
    });

IModel model = learner.Fit(new LearnerInput(training));

float[] continuation = Enumerable.Range(72, 16)
    .Select(i => (float)(10 + 0.05 * i + 2 * Math.Sin(2 * Math.PI * i / 12)))
    .ToArray();

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(72, continuation.Length).ToArray()),
    ("Value", continuation));

IDataHandle output = model.Transform.Apply(input);
using var rows = output.GetCursor(["Step", "Forecast", "LowerBound", "UpperBound"]);
while (rows.MoveNext())
{
    int step = rows.Current.GetValue<int>("Step");
    float[] forecast = rows.Current.GetValue<float[]>("Forecast");

    if (step is 72 or 82 or 83 or 87)
    {
        Console.WriteLine(
            $"after t={step}: [{string.Join(", ", forecast.Select(x => x.ToString("F2")))}]");
    }
}

Console.WriteLine(
    "Rows before the inference window fills contain zero vectors in 0.5.0.");

