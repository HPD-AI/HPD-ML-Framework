# Confusion Matrices

```csharp
IDataHandle counts = ITransform.ConfusionMatrix().Apply(predictions);
Console.WriteLine(ConfusionMatrixFormatter.Format(counts));
```

The transform returns one row per observed label pair, not a dense matrix.
The formatter builds a display grid.

Published `0.5.0` stringifies arbitrary label objects. Null labels become
empty strings and collide with real empty-string labels. Formatting uses fixed
widths, so long labels can misalign.

The transform reads the existing `PredictedLabel`; it does not recompute a
decision from a score.

```bash
dotnet run cookbook/Evaluation/02-confusion-matrix.cs
```
