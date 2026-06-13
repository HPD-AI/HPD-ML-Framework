# Mini-Batch K-Means

`MiniBatchKMeansLearner` samples a batch of row indexes with replacement on
each iteration. Assigned samples update their centroid incrementally with a
learning rate of `1 / observations seen by that centroid`.

```csharp
ILearner learner = ILearner.MiniBatchKMeans(
    options: new MiniBatchKMeansOptions
    {
        NumberOfClusters = 3,
        BatchSize = 64,
        MaxIterations = 200,
        ConvergenceTolerance = 1e-4f,
        Seed = 42
    });
```

## Options

| Option | `0.5.0` default | Meaning |
| --- | ---: | --- |
| `NumberOfClusters` | `5` | Number of centroids |
| `MaxIterations` | `100` | Maximum sampled batches |
| `BatchSize` | `1000` | Samples drawn per iteration |
| `ConvergenceTolerance` | `1e-4` | Relative change threshold for batch average squared distance |
| `Initialization` | `KMeansParallel` | Initial centroid strategy |
| `Seed` | `null` | Random seed; set it for repeatability |

`BatchSize` is clamped to the total row count. Sampling is still with
replacement, so a batch equal to the row count is not guaranteed to contain
every row.

## Memory behavior

Published `0.5.0` materializes every feature vector before initialization and
sampling. Mini-batch K-means lowers distance work per iteration; it does not
support streaming input or datasets larger than memory. Calling it
"out-of-core" would be inaccurate for this release.

True bounded-memory training, or documentation that formally keeps this
learner in-memory, should be decided for `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/Clustering/02-mini-batch-k-means.cs
```

## Next

- [Choose a learner](choosing-a-learner.md)
- [Training progress](training-progress.md)
