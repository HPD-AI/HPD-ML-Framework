# Evaluation

Evaluation compares model output with known labels. Evaluate data that was not
used to fit the model so the metrics describe behavior beyond the training
rows.

Install the evaluation package:

```bash
dotnet add package HPD-ML-Evaluation
```

## Score held-out data

Train on one handle and apply the model to a separate test handle:

```csharp
IModel model = await learner.FitAsync(
    new LearnerInput(trainingData));

IDataHandle predictions =
    model.Transform.Apply(testData);
```

The test handle includes labels for evaluation. Production prediction data can
omit labels.

When preparation is learned, fit it on training data and reuse that same
preparation model for the test data.

## Evaluate binary classification

Create and apply a metrics transform:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Evaluation;

ITransform evaluator = ITransform.BinaryClassificationMetrics(
    labelColumn: "Label",
    scoreColumn: "Probability",
    threshold: 0.5);

IDataHandle metrics = evaluator.Apply(predictions);
```

Linear binary classifiers expose both `Score` and `Probability`. Use
`Probability` when the evaluator's threshold and log-loss calculations should
operate on probabilities. Raw `Score` is a margin or logit and is useful for
ranking and diagnostics, but it is not constrained to `[0, 1]`.

The result is a one-row data handle containing:

- `AUC`
- `Accuracy`
- `F1Score`
- `Precision`
- `Recall`
- `LogLoss`
- `LogLossReduction`
- positive and negative precision/recall

Read metrics by name:

```csharp
using var row = metrics.GetCursor(
    ["Accuracy", "AUC", "F1Score", "LogLoss"]);

row.MoveNext();

Console.WriteLine(
    $"Accuracy: {row.Current.GetValue<double>("Accuracy"):P1}");
Console.WriteLine(
    $"AUC: {row.Current.GetValue<double>("AUC"):F3}");
```

Metric columns use `double`.

## Interpret common metrics

`Accuracy` is the fraction of correct thresholded predictions. It can be
misleading when one class dominates.

`Precision` answers how often positive predictions are correct.

`Recall` answers how many actual positives the model finds.

`F1Score` balances precision and recall.

`AUC` measures how well scores rank positive rows above negative rows across
thresholds.

`LogLoss` penalizes incorrect probability estimates, especially confident
incorrect estimates. Lower is better.

Choose metrics that reflect the application's error costs rather than relying
on one universal score.

## Build a confusion matrix

The confusion-matrix transform uses label and predicted-label columns:

```csharp
IDataHandle confusion =
    ITransform.ConfusionMatrix().Apply(predictions);

Console.WriteLine(
    ConfusionMatrixFormatter.Format(confusion));
```

The underlying handle contains one row per observed pair:

- `TrueLabel`
- `PredictedLabel`
- `Count`

The formatter converts those rows into a readable grid.

## Other workload metrics

### Regression

```csharp
IDataHandle metrics = ITransform.RegressionMetrics(
    labelColumn: "Label",
    scoreColumn: "Score",
    featureCount: 4)
    .Apply(predictions);
```

Regression output includes `MAE`, `MSE`, `RMSE`, `RSquared`, and
`AdjustedRSquared`.

### Multiclass classification

```csharp
IDataHandle metrics = ITransform.MulticlassMetrics().Apply(predictions);
```

Multiclass output includes micro accuracy, macro accuracy, log loss, and
log-loss reduction.

### Clustering

```csharp
IDataHandle metrics = ITransform.ClusteringMetrics().Apply(predictions);
```

Clustering output includes normalized mutual information, average assigned
distance, and Davies-Bouldin index. True labels are needed for normalized
mutual information; feature and distance columns support the other metrics.

### Ranking

```csharp
IDataHandle metrics = ITransform.RankingMetrics(
    truncationLevels: [1, 3, 5, 10])
    .Apply(predictions);
```

Ranking output contains `NDCG@K` and `DCG@K` columns for each requested
truncation level.

## Advanced evaluation

After the held-out evaluation workflow is working, continue with:

- [Cross-validation](../evaluation/cross-validation.md)
- [Permutation feature importance](../evaluation/feature-importance.md)

## Run the cookbook example

The recipe trains on one dataset, evaluates a held-out dataset, and prints a
confusion matrix:

```bash
dotnet run cookbook/GettingStarted/07-evaluation.cs
```

See
[`cookbook/GettingStarted/07-evaluation.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/07-evaluation.cs).

## Common problems

Evaluating training rows usually produces an optimistic result. Keep a test set
or use cross-validation.

Metric transforms require the expected label and prediction columns. Pass
custom column names when the schema differs from the defaults.

For binary probability metrics, explicitly choose `Probability` rather than
assuming raw `Score` is a probability.

Small or single-class test sets can make AUC unstable or uninformative. Ensure
the evaluation data represents the classes and conditions the model will face.

## Next

- [Save and load models](saving-and-loading.md)
- [Advanced evaluation](../evaluation/index.md)
