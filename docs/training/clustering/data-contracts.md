# Data Contracts

Both clustering learners use this default input:

```text
Features   float[]
```

Clustering is unsupervised, so fitting does not require a label column.
Custom feature names are supplied when constructing the learner:

```csharp
ILearner learner = ILearner.KMeans(
    featureColumn: "Signals",
    options: new KMeansOptions { NumberOfClusters = 3 });
```

## Features

Use dense vectors with the same nonzero length:

```csharp
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [0.1f, 0.2f],
        [0.2f, 0.1f],
        [8.9f, 9.1f]
    }));
```

Published `0.5.0` contains a scalar `float` fallback, but its initial
`float[]` probe can throw or reinterpret scalar values with current row
implementations. Use one-element `float[]` values for a single feature.

The first row determines dimensionality. During training, values beyond that
length in later rows are ignored, while shorter rows can fail with an index
error. During prediction, a longer vector can fail and a shorter vector can
produce an invalid distance because the full centroid norm is still included.

Use finite values. `NaN` and infinity are not rejected before distance
calculations and can produce meaningless assignments or centroids.

## Training size

The number of rows must be at least `NumberOfClusters`. Both learners copy all
feature vectors into memory. Published mini-batch K-means reduces work per
iteration, but it is not a streaming or out-of-core trainer.

## Current validation gaps

Published `0.5.0` does not consistently validate:

- positive cluster, iteration, and batch-size values;
- finite non-negative tolerances;
- empty or mixed-length vectors;
- null or non-finite features;
- whether every requested cluster is represented by a distinct point.

Explicit validation and stable scalar handling are recommended `0.6.0`
corrections.

## Run the recipe

```bash
dotnet run cookbook/Clustering/04-custom-feature-column.cs
```

## Next

- [Choose a learner](choosing-a-learner.md)
- [Prepare features](../../getting-started/preparing-data.md)
