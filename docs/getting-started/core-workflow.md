# Core Workflow

HPD ML uses the same small lifecycle across classification, regression,
clustering, deep learning, and time-series workloads:

```text
IDataHandle
  -> optional ITransform preparation
  -> ILearner.Fit/FitAsync
  -> IModel
  -> IModel.Transform.Apply
  -> IDataHandle predictions
```

This page focuses on those abstractions. Later pages add loading, preparation,
evaluation, serialization, and backend selection.

## How to think in HPD ML

Keep these ideas in mind while reading the examples:

- **Data is a handle plus a schema.** HPD ML passes `IDataHandle` objects
  between components instead of passing separate feature and label arrays.
- **Learners fit; transforms apply.** An `ILearner` inspects data and returns
  an `IModel`. An `ITransform` applies behavior that is already known.
- **A fitted model contains inference.** `IModel.Transform` is the operation
  used to score compatible data; `IModel.Parameters` contains what was
  learned.
- **Preparation can also be learned.** A fitted normalizer, encoder, or text
  featurizer is an `IModel` whose transform must be reused on test and
  production data.
- **Data flow is often lazy.** Applying a transform can return a new handle
  without immediately reading every row. Cursors, streaming, materialization,
  writers, training, and evaluation cause data to be consumed.
- **Schemas are part of the contract.** Column names, CLR types, and vector
  shapes must match what each transform or learner expects.

These rules are more important than any individual algorithm. They remain the
same across workload packages.

## Know when work executes

HPD ML separates pipeline description from row consumption:

| Operation | Reads rows? | Result |
| --- | --- | --- |
| `GetOutputSchema(...)` | No | Describes expected output columns |
| `transform.Apply(data)` | Usually not immediately | Returns a potentially lazy handle |
| `learner.Fit(...)` or `FitAsync(...)` | Yes | Learns and returns an `IModel` |
| `GetCursor(...)` or `StreamRows(...)` | Yes, as rows advance | Streams requested values |
| `Materialize()` | Yes | Stores the complete result in memory |

This distinction matters while debugging. Schema errors can appear while
constructing or validating a stage, while malformed row values may not appear
until a cursor, materialization, writer, evaluator, or learner consumes them.

## Create schema-bearing data

`IDataHandle` is the common data boundary:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle trainingData = InMemoryDataHandle.FromColumns(
    ("Features", new float[][]
    {
        [-2.0f, -1.0f],
        [-1.0f, -0.5f],
        [1.0f, 0.5f],
        [2.0f, 1.0f]
    }),
    ("Label", new[] { false, false, true, true }));
```

A data handle exposes its schema, row count when known, ordering guarantees,
materialization capabilities, cursors, and asynchronous row streaming.

Think of it as a reusable view over data rather than as a guarantee that every
row is already stored in memory. An in-memory handle owns materialized columns;
a CSV handle or transformed handle can instead produce rows when consumed.

## Choose a learner

`ILearner` describes an operation that learns model parameters from data:

```csharp
using HPD.ML.BinaryClassification;

ILearner learner = ILearner.LogisticRegression();
```

HPD ML workload packages expose learners through C# 14 extension members.
Import the workload namespace to make its learners discoverable.

## Fit a model

Pass the training data through `LearnerInput`:

```csharp
IModel model = await learner.FitAsync(new LearnerInput(trainingData));
```

`LearnerInput` can also carry validation data, an initial model, and an
execution environment:

```csharp
var input = new LearnerInput(
    TrainData: trainingData,
    ValidationData: validationData,
    InitialModel: existingModel,
    Environment: executionEnvironment);
```

Only `TrainData` is required. Individual learners decide which optional values
they support.

## Understand the model

An `IModel` is an immutable pair:

- `Transform` performs inference.
- `Parameters` contains the learned values.

The learner does not mutate the training data. It returns a model that can be
applied to another compatible data handle.

## Apply the model

Use the model's transform to produce predictions from schema-compatible data:

```csharp
IDataHandle predictions = model.Transform.Apply(predictionData);
```

For logistic regression, the result preserves the original columns and adds:

- `Score`
- `Probability`
- `PredictedLabel`

The result is another `IDataHandle`, so it can be read, transformed, evaluated,
materialized, or written by the same framework APIs.

Creating the prediction handle does not necessarily execute scoring for every
row. The work occurs as a consumer reads the handle.

When calculating metrics, apply the model to validation or test data that was
not used for fitting. Applying a model to training data can be useful for
diagnostics, but training metrics usually overstate performance on unseen
rows.

## Inspect intermediate data

Inspect the schema after each important preparation stage:

```csharp
IDataHandle prepared = preparation.Apply(rawData);

foreach (var column in prepared.Schema.Columns)
{
    Console.WriteLine(
        $"{column.Name}: {column.Type.ClrType.Name}, " +
        $"vector={column.Type.IsVector}");
}
```

Read only the columns and rows needed for diagnosis:

```csharp
using var rows = prepared.GetCursor(["Features", "Label"]);

for (var index = 0; index < 3 && rows.MoveNext(); index++)
{
    var features = rows.Current.GetValue<float[]>("Features");
    var label = rows.Current.GetValue<bool>("Label");

    Console.WriteLine(
        $"Label={label}, Features=[{string.Join(", ", features)}]");
}
```

Limiting the cursor is important for large or streaming datasets. Use
`Materialize()` when the complete result fits in memory and repeated reads are
worth the memory cost.

## Transform versus learner

An `ITransform` applies known behavior without learning new parameters:

```text
IDataHandle -> IDataHandle
```

An `ILearner` inspects data and produces an `IModel`:

```text
LearnerInput -> IModel
```

The model then exposes the learned inference behavior as an `ITransform`.
Normalization, categorical encoding, and text featurization can have both fixed
transform forms and learner forms depending on whether their configuration must
be learned from data.

## Schema propagation

Both transforms and learners expose `GetOutputSchema(...)`. This lets callers
inspect the expected output columns without reading or training on the data:

```csharp
var outputSchema = learner.GetOutputSchema(trainingData.Schema);
```

`GetOutputSchema(...)` does not consume rows. Training consumes its input;
`Apply(...)` may return a lazy handle whose rows are consumed later by a
cursor, stream, materialization, writer, evaluator, or another learner.

## Run the cookbook example

The matching file-based app pins the tested package versions:

```bash
dotnet run cookbook/GettingStarted/02-core-workflow.cs
```

See
[`cookbook/GettingStarted/02-core-workflow.cs`](../../cookbook/GettingStarted/02-core-workflow.cs).

## Common problems

Learners expect particular column names and types. Logistic regression defaults
to a `Features` column containing `float[]` values and a `Label` column
convertible to `bool`.

An extension learner is unavailable until both its package and namespace are
present. For logistic regression, install `HPD-ML-BinaryClassification` and
import `HPD.ML.BinaryClassification`.

Prediction data must have a schema compatible with the model's scoring
transform. A model trained on one feature shape cannot safely score rows with a
different feature shape.

## Next

- [Build the first complete model](first-model.md)
- [Load data](loading-data.md)
- [Prepare data](preparing-data.md)
