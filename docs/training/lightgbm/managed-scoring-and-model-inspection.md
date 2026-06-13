# Managed Scoring and Model Inspection

After native training, HPD ML exports the booster text, parses it, disposes the
native booster, and returns a managed model.

```text
TreeEnsembleParameters
    Ensemble
    FeatureImportance

TreeEnsemble
    Trees
    Bias
    NumberOfClasses
```

## Output columns

| Mode | Output |
| --- | --- |
| Regression and ranking | scalar `float Score` |
| Binary classification | scalar `float Score`, scalar `float Probability`, bool `PredictedLabel` |
| Multiclass | vector `float[] Score`, `uint PredictedLabel` |

Input columns and row count are preserved. New output columns replace existing
columns with the same names because schema merge uses last-writer-wins.

## Tree traversal

Each `RegressionTree` stores parallel arrays for split features, thresholds,
children, categorical sets, and leaf values. Numerical values go left when:

```text
feature <= threshold
```

Categorical values are cast to `int` and tested for set membership.

If a split references an index beyond the supplied prediction vector, the
scorer substitutes zero. This can hide feature-shape mistakes. Always use the
same exact vector width used for training.

## Fidelity limitations

The parser does not preserve the complete LightGBM decision contract:

- missing-value type;
- default-left/default-right routing;
- all decision-type flags;
- a certified model base score;
- objective-specific output transformations beyond HPD's managed logic.

There are no tests comparing predictions from a real native booster with the
parsed managed ensemble. Missing values, categorical splits, Poisson, Tweedie,
ranking, and multiclass behavior are especially important unverified areas.

Managed scoring should therefore be considered an implementation preview, not
a certified replacement for native LightGBM prediction.

## Managed-only demonstration

The public tree classes can demonstrate scoring without native training. This
complete file-based example requires only the published packages:

```csharp
#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.LightGBM;

var tree = new RegressionTree(
    numLeaves: 2,
    splitFeatures: [0],
    thresholds: [5.0],
    leftChild: [~0],
    rightChild: [~1],
    isCategoricalSplit: [false],
    categoricalValues: [null],
    leafValues: [10.0, 20.0]);

var ensemble = new TreeEnsemble([tree], bias: 1.0);
ITransform scorer = new TreeEnsembleScoringTransform(
    ensemble,
    featureColumn: "Features",
    mode: TreeEnsembleScoringTransform.OutputMode.Regression);

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [3f], [7f] }));

IDataHandle scored = scorer.Apply(input);
using var rows = scored.GetCursor(["Features", "Score"]);

while (rows.MoveNext())
{
    float x = rows.Current.GetValue<float[]>("Features")[0];
    float score = rows.Current.GetValue<float>("Score");
    Console.WriteLine($"x={x:F1}, score={score:F1}");
}
```

The example constructs a synthetic tree. It proves the managed transform runs;
it does not prove native model parsing fidelity.

## Model persistence

`TreeEnsembleParameters` implements `ILearnedParameters`, but published 0.5.0
has no end-to-end test showing that a trained LightGBM model survives the
supported serializer and scores identically after loading.

Do not promise native-free deployment of persisted models until that complete
round trip is package-certified.

## Next

- [Training runtime](training-runtime.md)
- [Troubleshooting](troubleshooting.md)
