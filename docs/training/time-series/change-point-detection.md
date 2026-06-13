# Change-Point Detection

Change-point detectors accumulate repeated low-p-value evidence in a
martingale. IID mode consumes an existing score stream; SSA mode first
computes prediction errors from a learned recurrence.

## IID change points

```csharp
var detector = new IidChangePointDetector(
    alerting: AlertingMode.RawScore,
    threshold: 10);
```

Raw-score alerting is the most transparent published `0.5.0` mode. Default
configuration instead uses power-martingale alerting with threshold 100.

## SSA change points

```csharp
ILearner learner = ILearner.SsaChangePointDetection(
    options: new SsaAnomalyOptions
    {
        WindowSize = 8,
        Alerting = AlertingMode.RawScore,
        Threshold = 3
    });
```

The extension creates an SSA anomaly learner configured for change-point
output.

## Reset behavior

On an alert, `0.5.0` resets `LogMartingale` immediately. The emitted
`MartingaleScore` is calculated after that reset, so an alert row reports
`1`, not the value that triggered it.

Comments call this a cooldown, but there is no cooldown counter. A later row
can alert as soon as the threshold is reached again.

The current power and mixture update formulas are not certified betting
martingales. Correct formulas, trigger-score output, bounded evidence, and a
real optional cooldown are planned for `0.6.0`.

## Run the recipes

```bash
dotnet run cookbook/TimeSeries/02-iid-change-point-detection.cs
dotnet run cookbook/TimeSeries/04-ssa-change-point-detection.cs
```

