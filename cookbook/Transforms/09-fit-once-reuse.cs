#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Fit preparation on training rows and reuse it unchanged on test rows.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Age", new[] { 20f, 40f, 60f }),
    ("City", new[] { "Chicago", "Austin", "Chicago" }));

IDataHandle test = InMemoryDataHandle.FromColumns(
    ("Age", new[] { 30f, 80f }),
    ("City", new[] { "Austin", "Seattle" }));

IModel ageModel = ILearner.MinMaxNormalize(
    "Age", outputColumn: "AgeScaled").Fit(new LearnerInput(training));

IModel cityModel = ILearner.OneHotEncode(
    "City", outputColumn: "CityFeatures").Fit(new LearnerInput(training));

ITransform preparation = TransformComposer.Compose(
    ageModel.Transform,
    cityModel.Transform);

IDataHandle preparedTest = preparation.Apply(test);
using var rows = preparedTest.GetCursor(
    ["Age", "AgeScaled", "City", "CityFeatures"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("City")}, " +
        $"age={rows.Current.GetValue<float>("Age")}, " +
        $"scaled={rows.Current.GetValue<float>("AgeScaled"):F2}, " +
        $"city=[{string.Join(", ", rows.Current.GetValue<float[]>("CityFeatures"))}]");
}

