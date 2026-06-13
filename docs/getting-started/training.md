# Training

Training configures an `ILearner`, supplies a `LearnerInput`, and produces an
`IModel`.

```text
ILearner + LearnerInput -> IModel
```

The model contains the learned parameters and the transform used for
prediction.

## Configure a learner

Workload packages expose learners through C# 14 extension members:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.BinaryClassification;

ILearner learner = ILearner.LinearSvm(
    labelColumn: "Label",
    featureColumn: "Features",
    options: new LinearSvmOptions
    {
        NumberOfIterations = 20,
        Lambda = 0.01,
        Seed = 42
    });
```

Options belong to the selected algorithm. For linear SVM:

- `NumberOfIterations` controls training passes.
- `Lambda` controls regularization.
- `Seed` makes row shuffling repeatable.
- `PerformProjection` enables the PEGASOS norm constraint.
- `NoBias` disables the learned bias term.

Other learners expose their own learning rates, iteration counts,
regularization, tolerances, architecture, or clustering options.

## Cache repeatedly scanned data

Some learners make multiple passes over their input. When prepared data is lazy
or expensive to read and fits comfortably in memory, wrap it before training:

```csharp
using HPD.ML.Abstractions;
using HPD.ML.Core;

IDataHandle cachedTrainingData = new CachedDataHandle(preparedTrainingData);
var model = learner.Fit(new LearnerInput(cachedTrainingData));
```

`CachedDataHandle` materializes its inner data handle on first access and reuses
that materialized data for later cursors, row streams, and column batches.
Place the cache after reusable preparation work so those transforms are not
recomputed on every training pass.

Caching trades memory for repeat-read speed. Do not cache a dataset that is too
large to materialize safely, and do not expect caching to make a single-pass,
already in-memory source faster.

## Build the learner input

Only training data is required:

```csharp
var input = new LearnerInput(TrainData: trainingData);
```

The complete input contract is:

```csharp
var input = new LearnerInput(
    TrainData: trainingData,
    ValidationData: validationData,
    InitialModel: existingModel,
    Environment: executionEnvironment);
```

Support for optional values is learner-specific:

- `ValidationData` calibrates probabilities for linear SVM and averaged
  perceptron.
- `InitialModel` warm-starts selected online learners, including averaged
  perceptron and online gradient descent.
- `Environment` supplies logging, seed, cancellation, scheduling, device
  preference, and backend selection. Learners use the parts they support.

Passing an optional value does not guarantee that every learner consumes it.
Check the learner guide or implementation when the behavior matters.

## Fit synchronously or asynchronously

Use `Fit(...)` in a synchronous application:

```csharp
IModel model = learner.Fit(input);
```

Use `FitAsync(...)` when the surrounding application is asynchronous:

```csharp
IModel model = await learner.FitAsync(input, cancellationToken);
```

Current CPU learners commonly implement `FitAsync` by scheduling their
synchronous training work. Cancellation support can therefore vary by learner
and may stop scheduling before work begins rather than interrupt every inner
optimization step.

## Observe progress

Subscribe before starting training:

```csharp
using var progress = learner.Progress.Subscribe(
    new TrainingObserver());

var model = await learner.FitAsync(input);
```

`ProgressEvent` can contain:

- `Epoch`
- `MetricName`
- `MetricValue`
- `Checkpoint`

The exact metric varies by learner. Linear SVM reports `HingeLoss`; averaged
perceptron reports `ErrorRate`; online gradient descent reports
`SquaredLoss`.

`IObservable<T>` requires an observer:

```csharp
sealed class TrainingObserver : IObserver<ProgressEvent>
{
    public void OnNext(ProgressEvent value) =>
        Console.WriteLine(
            $"Epoch {value.Epoch}: " +
            $"{value.MetricName}={value.MetricValue:F4}");

    public void OnCompleted() =>
        Console.WriteLine("Training complete.");

    public void OnError(Exception error) =>
        Console.Error.WriteLine(error.Message);
}
```

Dispose the subscription when progress is no longer needed.

## Use validation data

Validation data is separate from the rows used to update model parameters:

```csharp
var input = new LearnerInput(
    TrainData: trainingData,
    ValidationData: validationData);
```

For linear SVM and averaged perceptron, validation rows fit a Platt calibrator
after the linear weights are learned. The resulting transform adds calibrated
probabilities.

Validation data should use the same prepared schema as training data. Apply the
training-fitted preparation transforms to validation rows before passing them
to the learner.

## Use an initial model

Warm-start learners can continue from earlier parameters:

```csharp
IModel first = learner.Fit(new LearnerInput(firstBatch));

IModel continued = learner.Fit(
    new LearnerInput(
        TrainData: secondBatch,
        InitialModel: first));
```

Do not assume a batch learner uses `InitialModel`. This behavior is currently
implemented by selected online algorithms.

## Configure an execution environment

`DefaultExecutionEnvironment` carries shared runtime concerns:

```csharp
using HPD.ML.Core;

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    cancellationToken: cancellationToken,
    backend: BackendSpec.Cpu());

var input = new LearnerInput(
    TrainData: trainingData,
    Environment: environment);
```

Deep-learning learners use the environment seed for deterministic
initialization and the backend specification for provider selection. Some
classical learners instead expose an algorithm-specific seed in their options.

Backend configuration is covered separately in
[Execution Backends](execution-backends.md).

## Inspect learned parameters

Parameters are exposed through `IModel.Parameters`:

```csharp
var parameters = (LinearModelParameters)model.Parameters;

Console.WriteLine($"Features: {parameters.FeatureCount}");
Console.WriteLine($"Bias: {parameters.Bias}");

foreach (var weight in parameters.Weights)
    Console.WriteLine(weight);
```

The concrete parameter type depends on the learner. Treat parameters as
algorithm-specific diagnostic or serialization data; use `model.Transform` for
normal inference.

## Check output schema before training

Ask a learner which columns its model will add:

```csharp
ISchema predictionSchema =
    learner.GetOutputSchema(trainingData.Schema);
```

This validates and documents the expected prediction shape without fitting the
model.

## Run the cookbook example

The recipe configures linear SVM, supplies validation data, prints each progress
event, and inspects the learned weights:

```bash
dotnet run cookbook/GettingStarted/05-training.cs
```

See
[`cookbook/GettingStarted/05-training.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/05-training.cs).

## Common problems

Training columns must exist and use compatible types. Most linear learners
expect `Features` as `float[]` and a workload-specific `Label`.

Subscribe to `Progress` before calling `Fit` or `FitAsync`; progress events are
not replayed to later subscribers.

Use the same preparation model for training and validation data. Independently
fitted vocabularies, category maps, or normalizers can make feature positions
incompatible.

Algorithm seeds and environment seeds are not interchangeable. Set the seed
consumed by the selected learner.

## Next

- [Apply the model](prediction.md)
- [Evaluate predictions](evaluation.md)
- [Training workload guides](../training/index.md)
