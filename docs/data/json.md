# JSON and JSON Lines

`LoadJson(...)` supports one JSON object per line and a top-level JSON array.
The two formats have different memory behavior.

## Load JSON Lines

```csharp
var data = IDataHandle.LoadJson(
    "events.jsonl",
    new JsonOptions { IsJsonLines = true });
```

JSON Lines is read one physical line at a time as the cursor advances. Blank
lines are skipped. Each nonblank line must contain one complete JSON object.

Consume each returned row before advancing the cursor or async enumeration. In
`0.5.0`, a JSON Lines row references the current parsed document, which is
disposed when the next row is read. Materialize or copy needed values when they
must outlive the current iteration.

## Load a JSON array

```csharp
var data = IDataHandle.LoadJson(
    "events.json",
    new JsonOptions { IsJsonLines = false });
```

Published `0.5.0` parses the complete array into a `JsonDocument` for each
cursor. It is not a streaming choice for large files.

When `IsJsonLines` is `null`, the first non-whitespace character selects the
format: `[` means array; other input is treated as JSON Lines.

## Flatten nested objects

Nested names use dotted paths:

```csharp
var data = IDataHandle.LoadJson(
    "events.jsonl",
    new JsonOptions
    {
        IsJsonLines = true,
        MaxFlattenDepth = 1,
        PropertyMapping = new Dictionary<string, string>
        {
            ["device.temperature"] = "Temperature"
        }
    });
```

Property paths, mapped names, and row access are case-sensitive.

## Inference and type hints

JSON inference recognizes `int`, `long`, `double`, `bool`, and `string`.
Conflicts widen numerics and fall back to `string` for incompatible values.

```csharp
new JsonOptions
{
    InferenceScanRows = 100,
    TypeHints = new Dictionary<string, Type>
    {
        ["Temperature"] = typeof(float)
    }
}
```

Unlike CSV, `InferenceScanRows = 0` scans zero JSON records in `0.5.0`. Avoid
zero and use a positive limit.

## Current 0.5.0 limitations

- There is no explicit-schema `LoadJson(...)` overload.
- A property inferred from one row but missing from a later row throws when
  read.
- JSON `null` is treated as a string-shaped value during inference.
- Arrays and nested arrays are not modeled as vector columns.
- There is no JSON writer.
- `RowCount` is `null`, including for JSON arrays.
- Cursor projection is not enforced by the returned row object.

Use JSON Lines for large sequential sources. Use CSV with an explicit schema
when a stable text contract is more important than nested property mapping.

## Run the recipe

```bash
dotnet run cookbook/Data/04-json-and-json-lines.cs
```

## Next

- [Schemas and columns](schemas-and-columns.md)
- [Reading rows](reading-rows.md)
- [Materialization](materialization.md)
