# Evaluation and Monitoring

Published `0.5.0` has no dedicated time-series evaluation transform. Evaluate
ordered output manually and keep temporal alignment explicit.

## Forecast alignment

A forecast emitted after consuming row `t` predicts:

```text
Forecast[0] -> row t + 1
Forecast[1] -> row t + 2
...
```

Exclude warm-up rows before computing error. For each horizon slot, compare
only rows that have a corresponding future observation. Useful measures
include MAE, RMSE, interval coverage, and mean interval width.

Do not treat the `0.5.0` confidence bounds as calibrated coverage intervals;
their residual variance is the fixed fallback described in
[SSA forecasting](ssa-forecasting.md).

## Anomaly evaluation

For isolated spikes, row-level precision and recall can be useful. For change
points, evaluate events instead:

- whether an alert occurs within an allowed delay;
- detection delay from the event start;
- duplicate alerts for one event;
- false alerts outside event windows.

Raw thresholds and p-value thresholds are not interchangeable. Record the
alerting mode and all options with evaluation results.

## Operational monitoring

Track:

- current row count and warm-up status;
- source ordering and missing intervals;
- alert rate and score distribution;
- model training range and inference range;
- detector restarts;
- forecast error by horizon.

Because state is cursor-local and not serializable in `0.5.0`, a process or
batch restart repeats warm-up. Surface that reset in application telemetry.

## Run the workflow

```bash
dotnet run cookbook/TimeSeries/09-complete-monitoring-workflow.cs
```

