# Lifecycle and Concurrency

Published 0.5.0 does not define one concurrency contract for learners,
transforms, models, or data handles.

## Ownership

The caller owns every `IRowCursor` it requests and should dispose it. A cursor
implementation owns file readers or streams it opens. Subscribers own their
progress subscriptions.

Deep Learning disposes a disposable trainer after each fit. MLX and PJRT
tensor backends and tensors are disposable; direct backend consumers must
dispose them deterministically.

## Reuse

- Treat learner instances as single-fit-at-a-time.
- Do not run concurrent fits on one learner instance.
- Stateless scoring transforms are reasonable candidates for reuse, but verify
  the concrete transform.
- Stateful time-series transforms allocate state per cursor. Do not share one
  cursor across requests.
- Do not retain cursor-backed `IRow` instances after advancing the cursor.
  Copy required values.

Some parameter objects expose mutable arrays in 0.5.0. Never mutate parameters
shared by live inference. Copy arrays before inspection or experimentation.

## Deployment pattern

Construct and validate a model completely, then publish the model reference to
readers. Do not mutate its parameters or replace internal state in place.
For stateful inference, keep explicit per-stream or per-request sessions.

The 0.6.0 proposals add explicit reuse, concurrency, row-lifetime, parameter
immutability, and disposal contracts. Until those are package-certified,
application isolation remains necessary.

## Run the recipe

```bash
dotnet run cookbook/Operations/03-lifecycle-and-cursors.cs
```

