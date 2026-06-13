# Training

The training track will cover learner selection and configuration for binary
classification, regression, clustering, LightGBM, deep learning, and time
series.

Current verified introductions:

- [Binary classification](binary-classification/index.md)
- [Regression](regression/index.md)
- [Clustering](clustering/index.md)
- [Time series](time-series/index.md)
- [LightGBM preview](lightgbm/index.md)
- [Deep Learning](deep-learning/index.md)
- [Training](../getting-started/training.md)
- [Prediction](../getting-started/prediction.md)
- [Evaluation](../getting-started/evaluation.md)
- [Execution backends](../getting-started/execution-backends.md)

LightGBM is not a completed training track in published `0.5.0`. The package
contains its managed API but no native runtime asset, so the guides document
the current boundary rather than presenting a production-ready workflow.

Deep Learning 0.5.0 is documented as a dense-network foundation. Managed CPU
is the portable baseline; optional MLX and PJRT providers require explicit
registration and separately supplied native runtimes.
