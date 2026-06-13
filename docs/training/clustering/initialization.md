# Initialization

Both learners support the same initial-centroid strategies:

| Strategy | Behavior | Use |
| --- | --- | --- |
| `KMeansParallel` | K-means|| oversampling followed by K-means++ reduction | Published default |
| `KMeansPlusPlus` | Sequential squared-distance weighted selection | Strong general baseline |
| `Random` | Selects distinct row indexes with a partial shuffle | Fast comparison baseline |

Set the strategy and seed through the options record:

```csharp
var options = new KMeansOptions
{
    NumberOfClusters = 4,
    Initialization = KMeansInitialization.KMeansPlusPlus,
    Seed = 42
};
```

The seed controls initialization and, for mini-batch K-means, batch sampling.
Two separate learner instances with the same data, options, row order, and
seed produce repeatable centroids in published `0.5.0`.

K-means++ and K-means|| can select duplicate coordinates when input rows are
duplicates or all remaining squared-distance weights are zero. Empty clusters
retain their previous centroid rather than being reseeded.

Initialization changes cluster numbering. Treat `PredictedLabel` as an
arbitrary cluster identifier, not as an ordered category or stable semantic
name across independently trained models.

## Run the recipe

```bash
dotnet run cookbook/Clustering/03-initialization-and-seeds.cs
```

## Next

- [Prediction and model inspection](prediction-and-model-inspection.md)
- [Troubleshooting](troubleshooting.md)
