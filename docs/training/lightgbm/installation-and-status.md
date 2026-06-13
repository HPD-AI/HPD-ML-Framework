# Installation and Package Status

Install the published managed package:

```bash
dotnet add package HPD-ML-LightGBM
```

Version `0.5.0` depends on `HPD-ML-Core` and targets `net10.0`.

## What the package contains

Inspection of the published `HPD-ML-LightGBM` 0.5.0 package found:

```text
lib/net10.0/HPD.ML.LightGBM.dll
lib/net10.0/HPD.ML.LightGBM.xml
README.md
LICENSE
```

It does not contain:

```text
runtimes/<rid>/native/...
lib_lightgbm
a dependency on a LightGBM runtime package
```

The P/Invoke layer requests a library named `lib_lightgbm`. Therefore package
restore alone is not enough to train.

## Verified clean-install behavior

The managed package loads and this works:

```csharp
ILearner learner = ILearner.LightGbmRegression();
ISchema output = learner.GetOutputSchema(inputSchema);
```

The first native dataset call on a machine without a separately installed
library throws:

```text
System.DllNotFoundException
Unable to load shared library 'lib_lightgbm' or one of its dependencies.
```

The complete loader message is platform-specific.

## Bringing your own native library

Published 0.5.0 does not define or certify:

- a supported LightGBM native version;
- supported operating systems or architectures;
- ABI compatibility;
- required build flags;
- installation paths;
- transitive system-library requirements.

Supplying a native library manually may allow training, but that path is not
package-certified. It also does not prove that the exported model is scored
identically by the managed parser.

## Recommendation

Use this package for evaluation and API exploration only. Keep production
training on a separately certified LightGBM integration until HPD ML ships
native runtime assets and native-versus-managed parity tests.

## Complete availability probe

This example requires only the two published packages:

```csharp
#:package HPD-ML-Core@0.5.0
#:package HPD-ML-LightGBM@0.5.0
#:property TargetFramework=net10.0

using System.Runtime.InteropServices;
using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.LightGBM;

Console.WriteLine($"Runtime: {RuntimeInformation.RuntimeIdentifier}");

IDataHandle data = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f], [2f], [3f] }),
    ("Label", new[] { 0f, 1f, 2f, 3f }));

ILearner learner = ILearner.LightGbmRegression(
    options: new LightGbmOptions
    {
        NumberOfIterations = 2,
        MinDataInLeaf = 1
    });

Console.WriteLine("Managed LightGBM API loaded.");

try
{
    learner.Fit(new LearnerInput(data));
    Console.WriteLine("Native training is available.");
}
catch (DllNotFoundException)
{
    Console.WriteLine(
        "Native training is unavailable: lib_lightgbm was not found.");
    Console.WriteLine(
        "HPD-ML-LightGBM 0.5.0 does not ship native runtime assets.");
}
```

In a C# file-based app, run the file with `dotnet run <file>.cs`. In an SDK
project, remove the `#:` directives and use normal package references.

## Next

- [Data contracts](data-contracts.md)
- [Troubleshooting](troubleshooting.md)
