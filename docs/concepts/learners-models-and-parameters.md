# Learners, Models, and Parameters

An `ILearner` consumes rows and returns an `IModel`:

```csharp
IModel model = learner.Fit(new LearnerInput(trainingData));
```

## Learner input

`TrainData` is required. The common input also accepts:

- `ValidationData`;
- `InitialModel`;
- `Environment`.

These optional fields are not universal capabilities. In published `0.5.0`,
many learners silently ignore values they do not support. Read the workload
guide before relying on validation, warm start, environment seed, cancellation,
or backend selection.

## Output schema

`learner.GetOutputSchema(inputSchema)` describes expected prediction columns
without training. The fitted model's transform remains authoritative because
learned preparation can refine shapes or mappings.

## Model structure

```csharp
public interface IModel
{
    ITransform Transform { get; }
    ILearnedParameters Parameters { get; }
}
```

- `Transform` performs prediction.
- `Parameters` expose workload-specific learned values.

`Model` is reference-immutable, but several `0.5.0` parameter types expose
mutable arrays. Treat parameter values as read-only.

## Fit and predict

Training normally consumes and materializes required columns. Applying the
model usually returns a cursor-backed prediction handle whose scoring work
occurs during consumption.

## Progress

Subscribe before fitting:

```csharp
using var subscription = learner.Progress.Subscribe(observer);
```

Progress support varies. Some learners emit iterations or epochs; others only
complete. A `ProgressEvent.Checkpoint` being part of the interface does not
mean a learner produces checkpoints.

## Run the recipe

```bash
dotnet run cookbook/Concepts/06-learners-models-and-parameters.cs
```

## Next

- [Models](../models/index.md)
- [Training](../training/index.md)
