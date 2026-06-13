# Install HPD ML

HPD ML is distributed as a set of focused NuGet packages. Install the packages
for the workload you need rather than adding every package to an application.

## Requirements

- .NET 10 SDK
- A project that targets `net10.0`
- Access to [NuGet.org](https://www.nuget.org/)

HPD ML uses C# 14 extension members for API discovery. The .NET 10 SDK provides
the required compiler support.

Check your installed SDK:

```bash
dotnet --version
```

## Install the core package

`HPD-ML-Core` contains the standard schema, data-handle, transform-composition,
model, and execution implementations:

```bash
dotnet add package HPD-ML-Core
```

Most applications also install a data-source package and one workload package.
For example, a binary-classification application can start with:

```bash
dotnet add package HPD-ML-DataSources
dotnet add package HPD-ML-BinaryClassification
dotnet add package HPD-ML-Evaluation
```

See [Package selection](package-selection.md) for the complete package map.

## Use a file-based app

For experiments and cookbook recipes, .NET file-based apps keep the package
requirements beside the code:

```csharp
#:package HPD-ML-Core
#:property TargetFramework=net10.0

using HPD.ML.Core;

var data = InMemoryDataHandle.FromColumns(
    ("Temperature", new float[] { 18.5f, 21.0f, 23.5f }));

Console.WriteLine($"Rows: {data.RowCount}");
Console.WriteLine($"Column: {data.Schema.Columns[0].Name}");
```

Save the code as a `.cs` file and run it directly:

```bash
dotnet run app.cs
```

The complete runnable check is
[`cookbook/GettingStarted/01-installation-check.cs`](https://github.com/HPD-AI/HPD-ML-Framework/blob/main/cookbook/GettingStarted/01-installation-check.cs).

## Verify the result

The installation check prints:

```text
HPD ML is ready.
Rows: 3
Column: Temperature (Single)
```

This confirms that NuGet restored the package and that HPD ML can construct and
inspect a schema-bearing `IDataHandle`.

## Troubleshooting

### The SDK does not recognize `#:package`

File-based apps require the .NET 10 SDK. Confirm that `dotnet --version` starts
with `10.` and that an older SDK is not selected by a `global.json` file.

### NuGet cannot find an HPD ML package

Confirm that NuGet.org is enabled as a package source:

```bash
dotnet nuget list source
```

The package IDs contain hyphens, for example `HPD-ML-Core` and
`HPD-ML-DataSources`.

### The compiler cannot find an HPD ML namespace

Installing `HPD-ML-Core` does not install every workload. Add the package that
owns the API and import its namespace. The package-selection guide identifies
the package for each workload.

### Native backend loading fails

The core package uses managed execution and does not require MLX or PJRT.
Accelerated deep-learning backends have additional native runtime and platform
requirements covered by the [backend guides](../backends/index.md).

## Next steps

- [Choose packages](package-selection.md)
- [Understand the core workflow](core-workflow.md)
- [Train your first model](first-model.md)
