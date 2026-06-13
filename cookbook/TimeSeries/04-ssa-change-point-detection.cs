#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Train a baseline recurrence and detect a later persistent level shift.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] baseline = Enumerable.Range(0, 60)
    .Select(i => (float)Math.Sin(2 * Math.PI * i / 12))
    .ToArray();

IDataHandle training = InMemoryDataHandle.FromColumns(("Value", baseline));

ILearner learner = ILearner.SsaChangePointDetection(
    options: new SsaAnomalyOptions
    {
        WindowSize = 8,
        ErrorFunction = ErrorFunction.AbsoluteDifference,
        Alerting = AlertingMode.RawScore,
        Threshold = 2
    });

IModel model = learner.Fit(new LearnerInput(training));

float[] shifted = Enumerable.Range(60, 24)
    .Select(i =>
    {
        float seasonal = (float)Math.Sin(2 * Math.PI * i / 12);
        return i >= 74 ? seasonal + 6f : seasonal;
    })
    .ToArray();

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(60, shifted.Length).ToArray()),
    ("Value", shifted));

IDataHandle output = model.Transform.Apply(input);
using var rows = output.GetCursor(["Step", "RawScore", "MartingaleScore", "Alert"]);
while (rows.MoveNext())
{
    if (!rows.Current.GetValue<bool>("Alert"))
        continue;

    Console.WriteLine(
        $"alert at t={rows.Current.GetValue<int>("Step")}: " +
        $"raw={rows.Current.GetValue<float>("RawScore"):F3}, " +
        $"reported-martingale={rows.Current.GetValue<float>("MartingaleScore"):F3}");
}

