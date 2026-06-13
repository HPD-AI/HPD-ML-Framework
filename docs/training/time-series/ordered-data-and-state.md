# Ordered Data and State

Time-series output depends on every earlier row. Cursor order is time order;
the package does not inspect timestamps or sort observations.

The default input contract is:

```text
Value   scalar float
```

Custom input names are supported:

```csharp
var detector = new IidSpikeDetector(inputColumn: "Signal");
```

Use finite `float` values. Published `0.5.0` does not consistently validate a
missing column, another numeric type, `NaN`, infinity, or null before
processing.

## State belongs to a cursor

`Apply` returns a scan-backed data handle. Each cursor initializes fresh state:

```csharp
IDataHandle output = detector.Apply(data);

using var first = output.GetCursor(["Alert"]);
using var second = output.GetCursor(["Alert"]);
```

`first` and `second` are independent scans. A second enumeration does not
continue the first. Likewise, applying a fitted model separately to two
batches restarts its observation window.

For direct row-by-row control, concrete detectors expose `InitializeState()`
and `ProcessRow(...)`. This can keep state in one process, but published
`0.5.0` provides no supported checkpoint format:

```text
StateSerializer = null
```

Do not treat the public mutable state objects as durable storage. Their shape
is not a persistence contract.

## Warm-up

Stateful algorithms need history before producing meaningful output:

| Component | Warm-up behavior in `0.5.0` |
| --- | --- |
| SSA anomaly | first `WindowSize` rows have `RawScore = 0`, `PValue = 0.5` |
| SSA forecast | first `WindowSize - 1` rows contain zero-filled vectors |
| Spectral residual | rows before the FFT window fills contain zero scores |
| IID p-values | fewer than two historical scores returns `PValue = 0.5` |

There is no `IsReady` column. Track the row index yourself and do not interpret
these placeholders as real measurements.

## Ordering limitations

Transforms advertise strict ordering and preserve row count, but `0.5.0` does
not reject a weakly ordered input handle. Ensure the source is already in the
required sequence and never partition one logical series across parallel
cursors.

State continuation, cloning, checkpointing, restore validation, and explicit
readiness are planned `0.6.0` corrections.

## Run the recipe

```bash
dotnet run cookbook/TimeSeries/07-stateful-scan-control.cs
```

