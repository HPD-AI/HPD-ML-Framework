# Text Featurization

`TextFeaturize` learns a vocabulary and inverse-document-frequency weights from
a scalar `string` column.

```csharp
ILearner learner = ILearner.TextFeaturize(
    columnName: "Text",
    outputColumn: "Features",
    options: new TextFeaturizeOptions
    {
        CaseNormalize = true,
        RemoveStopWords = true,
        NgramMin = 1,
        NgramMax = 2,
        MaxFeatures = 1000
    });

IModel model = learner.Fit(new LearnerInput(trainingData));
IDataHandle features = model.Transform.Apply(trainingData);
```

## Processing

The implementation:

1. optionally lowercases with invariant casing;
2. splits on the configured separator characters;
3. optionally removes a built-in English stop-word set;
4. emits contiguous n-grams joined with `|`;
5. selects terms by descending document frequency;
6. applies term frequency multiplied by `log(documentCount / documentFrequency)`.

The output is a dense `float[]`. Its fitted dimension is the number of selected
terms, which can be smaller than `MaxFeatures`.

Unknown terms contribute zero. Terms present in every training document have
IDF zero and therefore also contribute zero.

## Reuse

Fit once on training text and reuse `model.Transform` for validation, test, and
production data. A separately fitted vocabulary can assign different meanings
to the same vector positions.

The pre-fit learner schema advertises `MaxFeatures`, while the fitted transform
advertises the actual vocabulary count in `0.5.0`.

Choose valid options explicitly: `MaxFeatures > 0`, `NgramMin >= 1`, and
`NgramMax >= NgramMin`. These are not comprehensively validated.

## Run the recipe

```bash
dotnet run cookbook/Transforms/06-text-featurization.cs
```

## Next

- [Categorical encoding](categorical.md)
- [Feature selection](feature-selection.md)

