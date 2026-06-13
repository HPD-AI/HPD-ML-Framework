# Prediction and Model Inspection

Apply the fitted transform to rows containing the same feature column and
vector dimensionality used for training:

```csharp
IDataHandle predictions = model.Transform.Apply(testData);
```

The transform preserves input columns and appends:

| Column | Type | Meaning |
| --- | --- | --- |
| `PredictedLabel` | `uint` | One-based index of the nearest centroid |
| `Score` | `float[]` | Squared Euclidean distance to each centroid |

`Score[0]` belongs to label `1`, `Score[1]` belongs to label `2`, and so on.
The minimum score determines `PredictedLabel`. These values are squared
distances, despite the shorter word "distance" in some API comments.

Inspect learned centroids through the model parameters:

```csharp
var parameters = (ClusteringModelParameters)model.Parameters;

for (int cluster = 0; cluster < parameters.K; cluster++)
{
    ReadOnlySpan<float> centroid = parameters.GetCentroid(cluster);
    Console.WriteLine($"Label {cluster + 1}: {string.Join(", ", centroid.ToArray())}");
}
```

`NearestCluster` returns a zero-based cluster index and squared distance,
while scored `PredictedLabel` is one-based.

The model parameter constructor and public centroid array do not enforce
immutability or validate dimensions. Treat `Centroids` as read-only after
construction; mutating it also leaves the precomputed centroid norms stale.

## Run the recipes

```bash
dotnet run cookbook/Clustering/05-inspect-centroids.cs
dotnet run cookbook/Clustering/06-prediction-distances.cs
```

## Next

- [Evaluation](evaluation.md)
- [Troubleshooting](troubleshooting.md)
