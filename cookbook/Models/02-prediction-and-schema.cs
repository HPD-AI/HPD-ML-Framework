#:package HPD-ML-Core@0.5.0
#:package HPD-ML-BinaryClassification@0.5.0
#:package HPD-ML-Regression@0.5.0
#:property TargetFramework=net10.0

// Inspect a model output schema without reading rows, then score lazily.

using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;
using HPD.ML.Core;
using HPD.ML.Regression;

var parameters = new LinearModelParameters([2.0, -1.0], bias: 0.5);
IModel model = new Model(
    new RegressionScoringTransform(parameters),
    parameters);

IDataHandle input = InMemoryDataHandle.FromColumns(
    ("RequestId", new[] { "a-1", "a-2" }),
    ("Features", new float[][] { [3f, 1f], [1f, 4f] }));

ISchema output = model.Transform.GetOutputSchema(input.Schema);
Console.WriteLine("Output schema:");
foreach (var column in output.Columns)
{
    Console.WriteLine(
        $"  {column.Name}: {column.Type.ClrType.Name}, vector={column.Type.IsVector}");
}

IDataHandle predictions = model.Transform.Apply(input);
Console.WriteLine($"Rows are consumed here:");
using var rows = predictions.GetCursor(["RequestId", "Score"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("RequestId")}: " +
        $"{rows.Current.GetValue<float>("Score"):F2}");
}
