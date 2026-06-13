# Choosing a Learner

Start with batch K-means. Use mini-batch K-means when reducing per-iteration
work matters more than obtaining the batch learner's centroid updates.

| Requirement | Recommended learner |
| --- | --- |
| Small or moderate in-memory dataset | K-means |
| Full assignment and centroid update each iteration | K-means |
| Lower work per iteration on a large in-memory dataset | Mini-batch K-means |
| Reproducible comparison | Either, with `Seed` set |
| Streaming or data larger than memory | Neither in published `0.5.0` |

Both learners:

- use squared Euclidean distance;
- require the number of clusters in advance;
- support K-means++, K-means||, and random initialization;
- keep an existing centroid when no points update that cluster;
- materialize the complete feature column;
- ignore validation data and `LearnerInput.Environment`.

Mini-batch training samples rows with replacement. `BatchSize` is clamped to
the row count when it is larger, but that does not make an iteration a full
pass because duplicate rows can be sampled and others omitted.

Neither learner standardizes feature scales. Normalize inputs when one
measurement's numeric range should not dominate squared distance.

## Run the recipes

```bash
dotnet run cookbook/Clustering/01-k-means.cs
dotnet run cookbook/Clustering/02-mini-batch-k-means.cs
```

## Next

- [Batch K-means](k-means.md)
- [Mini-batch K-means](mini-batch-k-means.md)
- [Initialization](initialization.md)
