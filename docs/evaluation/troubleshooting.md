# Evaluation Troubleshooting

## Log loss is enormous

The binary evaluator may be consuming raw `Score`. Use a verified probability
column for the combined `0.5.0` evaluator.

## A metric is unexpectedly zero

Check row count and required columns. Zero can represent empty input,
unavailable multiclass scores, missing clustering inputs, or a degenerate
metric case.

## Direct float data gives impossible results

Use `double` scalar columns for direct `0.5.0` metric handles. Float storage
can be reinterpreted during numeric probing.

## BestModel selects a worse model

It always maximizes. Do not use it for loss, error, distance, or DBI.

## Cross-validation looks too optimistic

Learned preparation is not fitted per fold by `featurePipeline`. Fit it only
on each training fold.

## Permutation importance is negative or always zero

Lower-is-better metrics use the wrong subtraction direction, and scalar
value-type columns may not be substituted reliably. Use a higher-is-better
metric and a verified reference/vector feature path.
