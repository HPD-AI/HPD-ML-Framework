#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

// Inspect objective-specific output contracts without invoking native training.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.LightGBM;

ISchema input = new SchemaBuilder()
    .AddVectorColumn<float>("Features", 4)
    .AddColumn<float>("Label")
    .Build();

var learners = new (string Name, ILearner Learner)[]
{
    ("Regression", ILearner.LightGbmRegression()),
    ("Binary", ILearner.LightGbmBinaryClassification()),
    ("Multiclass", ILearner.LightGbmMulticlass(numberOfClasses: 3)),
    ("Ranking", ILearner.LightGbmRanking())
};

foreach (var (name, learner) in learners)
{
    ISchema output = learner.GetOutputSchema(input);
    string added = string.Join(
        ", ",
        output.Columns
            .Where(column => input.FindByName(column.Name) is null)
            .Select(column => column.Name));

    Console.WriteLine($"{name}: {added}");
}
