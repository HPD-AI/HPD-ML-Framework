#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

// Exercise the public managed tree scorer without native LightGBM training.

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

Console.WriteLine("Synthetic managed scoring only; no native model was trained.");
