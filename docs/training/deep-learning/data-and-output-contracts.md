# Data and Output Contracts

The reliable 0.5.0 training shape is:

```text
Features: float[] with length == first layer InputSize
Label:    float for a one-output network
```

Vector labels are accepted when their length equals the final output width.
Scalar labels are converted with `Convert.ToSingle` only when output width is
one.

Scalar `float` features are accepted for a one-input network. Do not use other
scalar numeric types: they are not converted to `float`, and some row
implementations can reinterpret their stored bits as a `float` instead of
rejecting the value. This can silently train on incorrect data. Use `float[]`
consistently, including for one-element vectors.

```csharp
IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f] }),
    ("Label", new[] { 1f, 3f, 5f }));
```

All rows are copied into `float[][]` before training. Feature and label widths
are checked, but published 0.5.0 does not consistently reject nulls, `NaN`,
infinity, or numeric overflow with clear row context.

## Output limitation

The scoring transform always declares:

```text
Score: float
```

It computes the complete final-layer vector and stores only element zero.
Networks with multiple outputs therefore train, but their remaining outputs
are discarded at the public scoring boundary. Use one output for usable
0.5.0 prediction workflows.

## Run the recipes

```bash
dotnet run cookbook/DeepLearning/03-data-contracts.cs
dotnet run cookbook/DeepLearning/05-multi-output-boundary.cs
```
