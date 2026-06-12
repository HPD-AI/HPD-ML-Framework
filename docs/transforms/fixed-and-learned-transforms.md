# Fixed and Learned Transforms

HPD ML exposes two preparation shapes.

## Fixed transforms

An `ITransform` already contains everything required to process a row:

```text
IDataHandle -> IDataHandle
```

```csharp
ITransform hash = ITransform.Hash(
    columnName: "Category",
    numBits: 8,
    outputColumn: "CategoryHash");

IDataHandle transformed = hash.Apply(data);
```

Examples include hashing, type conversion, dropping rows with missing values,
and categorical encoding with an application-supplied mapping.

## Learned preparation

An `ILearner` consumes training rows and returns an `IModel`:

```text
LearnerInput -> IModel
```

The model's `Transform` contains the fitted preparation:

```csharp
ILearner learner = ILearner.MinMaxNormalize("Age");
IModel model = learner.Fit(new LearnerInput(trainingData));

IDataHandle preparedTraining = model.Transform.Apply(trainingData);
IDataHandle preparedTest = model.Transform.Apply(testData);
```

Normalization statistics, category mappings, missing-value statistics, text
vocabularies, and feature selections must be learned from training data only.
Fitting them again on validation, test, or production rows changes the feature
space and leaks information.

## Schema and execution

`GetOutputSchema(...)` describes a stage without reading rows:

```csharp
ISchema expected = learner.GetOutputSchema(data.Schema);
```

For learned stages, the pre-fit schema can be less precise than the fitted
model schema. In `0.5.0`, one-hot and text vector lengths are only exact after
fitting.

`Apply(...)` is usually lazy. Errors involving row values may appear only when
the output is consumed.

## Current constraints

- Most normalization transforms require scalar `float` columns.
- Extension members require `using HPD.ML.Transforms;`.
- A composed transform applies configured or fitted transforms; it does not
  fit a sequence of learners.
- Several transforms have limited schema validation in `0.5.0`. Inspect input
  and output schemas explicitly when building reusable pipelines.

## Run the recipe

```bash
dotnet run cookbook/Transforms/09-fit-once-reuse.cs
```

## Next

- [Compose transforms](composing-transforms.md)
- [Normalization](normalization.md)

