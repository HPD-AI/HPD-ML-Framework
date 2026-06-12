# Composing Transforms

`TransformComposer.Compose(...)` applies transforms from left to right:

```csharp
using HPD.ML.Abstractions;

ITransform preparation = TransformComposer.Compose(
    ITransform.IndicateMissing("Age"),
    ITransform.ReplaceMissing("Age", ReplacementValue.Zero),
    ITransform.Hash("City", numBits: 8, outputColumn: "CityHash"));

IDataHandle prepared = preparation.Apply(rawData);
```

Each stage receives the prior stage's schema and rows. Order matters when a
later stage consumes a column created or changed earlier.

## Compose fitted preparation

Fit data-dependent stages separately on training data, then compose the
returned transforms:

```csharp
IModel ageModel = ILearner.MinMaxNormalize("Age")
    .Fit(new LearnerInput(trainingData));

IModel cityModel = ILearner.OneHotEncode("City")
    .Fit(new LearnerInput(trainingData));

ITransform preparation = TransformComposer.Compose(
    ageModel.Transform,
    cityModel.Transform);
```

Apply the same composed transform to every split:

```csharp
IDataHandle train = preparation.Apply(trainingData);
IDataHandle test = preparation.Apply(testData);
```

`TransformComposer` does not accept or fit learners. If fitting stage B
depends on the output of fitted stage A, fit A, apply A to the training data,
then fit B from that prepared training handle.

## Properties and laziness

The composition:

- preserves row count only when every child does;
- requires ordering when any child does;
- applies each child lazily according to its implementation;
- propagates schemas in application order.

In published `0.5.0`, `TransformComposer.Compose(...)` returns a private nested
implementation exposed only as `ITransform`. Do not depend on casting it to
`HPD.ML.Core.ComposedTransform`.

## Run the recipe

```bash
dotnet run cookbook/Transforms/01-compose-transforms.cs
```

## Next

- [Fixed and learned transforms](fixed-and-learned-transforms.md)
- [Categorical encoding](categorical.md)
