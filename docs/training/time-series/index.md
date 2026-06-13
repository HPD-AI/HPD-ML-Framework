# Time Series

Time-series transforms process rows in cursor order and carry mutable inference
state from one row to the next. Published `0.5.0` provides anomaly detection,
change-point detection, SSA forecasting, seasonality detection, and seasonal
decomposition over scalar `float` observations.

Install:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-TimeSeries
```

Import `HPD.ML.TimeSeries` for the SSA learner extensions and concrete
detectors.

## Learning path

1. [Ordered data and state](ordered-data-and-state.md)
2. [Choosing an approach](choosing-an-approach.md)
3. [IID anomaly detection](iid-anomaly-detection.md)
4. [SSA anomaly detection](ssa-anomaly-detection.md)
5. [Change-point detection](change-point-detection.md)
6. [Spectral-residual detection](spectral-residual-detection.md)
7. [SSA forecasting](ssa-forecasting.md)
8. [Seasonality and decomposition](seasonality-and-decomposition.md)
9. [Evaluation and monitoring](evaluation-and-monitoring.md)
10. [Training progress](training-progress.md)
11. [Troubleshooting](troubleshooting.md)

## Current surface

| Component | Learns parameters | Primary output |
| --- | --- | --- |
| `IidSpikeDetector` | No | p-value or raw-score alert |
| `IidChangePointDetector` | No | martingale or raw-score alert |
| SSA spike learner | Yes | prediction-error anomaly |
| SSA change-point learner | Yes | persistent-change alert |
| `SpectralResidualDetector` | No | frequency-domain anomaly score |
| SSA forecasting learner | Yes | forecast and bound vectors |
| `SeasonalityDetector` | No | dominant period or `-1` |
| `StlDecomposition` | No | seasonal, trend, and residual arrays |

The guides and recipes describe verified published `0.5.0` behavior. Planned
state persistence, statistical corrections, explicit readiness, and forecast
uncertainty changes are defined separately in the Time Series `0.6.0`
proposal.

