# Troubleshooting

## Alerts differ after splitting a series

Each `Apply` call and cursor starts fresh state. Process one logical series
through one cursor, or call `InitializeState` and `ProcessRow` directly while
keeping the returned state in memory.

## Early values look valid but are all zero

Those rows may be warm-up placeholders. SSA and spectral transforms have no
`IsReady` output in `0.5.0`; exclude the documented warm-up range.

## IID detection does not learn a baseline

IID input is already a raw anomaly score. Use SSA when the package should learn
a recurring signal and score prediction errors.

## Change-point alert has martingale score 1

Published change-point detectors reset the martingale before writing output.
The triggering value is lost from that row. This is a known `0.5.0` defect.

## Confidence intervals seem unrelated to noise

Forecast residual variance always falls back to `1.0` in `0.5.0`. The bounds
are not calibrated from training or inference residuals.

## A window is larger than requested

Spectral residual rounds the requested window upward to a power of two.
Inspect `detector.InitializeState().Window.Capacity` to see the effective
window.

## SSA fitting throws an index error

Check `WindowSize`, `SeriesLength`, and manual `Rank`. Keep the effective
series length at least `WindowSize + 1` and rank below the window size.

## Training does not cancel promptly

Active SSA work does not observe cancellation in `0.5.0`. Use bounded input
and window sizes while developing.

## State cannot be saved

Every built-in `StateSerializer` is `null`. Persist the source position and
replay enough ordered history after restart, or wait for the proposed `0.6.0`
checkpoint contract.

## P-values or martingales are used for production decisions

The published formulas are not yet statistically certified. Validate against
your own event labels or use an explicit raw-score threshold with understood
units.

