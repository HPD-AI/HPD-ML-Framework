# IID Anomaly Detection

IID detectors require no training. The input value is copied to `RawScore`,
then compared directly or passed through the package's history-based p-value
and martingale helpers.

## Spike detection

```csharp
var detector = new IidSpikeDetector(
    inputColumn: "Value",
    alerting: AlertingMode.RawScore,
    threshold: 20,
    side: AnomalySide.TwoSided,
    scoreWindowSize: 50);

IDataHandle output = detector.Apply(data);
```

Output columns:

```text
Alert      bool
RawScore   float
PValue     float
```

Defaults use `PValueScore`, threshold `0.01`, a two-sided test, no martingale,
and a score-history capacity of 100.

## Score direction

`RawScore` alerting always checks `abs(rawScore) >= threshold`; `AnomalySide`
does not affect raw-score alerts. Side selection affects only the computed
p-value.

Published `0.5.0` returns `PValue = 0.5` until two history values exist. The
current implementation uses Gaussian similarity sums rather than a certified
tail-probability estimator.

## Martingale option

`IidSpikeDetector` can update a martingale when configured, but it does not
append a `MartingaleScore` column. Use the change-point detector when that
score must be visible.

The published power and mixture formulas are planned for correction in
`0.6.0`. Do not interpret their values as calibrated evidence without
independent validation.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/01-iid-spike-detection.cs
```

