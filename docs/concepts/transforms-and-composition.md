# Transforms and Composition

An `ITransform` applies behavior that is already known:

```text
IDataHandle -> IDataHandle
```

It exposes output-schema calculation, application, and `TransformProperties`.

## Fixed and fitted transforms

A fixed transform is ready to apply immediately. A data-dependent preparation
stage is first represented by an `ILearner`; fitting returns an `IModel` whose
`Transform` contains the learned behavior.

```csharp
IModel normalizer = ILearner.MinMaxNormalize("Age")
    .Fit(new LearnerInput(trainingData));

IDataHandle prepared = normalizer.Transform.Apply(testData);
```

Fit learned preparation on training data only and reuse it everywhere else.

## Compose left to right

```csharp
ITransform pipeline = TransformComposer.Compose(first, second, third);
```

Each stage receives the prior output schema and rows. Composition does not fit
learners.

Published `0.5.0` composition propagates:

- statefulness when any child is stateful;
- ordering requirement when any child requires ordering;
- row-count preservation only when every child preserves it.

It does not propagate `DevicePreference` or `ResourceRequirements`, and Core
does not centrally enforce the ordering requirement.

## Transform properties are claims

`TransformProperties` can describe state, ordering, row-count preservation,
device preference, and resources. In `0.5.0`, treat device and resource fields
as advisory unless a workload explicitly documents enforcement.

## Schema and application must agree

A transform should return rows matching `GetOutputSchema(...)`. Inspect both
when implementing custom transforms, especially around duplicate output names,
vector dimensions, and row-count-changing operations.

## Run the recipes

```bash
dotnet run cookbook/Concepts/04-transform-properties.cs
dotnet run cookbook/Concepts/05-compose-transforms.cs
```

## Next

- [Transforms guide](../transforms/index.md)
- [Learners, models, and parameters](learners-models-and-parameters.md)
