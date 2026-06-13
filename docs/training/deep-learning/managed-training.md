# Managed Training

The default `NeuralNetworkLearner` registers only
`ManagedDeepLearningBackendProvider`. It handles backend kinds `default`,
`cpu`, and `managed` without a native runtime.

```csharp
var learner = new NeuralNetworkLearner(
    definition,
    new TrainingOptions
    {
        Epochs = 200,
        LearningRate = 0.02f,
        BatchSize = 16
    });

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    backend: BackendSpec.Cpu());

IModel model = learner.Fit(
    new LearnerInput(training, Environment: environment));
```

Managed 0.5.0 training:

- initializes weights from the environment seed, or seed `0`;
- initializes biases to zero;
- uses squared-error gradients and SGD;
- visits rows in cursor order every epoch;
- performs one update per row;
- does not shuffle;
- ignores `BatchSize`;
- materializes all training rows.

The implementation is handwritten backpropagation. Optional native providers
use a separate trainable-tensor implementation and true batches, so identical
options do not define identical optimization in 0.5.0.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/01-single-layer-regression.cs
```

