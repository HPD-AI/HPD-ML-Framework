#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Compose fixed transforms left to right.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle raw = InMemoryDataHandle.FromColumns(
    ("Age", new[] { 25f, float.NaN, 40f }),
    ("City", new[] { "Chicago", "Austin", "Chicago" }));

ITransform preparation = TransformComposer.Compose(
    ITransform.IndicateMissing("Age", "AgeWasMissing"),
    ITransform.ReplaceMissing("Age", ReplacementValue.Constant(0f)),
    ITransform.Hash("City", numBits: 8, outputColumn: "CityHash"));

IDataHandle prepared = preparation.Apply(raw);

using var rows = prepared.GetCursor(
    ["Age", "AgeWasMissing", "City", "CityHash"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"Age={rows.Current.GetValue<float>("Age")}, " +
        $"Missing={rows.Current.GetValue<bool>("AgeWasMissing")}, " +
        $"City={rows.Current.GetValue<string>("City")}, " +
        $"Hash={rows.Current.GetValue<uint>("CityHash")}");
}
