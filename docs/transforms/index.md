# Transforms

Transforms prepare or reshape an `IDataHandle`. Install:

```bash
dotnet add package HPD-ML-Transforms
```

Import `HPD.ML.Transforms` to use the C# 14 extension members on `ITransform`
and `ILearner`.

## Learning path

1. [Fixed and learned transforms](fixed-and-learned-transforms.md)
2. [Composing transforms](composing-transforms.md)
3. [Categorical encoding](categorical.md)
4. [Missing values](missing-values.md)
5. [Normalization](normalization.md)
6. [Conversion and hashing](conversion-and-hashing.md)
7. [Text featurization](text.md)
8. [Image support](image-support.md)
9. [Feature selection](feature-selection.md)

## Quick reference

| Need | API shape |
| --- | --- |
| Apply known behavior | `ITransform.X(...).Apply(data)` |
| Learn preparation from training rows | `ILearner.X(...).Fit(new LearnerInput(train))` |
| Reuse learned preparation | `model.Transform.Apply(validationOrTest)` |
| Compose configured transforms | `TransformComposer.Compose(first, second)` |

Transforms are generally lazy: `Apply(...)` returns a handle, while a cursor,
materialization, writer, evaluator, or learner consumes its rows.

The guides and recipes are verified against published `HPD-ML-*` version
`0.5.0`. Planned behavior is not presented as available.

