#:package HPD-ML-Core@0.5.0
#:property TargetFramework=net10.0

// Configure an execution environment and inspect child inheritance.

using HPD.ML.Abstractions;
using HPD.ML.Core;

using var cts = new CancellationTokenSource();

var environment = new DefaultExecutionEnvironment(
    seed: 42,
    cancellationToken: cts.Token,
    defaultDevicePreference: new DevicePreference("cpu"),
    backend: BackendSpec.Cpu());

var child = environment.CreateChild();

Console.WriteLine($"parent seed: {environment.Seed}");
Console.WriteLine($"child seed: {child.Seed}");
Console.WriteLine($"backend: {child.Backend}");
Console.WriteLine($"device: {child.DefaultDevicePreference.DeviceId}");
Console.WriteLine($"same cancellation token: {child.CancellationToken == environment.CancellationToken}");
Console.WriteLine($"scheduler supplied: {child.Scheduler is not null}");

var first = SeededRandom.Create(environment.Seed).Next();
var repeated = SeededRandom.Create(environment.Seed).Next();
Console.WriteLine($"seeded random is repeatable: {first == repeated}");

// Guide: docs/concepts/execution-environments.md
