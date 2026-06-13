# Model Contracts

An HPD ML model is a pair:

```csharp
public interface IModel
{
    ITransform Transform { get; }
    ILearnedParameters Parameters { get; }
}
```

`Model` is the default implementation:

```csharp
IModel model = new Model(transform, parameters);
```

## Transform

`ITransform` owns executable inference:

```csharp
ISchema output = model.Transform.GetOutputSchema(input.Schema);
IDataHandle predictions = model.Transform.Apply(input);
```

`GetOutputSchema` describes the output without reading rows. `Apply` normally
returns a lazy data handle; scoring occurs when a cursor, stream, writer, or
materialization operation consumes rows.

`TransformProperties` describes row-count preservation, statefulness, ordering,
and optional device preference. It does not prove that a transform is
serializable.

## Learned parameters

`ILearnedParameters` is a marker interface. Each workload defines its concrete
parameter type, such as:

- `LinearModelParameters`
- `ClusteringModelParameters`
- `NeuralNetworkParameters`
- `TreeEnsembleParameters`
- `SsaModelParameters`

The marker alone does not provide serialization. ZIP loading requires a
registered `IParameterWriter<TParameters>`.

## Models are shallowly immutable

The `Model` record prevents replacing its references, but it does not make the
objects behind those references immutable. Several 0.5.0 parameter types expose
mutable arrays. Treat learned parameters as read-only unless the workload guide
documents a safe copy API.

## Run the recipe

```bash
dotnet run cookbook/Models/01-model-contracts.cs
```
