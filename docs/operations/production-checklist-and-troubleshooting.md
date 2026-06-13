# Production Checklist and Troubleshooting

## Release checklist

- Pin and record HPD ML package versions.
- Record schema names, CLR types, vector dimensions, and ordering requirements.
- Bound input rows, dimensions, files, and archive expansion.
- Set explicit seeds at every workload-specific random boundary.
- Measure materialization and training peak memory.
- Define learner, model, cursor, and native-resource ownership.
- Keep fits isolated; do not assume concurrent fitting is supported.
- Subscribe to progress before fitting and treat the task result as authoritative.
- Verify native runtimes and devices with a small startup operation.
- Validate loaded parameter types, rebuild scoring, and run probe rows.
- Publish and run the exact trimmed or AOT deployment artifact.
- Keep a known-good model and application version for rollback.

## Common failures

### Cancellation was requested but training continues

Most 0.5.0 learners do not observe cancellation during active fitting. Isolate
strictly bounded work in a worker process.

### Memory rises sharply during training

The learner likely materializes all rows despite a streaming source. Reduce
rows or dimensions, avoid concurrent fits, and measure the learner's copied
representation.

### The backend provider exists but startup fails

Provider registration does not install a native library or prove device
availability. Report each startup stage separately.

### Loaded predictions equal the input

The 0.5.0 topology loader returned identity. Rebuild the scoring transform from
validated parameters.

### An archive throws an unexpected exception

Reject it. Some malformed entry combinations produce incidental exceptions in
0.5.0. Preserve the original exception for diagnostics.

### Repeated training gives different results

Locate every random boundary, confirm row order, backend and runtime identity,
and compare using workload-appropriate tolerances.

### Progress says complete but the operation failed

Use the returned task or exception as the source of truth. Progress terminal
behavior is not normalized in 0.5.0.

## Run the complete check

```bash
dotnet run cookbook/Operations/09-production-readiness-check.cs
```

