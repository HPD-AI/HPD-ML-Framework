# Online Gradient Descent

Online gradient descent updates weights once per row:

```csharp
ILearner learner = ILearner.OnlineGradientDescent(
    options: new OnlineGradientDescentOptions
    {
        LearningRate = 0.01,
        NumberOfIterations = 10,
        AverageWeights = true,
        DecreaseLearningRate = false
    });
```

Despite the name, published `0.5.0` materializes all rows before training.
Rows are then processed in input order without shuffling.

## Options

| Option | Default |
| --- | ---: |
| `LearningRate` | `0.1` |
| `L2Regularization` | `0` |
| `NumberOfIterations` | `1` |
| `AverageWeights` | `true` |
| `DecreaseLearningRate` | `false` |

When decay is enabled, the rate is
`LearningRate / (1 + pass * rowCount + rowIndex)`.

## Warm start

This is the only `0.5.0` regression learner that reads `InitialModel`:

```csharp
IModel continued = learner.Fit(
    new LearnerInput(nextBatch, InitialModel: firstModel));
```

It copies overlapping weights and the bias. A feature-count mismatch is
silently truncated or zero-padded. Averaging accumulators and the decay
schedule restart, so this is a warm start rather than exact continuation.

Validation data and execution environments are ignored. Progress reports
zero-based `SquaredLoss` events once per pass.

## Run the recipe

```bash
dotnet run cookbook/Regression/03-online-gradient-descent-and-warm-start.cs
```

## Next

- [Poisson regression](poisson-regression.md)
- [Training progress](training-progress.md)

