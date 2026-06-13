# Schemas and Field Types

An `ISchema` describes columns without reading rows. It is part of every data,
transform, learner, and model contract.

## Columns

Each `IColumn` contains:

- a case-sensitive `Name`;
- an `IFieldType`;
- annotations;
- a hidden-column flag.

An `IFieldType` contains the CLR type, whether the value is a vector, and
optional vector dimensions.

```csharp
var schema = new SchemaBuilder()
    .AddColumn<int>("Id")
    .AddVectorColumn<float>("Features", 3)
    .AddColumn<bool>("Label", role: "Label")
    .Build();
```

Vector metadata is distinct from the runtime representation. A feature column
can declare scalar element type `float`, vector status, and dimensions while
rows return `float[]`.

## Exact and approximate schemas

`RefinementLevel` represents how precise a schema is. A fitted transform can
produce a more exact schema than its pre-fit learner signature, such as an
encoder whose final vector width is learned from training data.

`IsRefinementOf(...)` is intentionally directional: the more precise schema
tests whether it refines the approximate one.

## Horizontal merge

`MergeHorizontal(...)` combines columns:

- `ErrorOnConflict` rejects duplicate names.
- `LastWriterWins` replaces the previous column and records its prior CLR type
  in a `schema:shadowed-type` annotation.

Use explicit conflict handling when transforms append conventional names such
as `Score`.

## Vertical compatibility

`MergeVertical(...)` validates the same column count, order, names, and CLR
types. In published `0.5.0`, it does not compare every vector dimension or
annotation and does not concatenate rows.

## Typed access follows the schema

`IRow.GetValue<T>` is typed access, not a general conversion API. Read the CLR
type declared by the schema. Published `0.5.0` has unsafe numeric-probe paths
in some row implementations, so probing several numeric types is not a safe
conversion strategy.

## Run the recipe

```bash
dotnet run cookbook/Concepts/01-schema-bearing-data.cs
```

## Next

- [Data handles, rows, and cursors](data-handles-rows-and-cursors.md)
- [Data schemas guide](../data/schemas-and-columns.md)
