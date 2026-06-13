#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Inspect transform metadata and contrast it with runtime enforcement.

using HPD.ML.Abstractions;
using HPD.ML.Core;

ITransform transform = new DeclaredOrderedTransform();

Console.WriteLine($"stateful: {transform.Properties.IsStateful}");
Console.WriteLine($"requires ordering: {transform.Properties.RequiresOrdering}");
Console.WriteLine($"preserves rows: {transform.Properties.PreservesRowCount}");
Console.WriteLine($"device: {transform.Properties.DevicePreference?.DeviceId}");
Console.WriteLine($"estimated memory: {transform.Properties.Resources?.EstimatedMemoryBytes}");

IDataHandle unordered = new ShuffledDataHandle(
    InMemoryDataHandle.FromColumns(("Value", new[] { 1, 2, 3 })),
    seed: 42);

// Published 0.5.0 does not centrally reject this weaker ordering contract.
IDataHandle result = transform.Apply(unordered);
Console.WriteLine($"input ordering: {unordered.Ordering}");
Console.WriteLine($"application still succeeded: {result.RowCount == 3}");

sealed class DeclaredOrderedTransform : ITransform
{
    public TransformProperties Properties => new()
    {
        IsStateful = true,
        RequiresOrdering = true,
        PreservesRowCount = true,
        DevicePreference = new DevicePreference("cpu"),
        Resources = new ResourceRequirements(EstimatedMemoryBytes: 1024)
    };

    public ISchema GetOutputSchema(ISchema inputSchema) => inputSchema;
    public IDataHandle Apply(IDataHandle input) => input;
}

// Guide: docs/concepts/transforms-and-composition.md
