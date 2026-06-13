#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Read deterministic execution requests and derive a child environment.

using HPD.ML.Abstractions;
using HPD.ML.Core;

var parent = new DefaultExecutionEnvironment(
    seed: 42,
    defaultDevicePreference: new DevicePreference("cpu"),
    backend: BackendSpec.Cpu());

var child = parent.CreateChild();

Console.WriteLine($"Parent seed: {parent.Seed}");
Console.WriteLine($"Child seed: {child.Seed}");
Console.WriteLine($"Backend: {child.Backend}");
Console.WriteLine($"Device: {child.DefaultDevicePreference.DeviceId}");

Random first = SeededRandom.Create(parent.Seed);
Random second = SeededRandom.Create(parent.Seed);

Console.WriteLine(
    $"Deterministic: {first.Next() == second.Next()}");
