# Validation and Warm Start

`LearnerInput` accepts validation data and an initial model:

```csharp
new LearnerInput(
    TrainData: training,
    ValidationData: validation,
    InitialModel: priorModel,
    Environment: environment);
```

Published Deep Learning 0.5.0 silently ignores both `ValidationData` and
`InitialModel`.

Consequences:

- no validation loss is calculated;
- there is no early stopping;
- no best checkpoint is selected;
- supplied weights do not initialize training;
- optimizer continuation is unavailable.

Do not present a 0.5.0 fit as warm-started or validation-selected. The 0.6.0
proposal requires each optional input to be implemented deliberately or
rejected with `NotSupportedException`.

Parameter-only warm start, if added, should not be described as exact training
continuation because SGD state, epoch position, and shuffle state are absent.

## Run the recipe

```bash
dotnet run cookbook/DeepLearning/06-optional-input-boundary.cs
```

