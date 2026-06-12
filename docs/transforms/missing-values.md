# Missing Values

In `0.5.0`, a value is considered missing when it is:

- `null`;
- `float.NaN` or `double.NaN`;
- an empty or whitespace-only string.

## Replace with a known value

```csharp
ITransform replacement = ITransform.ReplaceMissing(
    "Temperature",
    ReplacementValue.Constant(-1f));
```

`ReplacementValue.Zero` is the `float` value `0f`. The replacement object must
match the column's runtime type because the schema is preserved unchanged.

## Learn a replacement statistic

```csharp
ILearner learner = ILearner.ReplaceMissing(
    "Temperature",
    ReplacementStrategy.Median);

IModel model = learner.Fit(new LearnerInput(trainingData));
```

Mean, median, and mode consume `float`, `double`, and `int` values and store a
`float` replacement. This is safest for `float` columns. Applying the learned
transform to an `int` or `double` schema can create a schema/value mismatch in
`0.5.0`.

When no usable values exist, the learned replacement is `0f`.

## Add an indicator

```csharp
ITransform indicator = ITransform.IndicateMissing(
    "Temperature",
    indicatorColumn: "TemperatureWasMissing");
```

The transform preserves the source column and appends a `bool` column.

## Drop rows

```csharp
ITransform completeRows = ITransform.DropMissing(
    "Temperature",
    "Humidity");
```

The schema is unchanged, but row count is no longer preserved. The resulting
handle is lazy and filters rows as a consumer advances its cursor.

Avoid choosing an indicator or output name that already exists; duplicate
column names are not consistently rejected in `0.5.0`.

## Run the recipe

```bash
dotnet run cookbook/Transforms/03-missing-values.cs
```

## Next

- [Normalization](normalization.md)
- [Composing transforms](composing-transforms.md)

