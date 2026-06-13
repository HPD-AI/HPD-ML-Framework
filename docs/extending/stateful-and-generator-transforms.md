# Stateful and Generator Transforms

HPD ML defines two specialized transform contracts.

## Scan transforms

`IScanTransform<TState>` carries state across ordered input rows and produces
one output per input:

```csharp
TState InitializeState();
(TState NextState, IRow Output) ProcessRow(
    TState state,
    IRow input);
```

Set:

```csharp
new TransformProperties
{
    IsStateful = true,
    RequiresOrdering = true,
    PreservesRowCount = true
};
```

Published 0.5.0 has no general Core scan adapter. Time Series contains an
internal adapter used by its models. Third-party extensions must currently
provide their own adapter or expose direct state control.

Allocate state per cursor. Do not keep one mutable state object on the
transform instance, or independent enumerations will interfere.

## Generator transforms

`IGeneratorTransform<TState>` starts from seed data and repeatedly calls
`Step(...)` until it returns `null`:

```csharp
TState InitializeState(IDataHandle seed);
(TState NextState, IRow Output)? Step(TState state);
int? MaxOutputLength { get; }
```

The interface exists in 0.5.0, but Core provides no execution adapter,
cancellation policy, or built-in implementation. Treat it as a low-level
contract. A custom adapter must enforce a hard output limit and cancellation
to prevent non-terminating generators.

## Checkpointing

`StateSerializer == null` means scan state is not checkpointable. When
implementing `IStateSerializer<TState>`, include an application-level state
type and version because the interface itself carries neither.

The 0.6.0 Core and Extensibility proposal calls for shared scan/generator
execution and stable checkpoint identity.

## Run the recipes

```bash
dotnet run cookbook/Extending/07-stateful-scan-transform.cs
dotnet run cookbook/Extending/08-generator-transform.cs
```
