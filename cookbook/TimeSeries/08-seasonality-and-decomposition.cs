#:package HPD-ML-TimeSeries@0.5.0
#:property TargetFramework=net10.0

// Detect a repeating period and decompose the series.

using HPD.ML.TimeSeries;

double[] values = Enumerable.Range(0, 72)
    .Select(i =>
    {
        double trend = 0.03 * i;
        double seasonal = new[] { -2.0, -1.0, 1.0, 2.0, 1.0, -1.0 }[i % 6];
        return 20 + trend + seasonal;
    })
    .ToArray();

int period = SeasonalityDetector.DetectPeriod(values, maxLag: 24);
Console.WriteLine($"Detected period: {period}");

var (seasonal, trend, residual) =
    StlDecomposition.Decompose(values, period: period > 0 ? period : 6);

double maxReconstructionError = values
    .Select((value, i) => Math.Abs(value - seasonal[i] - trend[i] - residual[i]))
    .Max();

Console.WriteLine($"Maximum reconstruction error: {maxReconstructionError:E3}");
Console.WriteLine(
    $"Last row: seasonal={seasonal[^1]:F3}, trend={trend[^1]:F3}, residual={residual[^1]:F3}");

