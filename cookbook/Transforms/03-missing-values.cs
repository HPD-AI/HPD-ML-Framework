#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Learn a replacement, preserve an indicator, and drop incomplete rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Temperature", new[] { 10f, float.NaN, 20f, 30f }));

IModel replacementModel = ILearner.ReplaceMissing(
    "Temperature",
    ReplacementStrategy.Mean).Fit(new LearnerInput(training));

var parameters = (MissingValueParameters)replacementModel.Parameters;
Console.WriteLine($"Learned mean replacement: {parameters.Replacement}");

ITransform preparation = TransformComposer.Compose(
    ITransform.IndicateMissing("Temperature"),
    replacementModel.Transform);

IDataHandle completed = preparation.Apply(training);
using var rows = completed.GetCursor(
    ["Temperature", "Temperature_Missing"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<float>("Temperature")}, " +
        $"was missing={rows.Current.GetValue<bool>("Temperature_Missing")}");
}

IDataHandle onlyComplete = ITransform.DropMissing("Temperature").Apply(training);
using var completeRows = onlyComplete.GetCursor(["Temperature"]);
var count = 0;
while (completeRows.MoveNext())
    count++;

Console.WriteLine($"Rows after DropMissing: {count}");

