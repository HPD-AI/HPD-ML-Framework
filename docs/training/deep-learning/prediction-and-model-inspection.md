# Prediction and Model Inspection

Fitting returns a model whose parameters are
`NeuralNetworkParameters`:

```csharp
IModel model = learner.Fit(new LearnerInput(training));
var parameters = (NeuralNetworkParameters)model.Parameters;

Console.WriteLine(parameters.Weights.Count);
Console.WriteLine(parameters.Biases.Count);
```

Weights are flattened in input-major order:

```text
weight[input * outputSize + output]
```

The constructor copies supplied arrays and validates their lengths. However,
the arrays exposed through `Weights` and `Biases` remain mutable. Treat them as
read-only; changing an element changes later predictions.

Scoring appends scalar `Score`, preserves row count, and executes in managed
code even when training used MLX or PJRT.

Supply exactly the feature width used by the first layer. Published 0.5.0 does
not issue a deliberate dimension error during prediction; a short vector can
fail through indexing, while extra values are ignored.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/07-model-inspection.cs
```

