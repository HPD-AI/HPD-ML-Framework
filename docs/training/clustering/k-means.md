# Batch K-Means

`KMeansLearner` implements Lloyd's algorithm. Each iteration assigns every row
to its nearest centroid and then recomputes centroids from those assignments.

```csharp
ILearner learner = ILearner.KMeans(
    options: new KMeansOptions
    {
        NumberOfClusters = 3,
        MaxIterations = 200,
        ConvergenceTolerance = 1e-6f,
        Initialization = KMeansInitialization.KMeansPlusPlus,
        Seed = 42
    });

IModel model = learner.Fit(new LearnerInput(training));
```

## Options

| Option | `0.5.0` default | Meaning |
| --- | ---: | --- |
| `NumberOfClusters` | `5` | Number of centroids |
| `MaxIterations` | `1000` | Maximum assignment iterations |
| `ConvergenceTolerance` | `1e-7` | Relative change threshold for average squared distance |
| `Initialization` | `KMeansParallel` | Initial centroid strategy |
| `Seed` | `null` | Random seed; set it for repeatability |

The learner can stop when no assignment changes or when the reported average
squared distance changes by less than the relative tolerance.

## Published early-stop defect

Assignments begin as cluster index zero. If every row is assigned to cluster
zero on the first assignment step, the learner stops before recomputing any
centroid. This always affects `NumberOfClusters = 1` and can affect other
degenerate initializations. The returned centroid can therefore remain an
initially selected row instead of the data mean.

Correct first-iteration assignment tracking is a recommended `0.6.0`
correction. Do not rely on published `0.5.0` batch K-means for a one-cluster
mean.

## Run the recipe

```bash
dotnet run cookbook/Clustering/01-k-means.cs
```

## Next

- [Initialization](initialization.md)
- [Prediction and model inspection](prediction-and-model-inspection.md)
