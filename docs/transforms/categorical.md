# Categorical Encoding

HPD ML provides one-hot encoding plus fixed value-to-key and key-to-value
mappings.

## Learn one-hot categories

```csharp
ILearner learner = ILearner.OneHotEncode(
    columnName: "City",
    maxCategories: 100,
    outputColumn: "CityFeatures");

IModel model = learner.Fit(new LearnerInput(trainingData));
IDataHandle encoded = model.Transform.Apply(trainingData);
```

The learner accepts values through their `ToString()` representation. Category
indices are assigned in first-seen order until `maxCategories` is reached.
Unseen and capped values produce an all-zero vector.

The output is `float[]`. Its vector dimension equals the fitted mapping count,
and the column carries a `role:KeyValues` string-array annotation.

## Supply a known mapping

```csharp
ITransform transform = ITransform.OneHotEncode(
    "City",
    new Dictionary<string, int>
    {
        ["Chicago"] = 0,
        ["Austin"] = 1
    },
    outputColumn: "CityFeatures");
```

Mappings must use contiguous, zero-based indices in `0.5.0`. The constructor
does not validate this; gaps or out-of-range indices can fail while reading.

## Keys and values

```csharp
ITransform toKey = ITransform.ValueToKey("City", mapping);
ITransform toValue = ITransform.KeyToValue("CityKey", ["Chicago", "Austin"]);
```

`ValueToKey` outputs `int`; unknown values become `-1`. `KeyToValue` outputs
`string`; negative or out-of-range keys become `""`.

When `outputColumn` is omitted, the source column is replaced in the output
schema. A distinct output name preserves the source column.

## Run the recipe

```bash
dotnet run cookbook/Transforms/02-categorical-encoding.cs
```

## Next

- [Missing values](missing-values.md)
- [Text featurization](text.md)

