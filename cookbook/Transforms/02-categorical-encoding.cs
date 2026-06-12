#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Learn categories from training rows and reuse the mapping.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("City", new[] { "Chicago", "Austin", "Chicago", "Seattle" }));

ILearner learner = ILearner.OneHotEncode(
    "City",
    outputColumn: "CityFeatures");

IModel model = learner.Fit(new LearnerInput(training));
var parameters = (OneHotParameters)model.Parameters;

Console.WriteLine("Learned mapping:");
foreach (var item in parameters.KeyMapping.OrderBy(item => item.Value))
    Console.WriteLine($"{item.Value}: {item.Key}");

IDataHandle scoring = InMemoryDataHandle.FromColumns(
    ("City", new[] { "Austin", "Boston" }));

IDataHandle encoded = model.Transform.Apply(scoring);

using var rows = encoded.GetCursor(["City", "CityFeatures"]);
while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("City")} -> " +
        $"[{string.Join(", ", rows.Current.GetValue<float[]>("CityFeatures"))}]");
}

