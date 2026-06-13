# Native Runtime Operations

Package installation, provider registration, runtime availability, and device
availability are separate deployment states.

## LightGBM

`HPD-ML-LightGBM` 0.5.0 contains managed bindings but does not ship
`lib_lightgbm` or a runtime-package dependency. Native training requires the
library to be installed where the platform loader can resolve it.

## MLX

`HPD-ML-Backends` contains the MLX provider and resolver but no MLX native
library. Resolution can use explicit `MlxRuntimeOptions`, the
`HELIUM_MLX_LIBRARY_PATH` environment variable, or prepared runtime
directories.

## PJRT

The same package contains PJRT provider code but no PJRT plugin. Resolution
checks explicit configuration, environment variables, and prepared runtime
directories. CPU, CUDA, and ROCm names in resolver code are not proof that a
compatible plugin, driver, device, and architecture are present.

## Startup checks

For every native workload, distinguish:

```text
managed package present
provider registered
request matched
capability accepted
native file found
library loaded
device initialized
small operation completed
```

Fail startup for an explicit required backend. Do not silently switch an
explicit GPU request to CPU unless the application has opted into fallback.
Log paths and runtime identities without exposing secrets.

## Run the recipe

```bash
dotnet run cookbook/Operations/06-native-runtime-inventory.cs
```

