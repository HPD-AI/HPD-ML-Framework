#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Serialization-Zip@0.5.0
#:property TargetFramework=net10.0

// Report the current runtime mode and the boundaries an AOT publish must test.

using System.Runtime.CompilerServices;
using HPD.ML.Serialization.Zip;

Console.WriteLine($"dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");
Console.WriteLine($"dynamic code compiled: {RuntimeFeature.IsDynamicCodeCompiled}");

_ = new ZipSerializer();
Console.WriteLine("ZIP serialization API loaded.");

Console.WriteLine("A real certification must publish the exact application:");
Console.WriteLine("dotnet publish -c Release -r <rid> -p:PublishAot=true");
Console.WriteLine(
    "Exercise model loading, runtime-type JSON paths, native resolution, and probe inference after publish.");

// Guide: docs/operations/native-aot-and-trimming.md
