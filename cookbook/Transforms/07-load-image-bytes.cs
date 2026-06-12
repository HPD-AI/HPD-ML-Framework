#:package HPD-ML-Core@0.5.0
#:package HPD-ML-Transforms@0.5.0
#:property TargetFramework=net10.0

// Load file bytes through the supported 0.5.0 image transform.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.Transforms;

var path = Path.Combine(
    Path.GetTempPath(),
    $"hpd-ml-image-{Guid.NewGuid():N}.bin");

try
{
    byte[] expected = [0x89, 0x50, 0x4E, 0x47];
    File.WriteAllBytes(path, expected);

    IDataHandle paths = InMemoryDataHandle.FromColumns(
        ("Path", new[] { path }));

    IDataHandle loaded = ITransform.LoadImage("Path").Apply(paths);

    using var rows = loaded.GetCursor(["Image"]);
    rows.MoveNext();
    byte[] actual = rows.Current.GetValue<byte[]>("Image");

    Console.WriteLine($"Loaded {actual.Length} bytes");
    Console.WriteLine($"Matches source: {actual.SequenceEqual(expected)}");
}
finally
{
    if (File.Exists(path))
        File.Delete(path);
}

