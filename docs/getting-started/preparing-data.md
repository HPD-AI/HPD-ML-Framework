# Preparing Data

Learners expect particular column names and types. Data preparation converts
raw application columns into that learner-ready schema.

Install the transform package:

```bash
dotnet add package HPD-ML-Transforms
```

## Fixed transforms and learned preparation

HPD ML has two preparation shapes.

An `ITransform` already knows how to change the data:

```text
IDataHandle -> IDataHandle
```

Examples include hashing with a configured bit count, replacing missing values
with a constant, converting a type, and applying a known category mapping.

An `ILearner` must inspect training data before it can produce a preparation
transform:

```text
LearnerInput -> IModel -> IModel.Transform.Apply(data)
```

Examples include:

- learning minimum and maximum values for normalization
- learning category values for one-hot encoding
- learning replacement statistics for missing values
- learning a text vocabulary and IDF weights
- selecting features using mutual information

Fit learned preparation only on training data. Reuse the returned transform for
validation, test, and prediction data to avoid data leakage.

## Prepare text as a feature vector

The binary-classification learner expects a `Features` column containing
`float[]` values. Raw text must first be featurized:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Transforms;

ILearner featurizer = ILearner.TextFeaturize(
    columnName: "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        NgramMin = 1,
        NgramMax = 1,
        MaxFeatures = 32
    });

IModel featureModel = await featurizer.FitAsync(
    new LearnerInput(trainingData));

IDataHandle preparedTraining =
    featureModel.Transform.Apply(trainingData);
```

`TextFeaturize` learns a vocabulary from the training text and produces TF-IDF
vectors. The prepared handle preserves `Text` and `Label` and adds `Features`.

Train the classifier from the prepared handle:

```csharp
using HPD.ML.BinaryClassification;

ILearner classifier = ILearner.LogisticRegression();
IModel classifierModel = await classifier.FitAsync(
    new LearnerInput(preparedTraining));
```

Prepare new data with the same learned vocabulary:

```csharp
IDataHandle preparedTest = featureModel.Transform.Apply(testData);
IDataHandle predictions = classifierModel.Transform.Apply(preparedTest);
```

Do not fit a second text featurizer on test or production data. A separately
learned vocabulary can assign different vector positions to the same terms.

## Normalize numeric values

Learn minimum and maximum values from training data:

```csharp
ILearner normalization = ILearner.MinMaxNormalize(
    columnName: "Age",
    scaleMin: 0f,
    scaleMax: 1f,
    outputColumn: "AgeNormalized");

IModel normalizationModel = normalization.Fit(
    new LearnerInput(trainingData));

IDataHandle normalized =
    normalizationModel.Transform.Apply(trainingData);
```

Use `MeanVarianceNormalize(...)` for zero-mean, variance-scaled values and
`BinNormalize(...)` for learned bins.

## Encode categorical values

Learn a category mapping from the training rows:

```csharp
ILearner encoding = ILearner.OneHotEncode(
    columnName: "City",
    maxCategories: 100,
    outputColumn: "CityFeatures");

IModel encodingModel = encoding.Fit(new LearnerInput(trainingData));
IDataHandle encoded = encodingModel.Transform.Apply(trainingData);
```

Values not seen while fitting map to an all-zero vector.

When the mapping is already known, use the fixed transform:

```csharp
ITransform encoding = ITransform.OneHotEncode(
    "City",
    new Dictionary<string, int>
    {
        ["Chicago"] = 0,
        ["Austin"] = 1
    },
    outputColumn: "CityFeatures");
```

## Handle missing values

Replace missing numeric values with a statistic learned from training data:

```csharp
ILearner replacement = ILearner.ReplaceMissing(
    "Temperature",
    ReplacementStrategy.Mean);

IModel replacementModel = replacement.Fit(
    new LearnerInput(trainingData));

IDataHandle completed =
    replacementModel.Transform.Apply(trainingData);
```

For a known constant, use a fixed transform:

```csharp
ITransform replacement = ITransform.ReplaceMissing(
    "Temperature",
    ReplacementValue.Zero);
```

Other fixed options include `IndicateMissing(...)` and `DropMissing(...)`.

## Compose fitted transforms

After fitting preparation learners separately, compose their resulting
transforms in application order:

```csharp
using HPD.ML.Core;

var preparation = TransformComposer.Compose(
    replacementModel.Transform,
    normalizationModel.Transform,
    encodingModel.Transform);

IDataHandle prepared = preparation.Apply(rawData);
```

Each transform receives the output schema and data of the previous transform.
Order matters when a later step depends on a column created or changed earlier.

The current API fits preparation learners individually. `TransformComposer`
composes already configured or fitted transforms; it does not fit a chain of
learners automatically.

## Inspect the prepared schema

Confirm the required learner columns before training:

```csharp
foreach (var column in prepared.Schema.Columns)
{
    Console.WriteLine(
        $"{column.Name}: {column.Type.ClrType.Name}, " +
        $"vector={column.Type.IsVector}");
}
```

For default binary classification, look for a `Features` vector and a `Label`
column convertible to `bool`.

## Run the cookbook example

The recipe learns text features and feeds the resulting vector into logistic
regression:

```bash
dotnet run cookbook/GettingStarted/04-preparing-data.cs
```

See
[`cookbook/GettingStarted/04-preparing-data.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/04-preparing-data.cs).

## Common problems

Fit preparation from training data only. Fitting normalization, categories, or
text vocabulary using validation or test rows leaks information into training.

Applying separately fitted preparation models can produce incompatible feature
spaces. Reuse the same fitted preparation model for every split.

Column names must line up across stages. Set `outputColumn: "Features"` when a
preparation step should feed a learner that uses the default feature name.

Many numeric transforms currently read `float` values. CSV and JSON decimal
inference defaults to `double`, so use type hints, an explicit schema, or
`ConvertType(...)` before applying a float-based transform.

## Next

- [Train a learner](training.md)
- [Transform guides](../transforms/index.md)

A dedicated schemas and columns guide is planned for the concepts
documentation track.
