#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Learn a TF-IDF vocabulary and apply it to unseen text.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "fast friendly service",
        "friendly helpful support",
        "slow broken service"
    }));

ILearner learner = ILearner.TextFeaturize(
    "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        RemoveStopWords = false,
        NgramMin = 1,
        NgramMax = 1,
        MaxFeatures = 8
    });

IModel model = learner.Fit(new LearnerInput(training));
var parameters = (TextFeaturizeParameters)model.Parameters;

Console.WriteLine($"Vocabulary size: {parameters.FeatureIndex.Count}");
foreach (var term in parameters.FeatureIndex.OrderBy(item => item.Value))
    Console.WriteLine($"{term.Value}: {term.Key}");

IDataHandle scoring = InMemoryDataHandle.FromColumns(
    ("Text", new[] { "friendly service", "completely unknown" }));

IDataHandle result = model.Transform.Apply(scoring);
using var rows = result.GetCursor(["Text", "Features"]);

while (rows.MoveNext())
{
    Console.WriteLine(
        $"{rows.Current.GetValue<string>("Text")} -> " +
        $"[{string.Join(", ", rows.Current.GetValue<float[]>("Features").Select(v => v.ToString("F3")))}]");
}

