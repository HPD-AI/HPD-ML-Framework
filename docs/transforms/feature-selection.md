# Feature Selection

Mutual-information selection ranks separate numeric scalar columns against a
label column and keeps the top K.

```csharp
ILearner learner = ILearner.MutualInfoFeatureSelection(
    labelColumn: "Label",
    featureColumns: ["Age", "Income", "Noise"],
    topK: 2,
    numBins: 16);

IModel model = learner.Fit(new LearnerInput(trainingData));
IDataHandle selected = model.Transform.Apply(trainingData);
```

The learner discretizes each feature and the label into quantile bins, computes
mutual information, and stores scores only for selected columns.

## Output contract

The fitted transform drops every column except:

- the configured label column;
- the selected feature columns.

It does not create or reduce a `Features` vector. Use it before assembling
features, or account for the removed metadata and identifier columns.

Supported feature and label inputs are numeric scalar values represented as
`float`, `double`, `int`, or `long`. Other runtime types are converted to `0f`
in `0.5.0`, which can produce meaningless scores rather than an error. Vectors
are not supported.

Fit selection on training data only and reuse the fitted transform. Fitting on
the full dataset leaks label information from validation or test rows.

Use `topK >= 0`, `numBins >= 1`, non-empty feature names, and non-empty training
data. Option validation is limited in `0.5.0`.

The learner's pre-fit schema keeps the first configured `topK` columns, not the
columns that will actually win after fitting. Treat the fitted model schema as
authoritative.

## Run the recipe

```bash
dotnet run cookbook/Transforms/08-feature-selection.cs
```

## Next

- [Text featurization](text.md)
- [Fixed and learned transforms](fixed-and-learned-transforms.md)
