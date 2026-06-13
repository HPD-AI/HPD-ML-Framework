# LightGBM Preview

`HPD-ML-LightGBM` 0.5.0 exposes a native LightGBM learner and a managed tree
ensemble scorer. This track is **experimental**: the published package contains
the managed assembly but does not include `lib_lightgbm` or a native runtime
package.

Install:

```bash
dotnet add package HPD-ML-Core
dotnet add package HPD-ML-LightGBM
```

The managed API can be loaded, learners can be configured, and output schemas
can be inspected. Training requires callers to provide a compatible native
LightGBM library. On a clean installation verified on macOS arm64, fitting a
model throws `DllNotFoundException`.

## Current status

| Capability | Published 0.5.0 status |
| --- | --- |
| Learner construction and schema inspection | Available |
| Native training from package installation alone | Unavailable |
| User-supplied `lib_lightgbm` | Intended, not package-certified |
| Managed tree scoring | Available |
| Native-versus-managed prediction parity | Not certified |
| Binary, multiclass, regression, and ranking objectives | Exposed, not end-to-end package-certified |
| Weight column | Internal support only; not publicly wired |
| Ranking group column | Fixed to `GroupId` |
| Categorical training | Not wired through feature metadata |
| Initial model | Ignored |
| Active cancellation | Not supported |

Do not treat the existence of a learner or output schema as proof that native
training is available.

## Learning path

1. [Installation and package status](installation-and-status.md)
2. [Data contracts](data-contracts.md)
3. [Objectives and options](objectives-and-options.md)
4. [Training runtime](training-runtime.md)
5. [Managed scoring and model inspection](managed-scoring-and-model-inspection.md)
6. [Troubleshooting](troubleshooting.md)

## Examples

The guides contain complete examples that require only the published NuGet
packages. You do not need the documentation repository to follow them.

The repository also contains matching files under `cookbook/LightGBM/` for
contributors who have cloned it. Those recipes intentionally do not claim a
successful packaged training workflow. Full native distribution, prediction
parity, and package certification are planned for `0.6.0`.
