#:package HPD-ML-Core@0.5.0
#:package HPD-ML-DeepLearning@0.5.0
#:property TargetFramework=net10.0

// Match an explicit backend request with a custom Deep Learning provider.

using HPD.ML.Abstractions;
using HPD.ML.Core;
using HPD.ML.DeepLearning;
using HPD.ML.DeepLearning.Backends;

IDataHandle training = InMemoryDataHandle.FromColumns(
    ("Features", new float[][] { [0f], [1f] }),
    ("Label", new[] { 0f, 1f }));

var definition = new NeuralNetworkDefinition(
    "Features",
    "Label",
    [new DenseLayerSpec(1, 1)]);

var provider = new FixedBackendProvider(definition);
var learner = new NeuralNetworkLearner(
    definition,
    backendProviders: [provider]);

IModel model = learner.Fit(
    new LearnerInput(
        training,
        Environment: new DefaultExecutionEnvironment(
            seed: 7,
            backend: new BackendSpec("fixed"))));

var parameters = (NeuralNetworkParameters)model.Parameters;
Console.WriteLine($"Provider created: {provider.WasCreated}");
Console.WriteLine($"Weight: {parameters.Weights[0][0]:F1}");

sealed class FixedBackendProvider(
    NeuralNetworkDefinition definition)
    : IDeepLearningBackendProvider
{
    public bool WasCreated { get; private set; }

    public bool CanHandle(BackendSpec backend) =>
        string.Equals(
            backend.Kind,
            "fixed",
            StringComparison.OrdinalIgnoreCase);

    public DeepLearningBackendCapabilities GetCapabilities(
        BackendSpec backend) =>
        new()
        {
            Name = "fixed",
            SupportsTraining = true,
            SupportsCpu = true,
            SupportsFloat32 = true,
            SupportedActivations =
                new HashSet<ActivationKind>
                {
                    ActivationKind.Identity
                }
        };

    public IDeepLearningTrainer CreateTrainer(
        DeepLearningBackendContext context)
    {
        WasCreated = true;
        return new FixedTrainer(definition);
    }
}

sealed class FixedTrainer(
    NeuralNetworkDefinition definition) : IDeepLearningTrainer
{
    public NeuralNetworkParameters Train(
        NeuralNetworkDefinition requestedDefinition,
        float[][] features,
        float[][] labels,
        TrainingOptions options,
        int seed) =>
        new(definition, [[2.0f]], [[1.0f]]);
}
