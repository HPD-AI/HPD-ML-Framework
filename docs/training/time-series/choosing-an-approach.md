# Choosing an Approach

Choose the component from what your input represents and whether a model must
learn the series pattern.

| Need | Use |
| --- | --- |
| Threshold an existing anomaly-score stream | IID spike detector |
| Accumulate evidence of a persistent shift in scores | IID change-point detector |
| Detect deviations from a learned recurring signal | SSA spike detection |
| Detect a structural shift from a learned signal | SSA change-point detection |
| Detect frequency-domain irregularity without training | Spectral residual |
| Predict several future univariate values | SSA forecasting |
| Estimate a dominant repeating period | Seasonality detector |
| Split a series into seasonal, trend, and residual components | STL decomposition |

## IID is not a signal model

IID detectors treat each input value as the raw anomaly score itself. They do
not estimate a baseline or remove seasonality. Feed them a score stream whose
direction and scale already have useful meaning.

## SSA learns a recurrence

SSA learners materialize the complete training series, construct a trajectory
matrix, decompose its covariance matrix, and derive autoregressive
coefficients. They are appropriate when the series has repeatable structure.

Published `0.5.0` starts inference with an empty observation window rather than
the tail of training data, so every new application warms up again.

## Spectral residual is windowed

Spectral residual performs an FFT whenever its window is full. It requires no
fit, but its requested window is silently rounded upward to the next power of
two. Larger windows need more observations and cost more work per row.

## Current caution

The `0.5.0` p-value and martingale implementations are useful only as package
behavior to inspect; they are not yet certified statistical tests. For
production decisions, validate alerts against your own labeled historical
events and use raw-score alerting when you need transparent thresholds.

