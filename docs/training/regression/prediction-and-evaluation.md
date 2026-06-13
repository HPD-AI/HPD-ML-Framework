# Prediction and Evaluation

All four learners append one prediction column:

```text
Score   scalar float
```

For OLS, SDCA, and online gradient descent, `Score` is `w.x + b`. For Poisson,
it is `exp(w.x + b)`.

```csharp
IDataHandle predictions = model.Transform.Apply(testData);

using var rows = predictions.GetCursor(["Label", "Score"]);
while (rows.MoveNext())
{
    float score = rows.Current.GetValue<float>("Score");
    Console.WriteLine(score);
}
```

Scoring is lazy and preserves input columns. Prediction rows require the same
feature name, shape, order, and minimum vector length used during training.
Published `0.5.0` does not produce an early schema error for feature-length
mismatches.

## Regression metrics

```csharp
IDataHandle metrics = ITransform.RegressionMetrics(
    labelColumn: "Label",
    scoreColumn: "Score",
    featureCount: 3)
    .Apply(predictions);
```

The one-row result contains `double` columns:

- `MAE`
- `MSE`
- `RMSE`
- `RSquared`
- `AdjustedRSquared`

Without `featureCount`, adjusted R-squared equals ordinary R-squared. When
`n <= featureCount + 1`, `0.5.0` also silently falls back to ordinary
R-squared.

Empty input returns zeros for every metric. Constant labels return
`RSquared = 0`, even for perfect constant predictions. These misleading edge
contracts are planned for correction in `0.6.0`.

For a directly constructed metric input, use `double` label and score columns.
Published `0.5.0` probes `double` before `float`; in-memory row coercion can
reinterpret boxed `float` values during that probe and produce meaningless
near-zero inputs. Normal model predictions use dictionary-backed rows that
coerce the `float` score correctly.

## Run the recipe

```bash
dotnet run cookbook/Regression/08-regression-metrics.cs
```

## Next

- [Training progress](training-progress.md)
- [General evaluation](../../getting-started/evaluation.md)
