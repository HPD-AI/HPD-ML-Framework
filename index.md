---
layout: home
title: HPD ML Framework
description: Composable machine learning for .NET, from data to deployment.
---
<section class="ml-hero">
  <div class="ml-hero-copy">
    <div class="ml-kicker">
      <span class="ml-kicker-dot"></span>
      Machine learning, shaped for .NET
    </div>
    <h1>Build the model.<br><span>See the whole system.</span></h1>
    <p class="ml-hero-lede">
      HPD ML turns data, transforms, learners, evaluation, and deployment
      into one composable workflow, with portable managed execution and
      explicit accelerator backends when you need them.
    </p>
    <div class="ml-actions">
      <a class="ml-button ml-button-primary" href="./docs/getting-started/first-model">
        Train your first model
        <span aria-hidden="true">→</span>
      </a>
      <a class="ml-button ml-button-secondary" href="./docs/concepts/architecture-and-packages">
        Explore the architecture
      </a>
    </div>
    <div class="ml-hero-meta" aria-label="Framework highlights">
      <span><b>01</b> File-based cookbooks</span>
      <span><b>02</b> Async-first training</span>
      <span><b>03</b> Portable model artifacts</span>
    </div>
  </div>
  <div class="ml-workbench" aria-label="HPD ML workflow preview">
    <div class="ml-workbench-bar">
      <div class="ml-window-dots" aria-hidden="true"><i></i><i></i><i></i></div>
      <span>training-run.cs</span>
      <span class="ml-run-state"><i></i> completed</span>
    </div>
    <div class="ml-pipeline">
      <div class="ml-pipeline-node ml-node-data">
        <span class="ml-node-icon">
          <svg viewBox="0 0 28 28" aria-hidden="true">
            <ellipse cx="14" cy="6" rx="8" ry="3.5"/>
            <path d="M6 6v7c0 2 3.6 3.5 8 3.5s8-1.5 8-3.5V6M6 13v7c0 2 3.6 3.5 8 3.5s8-1.5 8-3.5v-7"/>
          </svg>
        </span>
        <small>INPUT</small>
        <strong>IDataHandle</strong>
        <em>8,240 rows</em>
      </div>
      <span class="ml-flow-arrow" aria-hidden="true">→</span>
      <div class="ml-pipeline-node ml-node-transform">
        <span class="ml-node-icon">
          <svg viewBox="0 0 28 28" aria-hidden="true">
            <path d="M5 7h18M8 14h12M11 21h6"/>
            <circle cx="9" cy="7" r="2"/><circle cx="17" cy="14" r="2"/><circle cx="14" cy="21" r="2"/>
          </svg>
        </span>
        <small>PREPARE</small>
        <strong>ITransform</strong>
        <em>6 features</em>
      </div>
      <span class="ml-flow-arrow" aria-hidden="true">→</span>
      <div class="ml-pipeline-node ml-node-learn">
        <span class="ml-node-icon">
          <svg viewBox="0 0 28 28" aria-hidden="true">
            <circle cx="7" cy="14" r="3"/><circle cx="21" cy="7" r="3"/><circle cx="21" cy="21" r="3"/>
            <path d="M10 13l8-5M10 15l8 5"/>
          </svg>
        </span>
        <small>FIT</small>
        <strong>ILearner</strong>
        <em>logistic</em>
      </div>
    </div>
    <div class="ml-metrics">
      <div class="ml-metric-main">
        <span>VALIDATION ACCURACY</span>
        <strong>94.8<small>%</small></strong>
        <div class="ml-sparkline" aria-hidden="true">
          <svg viewBox="0 0 240 58" preserveAspectRatio="none">
            <defs>
              <linearGradient id="mlArea" x1="0" x2="0" y1="0" y2="1">
                <stop offset="0" stop-color="#6c5ce7" stop-opacity=".28"/>
                <stop offset="1" stop-color="#6c5ce7" stop-opacity="0"/>
              </linearGradient>
            </defs>
            <path class="ml-spark-area" d="M0 50L24 44L48 46L72 32L96 35L120 22L144 27L168 15L192 18L216 8L240 11V58H0Z"/>
            <path class="ml-spark-line" d="M0 50L24 44L48 46L72 32L96 35L120 22L144 27L168 15L192 18L216 8L240 11"/>
          </svg>
        </div>
      </div>
      <div class="ml-metric-stack">
        <div><span>AUC</span><strong>0.972</strong></div>
        <div><span>F1 SCORE</span><strong>0.941</strong></div>
        <div><span>FIT TIME</span><strong>1.2s</strong></div>
      </div>
    </div>
    <div class="ml-backend-row">
      <span>Execution</span>
      <b class="is-active">Managed CPU</b>
      <b>MLX</b>
      <b>PJRT</b>
      <span class="ml-model-ready">model.zip ready</span>
    </div>
  </div>
</section>
<section class="ml-stage-strip" aria-label="The HPD ML workflow">
  <a href="./docs/data/">
    <span>01</span>
    <div><strong>Load</strong><small>Schema-bearing data handles</small></div>
    <i>→</i>
  </a>
  <a href="./docs/transforms/">
    <span>02</span>
    <div><strong>Prepare</strong><small>Composable feature transforms</small></div>
    <i>→</i>
  </a>
  <a href="./docs/training/">
    <span>03</span>
    <div><strong>Train</strong><small>Learners across workloads</small></div>
    <i>→</i>
  </a>
  <a href="./docs/evaluation/">
    <span>04</span>
    <div><strong>Evaluate</strong><small>Metrics and validation</small></div>
    <i>→</i>
  </a>
  <a href="./docs/models/">
    <span>05</span>
    <div><strong>Ship</strong><small>Portable models and inference</small></div>
  </a>
</section>
<section class="ml-idea-section">
  <div class="ml-section-heading">
    <p class="ml-kicker">One vocabulary, end to end</p>
    <h2>The workflow stays legible as the model gets serious.</h2>
    <p>
      HPD ML keeps the boundaries visible. Data remains data. Learning produces
      a model. Models expose transforms. Evaluation is another inspectable data flow.
    </p>
  </div>
  <div class="ml-contract-grid">
    <a class="ml-contract-card ml-contract-data" href="./docs/concepts/data-handles-rows-and-cursors">
      <span class="ml-card-index">DATA / 01</span>
      <svg viewBox="0 0 80 80" aria-hidden="true">
        <rect x="9" y="15" width="62" height="50" rx="8"/>
        <path d="M9 30h62M29 30v35M50 30v35M16 23h18"/>
      </svg>
      <h3>Data with a contract</h3>
      <p>Rows, schemas, cursors, ordering, and materialization travel together through <code>IDataHandle</code>.</p>
      <b>Understand data handles →</b>
    </a>
    <a class="ml-contract-card ml-contract-compose" href="./docs/concepts/transforms-and-composition">
      <span class="ml-card-index">COMPOSE / 02</span>
      <svg viewBox="0 0 80 80" aria-hidden="true">
        <circle cx="16" cy="40" r="7"/><circle cx="40" cy="20" r="7"/><circle cx="40" cy="60" r="7"/><circle cx="64" cy="40" r="7"/>
        <path d="M23 37l10-11M23 43l10 11M47 23l10 11M47 57l10-11"/>
      </svg>
      <h3>Composition you can inspect</h3>
      <p>Chain fixed and learned transforms without hiding execution, schema changes, or fitted parameters.</p>
      <b>Compose a pipeline →</b>
    </a>
    <a class="ml-contract-card ml-contract-runtime" href="./docs/backends/">
      <span class="ml-card-index">RUNTIME / 03</span>
      <svg viewBox="0 0 80 80" aria-hidden="true">
        <rect x="15" y="15" width="50" height="50" rx="10"/>
        <path d="M25 40h30M40 25v30M15 29H7M15 51H7M65 29h8M65 51h8M29 15V7M51 15V7M29 65v8M51 65v8"/>
      </svg>
      <h3>Backends are explicit</h3>
      <p>Start with portable managed execution, then select MLX or PJRT when the workload and runtime justify it.</p>
      <b>Choose a backend →</b>
    </a>
  </div>
</section>
<section class="ml-code-section">
  <div class="ml-code-copy">
    <p class="ml-kicker">The shortest useful path</p>
    <h2>From rows to evaluated model in one readable flow.</h2>
    <p>
      The core API reads in the same order you reason about the work:
      fit, transform, evaluate. No separate orchestration language is required.
    </p>
    <ul>
      <li><span>1</span> Use normal C# and normal NuGet packages.</li>
      <li><span>2</span> Train asynchronously with cancellation and progress.</li>
      <li><span>3</span> Keep predictions and metrics schema-bearing.</li>
    </ul>
    <a href="./docs/getting-started/core-workflow">Walk through the core workflow →</a>
  </div>
  <div class="ml-code-window">
    <div class="ml-code-title">
      <span>Program.cs</span>
      <span>HPD ML</span>
    </div>
    <div class="ml-code-body">
      <div><span class="ml-code-dim">// Data keeps its schema and execution contract.</span></div>
      <div><span class="ml-code-key">var</span> data = InMemoryDataHandle.FromColumns(</div>
      <div>&nbsp;&nbsp;&nbsp;&nbsp;(<span class="ml-code-string">"Features"</span>, features),</div>
      <div>&nbsp;&nbsp;&nbsp;&nbsp;(<span class="ml-code-string">"Label"</span>, labels));</div>
      <div>&nbsp;</div>
      <div><span class="ml-code-dim">// A learner fits parameters and returns a model.</span></div>
      <div><span class="ml-code-key">var</span> learner = ILearner.LogisticRegression();</div>
      <div><span class="ml-code-key">var</span> model = <span class="ml-code-key">await</span> learner.FitAsync(</div>
      <div>&nbsp;&nbsp;&nbsp;&nbsp;<span class="ml-code-key">new</span> LearnerInput(data));</div>
      <div>&nbsp;</div>
      <div><span class="ml-code-dim">// Scoring and evaluation remain composable transforms.</span></div>
      <div><span class="ml-code-key">var</span> scored = model.Transform.Apply(testData);</div>
      <div><span class="ml-code-key">var</span> metrics = ITransform</div>
      <div>&nbsp;&nbsp;&nbsp;&nbsp;.BinaryClassificationMetrics()</div>
      <div>&nbsp;&nbsp;&nbsp;&nbsp;.Apply(scored);</div>
    </div>
    <div class="ml-code-output">
      <span>OUTPUT</span>
      <b>Accuracy 94.8%</b>
      <b>AUC 0.972</b>
      <i>✓ model ready</i>
    </div>
  </div>
</section>
<section class="ml-workloads">
  <div class="ml-section-heading ml-heading-row">
    <div>
      <p class="ml-kicker">Choose your workload</p>
      <h2>Classical ML, sequences, and neural networks.</h2>
    </div>
    <a href="./docs/training/">View all training guides →</a>
  </div>
  <div class="ml-workload-grid">
    <a href="./docs/training/binary-classification/"><span>BC</span><strong>Binary classification</strong><small>Logistic regression, SDCA, SVM, perceptron</small></a>
    <a href="./docs/training/regression/"><span>RG</span><strong>Regression</strong><small>OLS, SDCA, online gradient descent, Poisson</small></a>
    <a href="./docs/training/clustering/"><span>CL</span><strong>Clustering</strong><small>K-means and mini-batch K-means</small></a>
    <a href="./docs/training/time-series/"><span>TS</span><strong>Time series</strong><small>Anomaly detection, change points, forecasting</small></a>
    <a href="./docs/training/deep-learning/"><span>DL</span><strong>Deep learning</strong><small>Dense networks with managed, MLX, and PJRT backends</small></a>
    <a href="./docs/training/lightgbm/"><span>LG</span><strong>LightGBM</strong><small>Tree ensembles with explicit native runtime setup</small></a>
  </div>
</section>
<section class="ml-bottom-grid">
  <div class="ml-cookbook-panel">
    <p class="ml-kicker">Executable documentation</p>
    <h2>Learn from programs, not fragments.</h2>
    <p>
      Every cookbook entry is a complete, numbered C# file with its own
      package references. Open one, run it, change it, and keep the result.
    </p>
    <div class="ml-file-list">
      <span><b>01</b> installation-check.cs <i>ready</i></span>
      <span><b>05</b> training.cs <i>ready</i></span>
      <span><b>07</b> evaluation.cs <i>ready</i></span>
      <span><b>10</b> complete-model.cs <i>ready</i></span>
    </div>
    <a class="ml-button ml-button-light" href="https://github.com/HPD-AI/HPD-ML-Framework/tree/main/cookbook/GettingStarted">
      Open the cookbook
      <span aria-hidden="true">→</span>
    </a>
  </div>
  <div class="ml-reference-panel">
    <span class="ml-reference-count">279</span>
    <p class="ml-kicker">Generated API reference</p>
    <h2>The shipped surface, indexed.</h2>
    <p>
      Browse packages, namespaces, declarations, nullable contracts,
      defaults, and members generated directly from published assemblies.
    </p>
    <a href="./docs/reference/api/">Browse the API reference →</a>
  </div>
</section>
<section class="ml-final-cta">
  <div>
    <span class="ml-kicker">Start with the complete loop</span>
    <h2>Load. Prepare. Train. Evaluate. Ship.</h2>
  </div>
  <a class="ml-button ml-button-primary" href="./docs/getting-started/">
    Start learning
    <span aria-hidden="true">→</span>
  </a>
</section>
