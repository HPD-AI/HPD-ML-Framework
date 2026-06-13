# Prediction and Output Schemas

Prediction applies the model transform to compatible rows:

```csharp
IDataHandle predictions = model.Transform.Apply(input);
```

The input needs the columns and shapes expected by the fitted transform. Labels
are normally unnecessary at inference time.

## Inspect before reading

```csharp
ISchema output =
    model.Transform.GetOutputSchema(input.Schema);
```

Common outputs are:

| Workload | Output |
| --- | --- |
| Binary classification | `Score`, `Probability`, `PredictedLabel` |
| Regression | `Score` |
| Clustering | `PredictedLabel`, vector `Score` |
| Deep Learning 0.5.0 | scalar `Score` |
| SSA forecasting | `Forecast`, `LowerBound`, `UpperBound` |

Output names belong to the fitted transform. Existing output-name collisions
are not handled consistently across 0.5.0 workloads, so inspect schemas and
avoid supplying reserved prediction columns.

## Shape compatibility

Current linear and neural-network scorers do not provide a uniform deliberate
feature-width diagnostic. Short vectors can fail through indexing; some
workloads can ignore extra values. Preserve training feature order and width.

## Laziness

`Apply` does not necessarily score immediately:

```csharp
using var rows = predictions.GetCursor(["Score"]);
while (rows.MoveNext())
    Console.WriteLine(rows.Current.GetValue<float>("Score"));
```

Materialize only when repeated consumption justifies the memory cost.

## Run the recipe

```bash
dotnet run cookbook/Models/02-prediction-and-schema.cs
```
