# Models and Persistence

This track explains what an HPD ML model contains, how prediction works, what
can be inspected, and which parts of ZIP persistence are complete in published
0.5.0.

- [Model contracts](model-contracts.md)
- [Prediction and output schemas](prediction-and-output-schemas.md)
- [Learned parameters](learned-parameters.md)
- [Saving models](saving-models.md)
- [Loading and validation](loading-and-validation.md)
- [ZIP format](zip-format.md)
- [Inference state](inference-state.md)
- [Versioning and deployment](versioning-and-deployment.md)
- [Troubleshooting](troubleshooting.md)

## Current support boundary

`IModel` combines an executable `ITransform` with `ILearnedParameters`.
Prediction uses the transform, not the parameter object directly.

Published `HPD-ML-Serialization-Zip` 0.5.0 has built-in round-trip support for
`NeuralNetworkParameters`. It does not reconstruct saved topology: every loaded
model receives an identity transform. Rebuild neural-network scoring explicitly
after loading parameters.

Other built-in parameter types do not have registered writers in 0.5.0.
Saving them can create a JSON fallback entry, but loading that fallback fails.

The internal
`HPD.ML.ModelsAndPersistence.v0.6.0.Proposal.md` proposal describes complete
topology reconstruction, stable codecs, typed checkpoints, archive validation,
and immutable parameter exposure. Those are proposed changes, not current
package behavior.

## Cookbook

Run the numbered examples under
[`cookbook/Models`](https://github.com/HPD-AI/HPD-ML-Framework/tree/main/cookbook/Models).
They use published `HPD-ML-*` version `0.5.0` packages.
