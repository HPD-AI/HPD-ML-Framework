# Prediction

Prediction applies a trained model's transform to compatible data:

```csharp
IDataHandle predictions = model.Transform.Apply(inputData);
```

The input does not need labels. It needs the feature columns and schema expected
by the model.

## Reuse training-fitted preparation

When training included learned preparation, apply the same preparation model
before the prediction model:

```csharp
IDataHandle preparedNewData =
    featureModel.Transform.Apply(newData);

IDataHandle predictions =
    classifierModel.Transform.Apply(preparedNewData);
```

This is especially important for text vocabularies, category mappings,
normalization statistics, missing-value replacements, and feature selection.
Fitting new preparation on production rows can change feature meanings.

## Predict unlabeled rows

Training data contains `Text` and `Label`, but new inference data only needs the
raw feature source:

```csharp
using HPD.ML.Core;

IDataHandle newData = InMemoryDataHandle.FromColumns(
    ("Text", new[]
    {
        "helpful reliable service",
        "slow broken product"
    }));
```

Apply the fitted text featurizer and classifier:

```csharp
IDataHandle prepared = featureModel.Transform.Apply(newData);
IDataHandle predictions = classifierModel.Transform.Apply(prepared);
```

The classifier does not require `Label` to calculate predictions.

## Read binary-classification output

Linear binary classifiers append:

- `Score`: the raw linear score
- `Probability`: the sigmoid or calibrated probability
- `PredictedLabel`: `true` when the probability meets the scoring threshold

Read only the columns needed by the application:

```csharp
using var rows = predictions.GetCursor(
    ["Text", "Score", "Probability", "PredictedLabel"]);

while (rows.MoveNext())
{
    var text = rows.Current.GetValue<string>("Text");
    var score = rows.Current.GetValue<float>("Score");
    var probability = rows.Current.GetValue<float>("Probability");
    var label = rows.Current.GetValue<bool>("PredictedLabel");

    Console.WriteLine(
        $"{text}: score={score:F3}, probability={probability:P1}, label={label}");
}
```

`Score` and `Probability` are different values. For logistic regression,
`Probability` is the sigmoid of `Score`. For margin-based classifiers,
validation data can add probability calibration.

## Prediction columns by workload

| Workload | Typical output |
| --- | --- |
| Binary classification | `Score`, `Probability`, `PredictedLabel` |
| Regression | `Score` |
| Clustering | `PredictedLabel`, `Score` |
| Neural network | `Score` |
| SSA forecasting | `Forecast`, `LowerBound`, `UpperBound` |
| Anomaly detection | `Alert`, `RawScore`, and detector-specific scores |

For clustering, `PredictedLabel` is a `uint` cluster identifier and `Score` is
a `float[]` containing distances to all centroids.

Output names and meanings belong to the selected model. Inspect the model's
output schema rather than assuming every learner uses classification columns.

## Inspect output schema

Ask the fitted transform for its output shape:

```csharp
ISchema outputSchema =
    model.Transform.GetOutputSchema(inputData.Schema);

foreach (var column in outputSchema.Columns)
{
    Console.WriteLine(
        $"{column.Name}: {column.Type.ClrType.Name}, " +
        $"vector={column.Type.IsVector}");
}
```

This does not enumerate input rows.

## Prediction is lazy

`Apply(...)` typically returns a transformed data handle. Rows are scored as a
cursor or stream reads them:

```csharp
using var cursor = predictions.GetCursor(["Probability"]);

while (cursor.MoveNext())
{
    var probability =
        cursor.Current.GetValue<float>("Probability");
}
```

Repeated cursor enumeration can repeat transformation and scoring work. Use
`Materialize()` when the complete result fits in memory and will be consumed
more than once:

```csharp
IDataHandle cachedPredictions = predictions.Materialize();
```

## Stream rows asynchronously

Use `StreamRows(...)` when an asynchronous consumer needs complete rows:

```csharp
await foreach (var row in predictions.StreamRows(cancellationToken))
{
    Console.WriteLine(row.GetValue<float>("Probability"));
}
```

The exact source and transform chain determine whether upstream I/O or
computation is genuinely asynchronous.

## Preserve application identifiers

Scoring transforms preserve input columns. Include a stable identifier in the
prediction input so output rows can be joined back to application records:

```csharp
var newData = InMemoryDataHandle.FromColumns(
    ("RequestId", new[] { "a-101", "a-102" }),
    ("Features", featureVectors));
```

Request `RequestId` and the prediction columns from the output cursor.

## Feature compatibility

Prediction rows must use the feature schema expected by the model:

- the feature column name must match
- scalar versus vector shape must match
- vector length and feature ordering must match training
- stored CLR values must use compatible types

The current linear scoring transform reads a `float[]` vector or a scalar
`float`. A mismatched vector length can fail while calculating the score.

## Stateful prediction

Time-series forecasting and anomaly detection use stateful scan transforms.
They require ordered input and update inference state as rows are consumed.
Their `TransformProperties` report `IsStateful` and `RequiresOrdering`.

Do not treat a stateful transform like independent row scoring. Preserve row
order and plan explicitly for inference-state persistence when continuing a
stream.

## Run the cookbook example

The recipe trains on labeled text and predicts two new unlabeled reviews:

```bash
dotnet run cookbook/GettingStarted/06-prediction.cs
```

See
[`cookbook/GettingStarted/06-prediction.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/06-prediction.cs).

## Common problems

Applying only the classifier to raw text fails because the classifier expects
the prepared `Features` column. Apply the training-fitted featurizer first.

Do not read `Probability` as `double`; binary scoring stores it as `float`.
Inspect the schema when a typed read fails.

Prediction handles are lazy. If no cursor, stream, materialization, or writer
reads the output, scoring work has not occurred.

A prediction model does not automatically include its earlier preparation
models unless the transforms were explicitly composed and stored together.
Keep the full inference chain available.

## Next

- [Evaluate predictions](evaluation.md)
- [Save and load models](saving-and-loading.md)

A dedicated production prediction guide is planned for the models
documentation track.
