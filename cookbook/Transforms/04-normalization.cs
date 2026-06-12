#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Fit three numeric preparation strategies from the same training column.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Value", new[] { 10f, 20f, 30f, 40f }));

IModel minMax = ILearner.MinMaxNormalize(
    "Value", outputColumn: "Scaled").Fit(new LearnerInput(training));

IModel zScore = ILearner.MeanVarianceNormalize(
    "Value", outputColumn: "ZScore").Fit(new LearnerInput(training));

IModel bins = ILearner.BinNormalize(
    "Value", numBins: 2, outputColumn: "Bin").Fit(new LearnerInput(training));

ITransform preparation = TransformComposer.Compose(
    minMax.Transform,
    zScore.Transform,
    bins.Transform);

IDataHandle prepared = preparation.Apply(training);
using var rows = prepared.GetCursor(["Value", "Scaled", "ZScore", "Bin"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<float>("Value"),5:F1} -> " +
        $"scaled={rows.Current.GetValue<float>("Scaled"):F2}, " +
        $"z={rows.Current.GetValue<float>("ZScore"):F2}, " +
        $"bin={rows.Current.GetValue<float>("Bin"):F2}");
}

