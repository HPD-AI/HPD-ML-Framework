import { defineConfig } from 'vitepress'

const link = (text, path) => ({ text, link: path })
const repositoryName = process.env.GITHUB_REPOSITORY?.split('/')[1]
const base = process.env.GITHUB_ACTIONS && repositoryName ? `/${repositoryName}/` : '/'

const gettingStarted = [
  link('Learning Path', '/docs/getting-started/'),
  link('Installation', '/docs/getting-started/installation'),
  link('Package Selection', '/docs/getting-started/package-selection'),
  link('First Model', '/docs/getting-started/first-model'),
  link('Core Workflow', '/docs/getting-started/core-workflow'),
  link('Loading Data', '/docs/getting-started/loading-data'),
  link('Preparing Data', '/docs/getting-started/preparing-data'),
  link('Training', '/docs/getting-started/training'),
  link('Prediction', '/docs/getting-started/prediction'),
  link('Evaluation', '/docs/getting-started/evaluation'),
  link('Saving And Loading', '/docs/getting-started/saving-and-loading'),
  link('Execution Backends', '/docs/getting-started/execution-backends'),
  link('Next Steps', '/docs/getting-started/next-steps')
]

const sidebar = [
  {
    text: 'Start',
    collapsed: false,
    items: [
      link('HPD ML', '/'),
      link('Documentation', '/docs/'),
      ...gettingStarted
    ]
  },
  {
    text: 'Fundamentals',
    collapsed: true,
    items: [
      link('Core Concepts', '/docs/concepts/'),
      link('Architecture And Packages', '/docs/concepts/architecture-and-packages'),
      link('Data Handles, Rows And Cursors', '/docs/concepts/data-handles-rows-and-cursors'),
      link('Schemas And Field Types', '/docs/concepts/schemas-and-field-types'),
      link('Learners, Models And Parameters', '/docs/concepts/learners-models-and-parameters'),
      link('Transforms And Composition', '/docs/concepts/transforms-and-composition'),
      link('Execution Environments', '/docs/concepts/execution-environments'),
      link('Lazy, Eager And Cached Execution', '/docs/concepts/lazy-eager-and-cached-execution'),
      link('Ordering And Stateful Execution', '/docs/concepts/ordering-and-stateful-execution'),
      link('Sync, Async And Cancellation', '/docs/concepts/sync-async-and-cancellation'),
      link('Contract Boundaries', '/docs/concepts/contract-boundaries-and-troubleshooting')
    ]
  },
  {
    text: 'Data And Features',
    collapsed: true,
    items: [
      link('Data Overview', '/docs/data/'),
      link('CSV', '/docs/data/csv'),
      link('JSON', '/docs/data/json'),
      link('Parquet', '/docs/data/parquet'),
      link('Schemas And Columns', '/docs/data/schemas-and-columns'),
      link('Rows And Cursors', '/docs/data/reading-rows'),
      link('Transforms Overview', '/docs/transforms/'),
      link('Fixed And Learned Transforms', '/docs/transforms/fixed-and-learned-transforms'),
      link('Missing Values', '/docs/transforms/missing-values'),
      link('Normalization', '/docs/transforms/normalization'),
      link('Categorical Encoding', '/docs/transforms/categorical'),
      link('Text Featurization', '/docs/transforms/text'),
      link('Feature Selection', '/docs/transforms/feature-selection'),
      link('Images', '/docs/transforms/image-support')
    ]
  },
  {
    text: 'Training',
    collapsed: false,
    items: [
      link('Training Overview', '/docs/training/'),
      link('Binary Classification', '/docs/training/binary-classification/'),
      link('Regression', '/docs/training/regression/'),
      link('Clustering', '/docs/training/clustering/'),
      link('Time Series', '/docs/training/time-series/'),
      link('Deep Learning', '/docs/training/deep-learning/'),
      link('LightGBM', '/docs/training/lightgbm/')
    ]
  },
  {
    text: 'Evaluate And Ship',
    collapsed: true,
    items: [
      link('Evaluation', '/docs/evaluation/'),
      link('Choosing An Evaluator', '/docs/evaluation/choosing-an-evaluator'),
      link('Binary Classification', '/docs/evaluation/binary-classification'),
      link('Regression', '/docs/evaluation/regression'),
      link('Clustering', '/docs/evaluation/clustering'),
      link('Cross Validation', '/docs/evaluation/cross-validation'),
      link('Feature Importance', '/docs/evaluation/feature-importance'),
      link('Models And Persistence', '/docs/models/'),
      link('Model Contracts', '/docs/models/model-contracts'),
      link('Saving Models', '/docs/models/saving-models'),
      link('Loading And Validation', '/docs/models/loading-and-validation'),
      link('Versioning And Deployment', '/docs/models/versioning-and-deployment')
    ]
  },
  {
    text: 'Runtime',
    collapsed: true,
    items: [
      link('Execution Backends', '/docs/backends/'),
      link('Deep Learning Backends', '/docs/backends/deep-learning'),
      link('MLX', '/docs/backends/mlx'),
      link('PJRT', '/docs/backends/pjrt'),
      link('Operations', '/docs/operations/'),
      link('Performance And Memory', '/docs/operations/performance-and-memory'),
      link('Native Runtime Operations', '/docs/operations/native-runtime-operations'),
      link('Reproducibility', '/docs/operations/reproducibility'),
      link('Deployment Validation', '/docs/operations/deployment-validation'),
      link('Production Checklist', '/docs/operations/production-checklist-and-troubleshooting')
    ]
  },
  {
    text: 'Build On HPD ML',
    collapsed: true,
    items: [
      link('Extending HPD ML', '/docs/extending/'),
      link('Custom Data Handles', '/docs/extending/custom-data-handles'),
      link('Custom Transforms', '/docs/extending/custom-transforms'),
      link('Custom Learners And Models', '/docs/extending/custom-learners-and-models'),
      link('Backend Providers', '/docs/extending/backend-providers'),
      link('Execution Environments', '/docs/extending/execution-environments'),
      link('Testing And Packaging', '/docs/extending/testing-and-packaging')
    ]
  },
  {
    text: 'Reference',
    collapsed: true,
    items: [
      link('Reference Index', '/docs/reference/'),
      link('API Reference', '/docs/reference/api/'),
      link('Packages', '/docs/reference/api/packages/hpd-ml-core'),
      link('Namespaces', '/docs/reference/api/namespaces/hpd-ml-core')
    ]
  }
]

export default defineConfig({
  title: 'HPD ML',
  description: 'Composable machine learning for .NET, from data to deployment.',
  base,
  srcDir: '.',
  outDir: './.site-dist',
  cacheDir: './.site-cache',
  cleanUrls: true,
  lastUpdated: true,

  themeConfig: {
    logo: '/hpd-ml-mark.svg',
    siteTitle: 'HPD ML',

    nav: [
      link('Get Started', '/docs/getting-started/'),
      {
        text: 'Learn',
        items: [
          link('Core Concepts', '/docs/concepts/'),
          link('Data', '/docs/data/'),
          link('Transforms', '/docs/transforms/'),
          link('Training', '/docs/training/'),
          link('Evaluation', '/docs/evaluation/')
        ]
      },
      {
        text: 'Deploy',
        items: [
          link('Models', '/docs/models/'),
          link('Backends', '/docs/backends/'),
          link('Operations', '/docs/operations/')
        ]
      },
      link('API', '/docs/reference/api/'),
      link('Cookbook', 'https://github.com/HPD-AI/HPD-ML-Framework/tree/main/cookbook')
    ],

    sidebar,

    outline: {
      level: [2, 3],
      label: 'On this page'
    },

    search: {
      provider: 'local'
    },

    socialLinks: [
      { icon: 'github', link: 'https://github.com/HPD-AI/HPD-ML-Framework' }
    ],

    footer: {
      message: 'Composable machine learning for .NET.',
      copyright: 'Copyright © 2026 HPD AI'
    }
  },

  markdown: {
    theme: {
      light: 'github-light',
      dark: 'github-dark'
    },
    lineNumbers: true
  },

  head: [
    ['link', { rel: 'icon', type: 'image/svg+xml', href: `${base}hpd-ml-mark.svg` }],
    ['meta', { name: 'theme-color', content: '#5b4bdb' }],
    ['meta', { property: 'og:title', content: 'HPD ML Framework' }],
    ['meta', {
      property: 'og:description',
      content: 'Composable machine learning for .NET, from data to deployment.'
    }]
  ]
})
