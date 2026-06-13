# Troubleshooting

## `DllNotFoundException: lib_lightgbm`

This is the expected result from a clean published 0.5.0 installation. The
NuGet package does not contain native runtime assets or depend on a runtime
package.

Use the complete package-only probe in
[Installation and package status](installation-and-status.md#complete-availability-probe).

Supplying your own native library is possible in principle but is not
versioned or certified by the package.

## Learner construction works but fitting fails

Output-schema inspection is entirely managed. Native loading starts only when
the learner creates a dataset. A successful constructor or schema call does
not verify training availability.

## A later row fails during feature copying

All feature vectors must have the same length as the first row. Published
0.5.0 does not validate this clearly. Short rows can fail; long rows are
truncated.

## Double labels produce unexpected training behavior

Use `float` label columns. The intended `double` fallback first probes the
value as `float`; in-memory rows can reinterpret boxed `double` storage rather
than reaching the explicit conversion. This can silently corrupt labels.

## Predictions change when a feature is omitted

The managed scorer substitutes zero for an out-of-range split feature. This is
not shape validation. Supply the exact training vector width.

## Ranking training fails

Ranking expects a `GroupId` column and contiguous rows per group. The group
column name cannot be changed through the public API. Native LightGBM may also
reject relevance values or option combinations that HPD ML does not validate
first.

## Random-forest mode fails

LightGBM random-forest boosting requires compatible bagging and feature
sampling. Published 0.5.0 forwards options without checking the required
combination.

## Early stopping selects a poor model

The implementation assumes lower metric values are better and exports the
last trained state rather than the best iteration. Avoid 0.5.0 early stopping,
especially for ranking metrics.

## Feature importance is null

The learner catches every feature-importance exception and returns `null`.
There is no diagnostic reason in the learned parameters.

## Cancellation does not stop training

The cancellation token only controls task scheduling. Once native training has
started, the loop does not inspect it.

## Should I use this in production?

Not from the published 0.5.0 package. The minimum production gate should
include:

- packaged native runtime assets;
- supported-RID declarations;
- native training integration tests;
- native-versus-managed prediction parity;
- corrected early stopping;
- strict data and option validation;
- persistence certification.

Those items are defined in the LightGBM `0.6.0` completion proposal.
