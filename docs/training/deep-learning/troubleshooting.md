# Troubleshooting

## A multi-output network returns one value

This is a 0.5.0 scoring defect. Training accepts vector labels, but the
transform publishes only the first final-layer output as scalar `Score`.

## Changing `BatchSize` does nothing on managed CPU

The managed trainer ignores `BatchSize` and updates after every row. MLX and
PJRT providers use real batches, so the option has provider-dependent meaning.

## Validation or an initial model has no effect

Both are silently ignored in 0.5.0. Do not use them for model selection or
warm start.

## Progress shows only completion

No epoch or loss events are emitted. This is current behavior.

## Cancellation does not stop an active fit

`FitAsync` cancellation controls task scheduling only. Active materialization
and training do not inspect the token.

## Selecting MLX or PJRT says no provider is registered

Installing `HPD-ML-Backends` does not register providers. Pass
`MlxDeepLearningBackendProvider` or `PjrtDeepLearningBackendProvider` to the
learner explicitly.

## A native provider cannot find its library

The published backend package contains no MLX native library or PJRT plugin.
Supply a compatible runtime and configure its path. See the backend guides.

## Predictions fail for a short feature vector

Supply exactly the first layer's input width. Prediction lacks a clear
preflight shape error in 0.5.0.

## Can I mutate weights for experimentation?

The arrays are publicly mutable, but mutation changes model behavior without
validation. Treat them as read-only.

## Is this a transformer framework?

No. Published 0.5.0 is a dense-network foundation. Transformers, attention,
mixed precision, and distributed training are roadmap items after the
foundation contracts are completed.

