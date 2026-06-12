#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Convert a scalar column and hash categories into a bounded uint range.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle raw = InMemoryDataHandle.FromColumns(
    ("Count", new[] { 1, 2, 3 }),
    ("Category", new[] { "red", "green", "blue" }));

ITransform transforms = TransformComposer.Compose(
    ITransform.ConvertType("Count", typeof(float)),
    ITransform.Hash("Category", numBits: 8, outputColumn: "CategoryHash"));

IDataHandle result = transforms.Apply(raw);
using var rows = result.GetCursor(["Count", "Category", "CategoryHash"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"Count={rows.Current.GetValue<float>("Count"):F1}, " +
        $"Category={rows.Current.GetValue<string>("Category")}, " +
        $"Hash={rows.Current.GetValue<uint>("CategoryHash")}");
}

