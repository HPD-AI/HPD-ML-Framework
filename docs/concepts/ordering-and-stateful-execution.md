# Ordering and Stateful Execution

Some operations depend on encounter order, especially time-series scans.

## Ordering policies

```text
Unordered
Ordered
StrictlyOrdered
```

In-memory handles report `StrictlyOrdered`. Shuffled handles report
`Unordered`. Filters preserve their source ordering.

Published `0.5.0` does not define every distinction rigorously. Use
`StrictlyOrdered` for sequence-sensitive inputs and treat cursor order as event
order.

## Stateful transform properties

A stateful transform should declare:

```csharp
new TransformProperties
{
    IsStateful = true,
    RequiresOrdering = true
};
```

These properties are not centrally enforced in `0.5.0`. Current time-series
scan adapters can accept weaker ordering despite declaring the requirement.

## State lifetime

For current scan-backed time-series transforms:

- each new cursor starts with fresh state;
- separate cursors are independent;
- materializing consumes one fresh scan;
- there is no public continuation session;
- built-in state serializers are absent.

Do not assume that opening a second cursor continues the first.

## Shuffling and splitting

Shuffling materializes and weakens the ordering guarantee. Random train/test
splits are inappropriate for sequence-sensitive evaluation unless the workload
explicitly permits them.

## Run the recipe

```bash
dotnet run cookbook/Concepts/08-ordering-and-state.cs
```

## Next

- [Time-series ordered data and state](../training/time-series/ordered-data-and-state.md)
- [Sync, async, and cancellation](sync-async-and-cancellation.md)
