#:package HPD-ML-Core@0.5.0
#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Train SSA on a recurring signal, then score a later spike.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.TimeSeries;

float[] trainingValues = Enumerable.Range(0, 60)
    .Select(i => (float)Math.Sin(2 * Math.PI * i / 12))
    .ToArray();

IDataHandle training = InMemoryDataHandle.FromColumns(("Value", trainingValues));

ILearner learner = ILearner.SsaSpikeDetection(
    options: new SsaAnomalyOptions
    {
        WindowSize = 8,
        ErrorFunction = ErrorFunction.AbsoluteDifference,
        Alerting = AlertingMode.RawScore,
        Threshold = 2
    });

IModel model = learner.Fit(new LearnerInput(training));

float[] observed = Enumerable.Range(60, 20)
    .Select(i => i == 74 ? 8f : (float)Math.Sin(2 * Math.PI * i / 12))
    .ToArray();

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Step", Enumerable.Range(60, observed.Length).ToArray()),
    ("Value", observed));

IDataHandle output = model.Transform.Apply(input);
using var rows = output.GetCursor(["Step", "Value", "RawScore", "Alert"]);
while (rows.MoveNext())
{
    int step = rows.Current.GetValue<int>("Step");
    float score = rows.Current.GetValue<float>("RawScore");
    bool alert = rows.Current.GetValue<bool>("Alert");
    if (step < 68 || alert)
        Console.WriteLine($"t={step}: raw-score={score:F3}, alert={alert}");
}

Console.WriteLine("The first WindowSize rows are 0.5.0 warm-up placeholders.");
