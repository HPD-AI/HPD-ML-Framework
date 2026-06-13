# Reproducibility

Reproducibility in 0.5.0 is workload-specific. There is no framework-wide seed
precedence rule.

## Seed every random boundary

Set explicit seeds for:

- train/test splits and shuffles;
- clustering initialization;
- SDCA and linear SVM row order;
- permutation feature importance;
- LightGBM options;
- deep-learning execution environments.

When no option seed is supplied, many APIs use `Random.Shared`. Deep Learning
uses `Environment.Seed`, defaulting to zero when no environment seed exists.
Other managed learners commonly ignore the environment seed and read only
their own options.

`DefaultExecutionEnvironment.CreateChild()` derives an unspecified child seed
as parent seed plus one in 0.5.0. Record the resulting seed rather than relying
on implicit derivation across releases.

## Determinism is more than a seed

Record:

- package versions;
- input schema, order, and vector dimensions;
- split and shuffle seeds;
- learner options;
- backend, device, runtime, and architecture;
- native-library and driver versions;
- probe inputs and expected outputs with tolerances.

Native parallel reductions and different backends may produce numerically
different results even when seeds match. Prefer tolerance-based validation
unless a workload explicitly promises bitwise identity.

## Run the recipe

```bash
dotnet run cookbook/Operations/02-reproducible-splits.cs
```

