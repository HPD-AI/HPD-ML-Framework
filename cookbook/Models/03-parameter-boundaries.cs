#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Clustering@0.5.0
#:property TargetFramework=net10.0

// Contrast copied linear weights with mutable clustering storage.

using HPD.ML.BinaryClassification;
using HPD.ML.Clustering;

double[] sourceWeights = [1.0, 2.0];
var linear = new LinearModelParameters(sourceWeights, bias: 0.5);
sourceWeights[0] = 99.0;

Console.WriteLine(
    $"Linear constructor copied input: {linear.Weights[0] == 1.0}");

float[] sourceCentroids = [0f, 0f, 10f, 10f];
var clustering = new ClusteringModelParameters(
    k: 2,
    dimensionality: 2,
    centroids: sourceCentroids);

float before = clustering.DistanceSquared([0f, 0f], 0);
clustering.Centroids[0] = 100f;
float after = clustering.DistanceSquared([0f, 0f], 0);

Console.WriteLine($"Centroid[0] after mutation: {clustering.Centroids[0]:F1}");
Console.WriteLine($"Cached norm after mutation: {clustering.CentroidNormsSquared[0]:F1}");
Console.WriteLine($"Distance before mutation:   {before:F1}");
Console.WriteLine($"Distance after mutation:    {after:F1}");
Console.WriteLine(
    "CentroidNormsSquared was not recomputed; treat parameter arrays as read-only.");
