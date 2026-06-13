# Learned Parameters

Inspect concrete parameters by casting `model.Parameters` to the type returned
by the selected learner:

```csharp
var parameters =
    (LinearModelParameters)model.Parameters;

Console.WriteLine(parameters.FeatureCount);
Console.WriteLine(parameters.Bias);
```

## 0.5.0 mutability matrix

| Type | Constructor copies input | Public mutation risk |
| --- | --- | --- |
| `LinearModelParameters` | Yes | weights exposed as `IReadOnlyList<double>` |
| `ClusteringModelParameters` | No | centroid and cached-norm arrays are mutable |
| `NeuralNetworkParameters` | Yes | inner weight and bias arrays are mutable |
| `TreeEnsembleParameters` | No defensive copy documented | feature-importance array is mutable |
| `SsaModelParameters` | No | all numeric arrays are mutable |
| `BinParameters` | Record stores array | edge array is mutable |
| `TextFeaturizeParameters` | Record stores collections/array | IDF array is mutable |

Mutation can change predictions. For clustering it can also leave
`CentroidNormsSquared` inconsistent with `Centroids`.

## Operational guidance

- Do not modify arrays returned from fitted parameters.
- Copy values before experimentation or display.
- Validate dimensions and finite values in custom parameter constructors.
- Do not treat `ILearnedParameters` as proof that ZIP round-trip support exists.

The 0.6.0 proposal requires defensive storage and mutation-safe public access.

## Run the recipe

```bash
dotnet run cookbook/Models/03-parameter-boundaries.cs
```
