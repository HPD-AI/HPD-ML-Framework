# Custom Learners and Models

Implement `ILearner` when behavior must be fitted from data:

```csharp
public interface ILearner
{
    ISchema GetOutputSchema(ISchema inputSchema);
    IModel Fit(LearnerInput input);
    Task<IModel> FitAsync(
        LearnerInput input,
        CancellationToken ct = default);
    IObservable<ProgressEvent> Progress { get; }
}
```

A fitted model pairs executable inference with inspectable parameters:

```csharp
return new Model(
    new MyScoringTransform(parameters),
    parameters);
```

## Fit contract

Before expensive work:

- validate required columns and types;
- validate options;
- decide whether `ValidationData` is supported;
- decide whether `InitialModel` is supported;
- validate backend or device requests;
- check cancellation.

Do not silently ignore optional inputs. Published 0.5.0 has learners that do,
but an extension should either implement the input or reject it explicitly.

## Parameters

`ILearnedParameters` is a marker. A custom parameter type should:

- validate dimensions and finite values;
- copy caller-owned arrays;
- avoid exposing mutable model storage;
- keep inference state separate from learned parameters;
- provide a custom parameter writer if ZIP loading is required.

## Async work

`Task.Run(() => Fit(input), ct)` only prevents work from starting when the
token is already canceled. The synchronous fitting loop must also check the
linked token during row loading and bounded training steps.

## Reuse

Published 0.5.0 does not define whether a learner instance supports repeated or
concurrent fits. Prefer independent fit-local state, reject concurrent fitting,
and keep progress lifecycles separate.

## Run the recipe

```bash
dotnet run cookbook/Extending/04-custom-learner-and-model.cs
```

## Related

- [Progress and events](progress-and-events.md)
- [Custom serialization](custom-serialization.md)
