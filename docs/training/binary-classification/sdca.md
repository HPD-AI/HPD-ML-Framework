# SDCA

The SDCA learner performs shuffled stochastic updates for logistic loss:

```csharp
ILearner learner = ILearner.Sdca(
    options: new SdcaOptions
    {
        L2Regularization = 1e-4,
        NumberOfIterations = 20,
        ConvergenceTolerance = 1e-4,
        Seed = 42
    });
```

## Defaults

| Option | Default |
| --- | ---: |
| `L2Regularization` | `1e-4` |
| `NumberOfIterations` | `20` |
| `ConvergenceTolerance` | `1e-4` |
| `Seed` | `null` |

An explicit seed makes row shuffling repeatable. With no seed, the learner uses
`Random.Shared`.

Progress uses `LogLoss` and zero-based epochs. The current early-stop check
stops when absolute epoch loss falls below `ConvergenceTolerance`; it does not
measure loss improvement or a duality gap.

SDCA loads the entire dataset into lists before training. `ValidationData`,
`InitialModel`, and `Environment` are ignored in `0.5.0`.

## Current correctness status

Do not use published `0.5.0` SDCA for production models. Verification against
clear separable datasets shows the update direction can be reversed: positive
examples drive scores negative and negative examples drive scores positive.
Held-out AUC can therefore be `0.0`, and changing regularization does not fix
the orientation. Small regularization values can also produce enormous
weights or infinite reported loss.

The recipe deliberately prints this result as a package-level regression
demonstration. Correct SDCA update math, finite-state checks, and convergence
tests are required for `0.6.0`.

## Run the recipe

```bash
dotnet run cookbook/BinaryClassification/02-sdca.cs
```

## Next

- [Averaged perceptron](averaged-perceptron.md)
- [Training progress](training-progress.md)
