# Troubleshooting

## Fewer rows than clusters

Both learners throw when the row count is less than `NumberOfClusters`.
Reduce the requested cluster count or provide more rows.

## Unexpected cluster numbers

Labels are one-based identifiers, not rankings. Initialization can permute
them between independently trained models. Attach business names only after
inspecting the fitted centroids.

## One cluster does not equal the mean

Published batch K-means can stop before its first centroid update when every
row initially maps to cluster zero. With one requested cluster, use an
explicit mean calculation instead of relying on the `0.5.0` learner.

## Index errors or implausible scores

Verify every feature vector has exactly the same length as the first training
row. Mixed lengths are not validated and produce asymmetric failures during
training and scoring.

## A large-scale feature dominates

Squared Euclidean distance is scale-sensitive. Normalize features before
training when their units or ranges differ.

## Mini-batch training uses substantial memory

Published mini-batch K-means materializes all rows. It reduces computation per
iteration, not storage. It is not suitable for data that cannot fit in memory.

## A metric is unexpectedly zero

The evaluator uses zero as a fallback for unavailable score/features and
several degenerate cases. Confirm all four expected columns exist:
`Label`, `PredictedLabel`, `Score`, and `Features`.

## Training does not stop after cancellation

The active loops do not observe the `FitAsync` token. Cancellation can only
prevent the scheduled task from beginning.

## Invalid options fail unclearly

Use positive values for clusters, iterations, and mini-batch size, plus a
finite non-negative tolerance. Published `0.5.0` lacks complete option
validation.

## Recommended future corrections

A Clustering-specific `0.6.0` proposal has not yet been accepted. Recommended
work includes first-iteration assignment correctness, strict data and option
validation, honest mini-batch memory semantics, active cancellation, and
explicit evaluation contracts for unlabeled and degenerate data.

## Next

- [Data contracts](data-contracts.md)
- [Evaluation](evaluation.md)
