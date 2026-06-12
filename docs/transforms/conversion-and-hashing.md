# Conversion and Hashing

Conversion and hashing are fixed transforms.

## Convert a scalar column

```csharp
ITransform convert = ITransform.ConvertType("Value", typeof(float));
IDataHandle converted = convert.Apply(data);
```

`TypeConvertTransform` uses `Convert.ChangeType(...)`. It is appropriate for
compatible scalar conversions such as `int` to `float` or numeric text to an
integer.

Conversion uses the process culture in `0.5.0`. Invalid text and unsupported
conversions fail when rows are consumed. A null converted to a non-nullable
value type fails; conversions targeting compatible reference types may preserve
null.

Although vector metadata is retained in the output schema, arrays are not
converted element by element. Do not use `ConvertType` for vector columns.

## Hash values

```csharp
ITransform hash = ITransform.Hash(
    "Category",
    numBits: 8,
    outputColumn: "CategoryHash");
```

Hashing converts `value?.ToString() ?? ""` to a deterministic MurmurHash3 value
and masks it into `2^numBits` buckets. The output type is scalar `uint`.
Collisions are expected.

Use `numBits` from 1 through 31. Published `0.5.0` does not validate the value,
and C# shift behavior makes zero, negative, or values at least 32 misleading.

The extension member does not expose the hash seed. Construct `HashTransform`
directly when a non-default seed is required.

With no output name, hashing replaces the source field's schema type with
`uint`; a distinct output name preserves the source.

## Run the recipe

```bash
dotnet run cookbook/Transforms/05-conversion-and-hashing.cs
```

## Next

- [Categorical encoding](categorical.md)
- [Normalization](normalization.md)
