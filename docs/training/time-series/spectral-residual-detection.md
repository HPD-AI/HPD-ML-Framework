# Spectral-Residual Detection

`SpectralResidualDetector` identifies frequency-domain irregularities without
training. It maintains a sliding window, computes an FFT, removes a local
average from the log-amplitude spectrum, and scores the newest saliency value.

```csharp
var detector = new SpectralResidualDetector(
    inputColumn: "Value",
    windowSize: 32,
    spectralAveragingWindow: 3,
    saliencyAveragingWindow: 5,
    threshold: 0.3);

IDataHandle output = detector.Apply(data);
```

Output columns:

```text
Alert       bool
RawScore    float
Magnitude   float
```

## Window behavior

The requested window is silently rounded up to the next power of two. For
example, 100 becomes 128. Rows before the effective window fills emit false
alerts and zero scores.

The implementation recomputes the FFT for every later row. Choose the window
with both signal period and per-row cost in mind.

Published `0.5.0` does not consistently validate averaging-window sizes,
thresholds, or finite input. Explicit power-of-two and option validation is
planned for `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/05-spectral-residual-detection.cs
```

