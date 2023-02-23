const { readGitignoreFiles } = require('eslint-gitignore');

module.exports = {
  root: true,
  ignorePatterns: [...readGitignoreFiles()],
  env: {
    browser: true,
    node: true,
  },
  plugins: ['simple-import-sort'],
  extends: [
    'eslint:recommended',
    'plugin:import/recommended',
    'plugin:vue/vue3-recommended',
    'plugin:vue-scoped-css/vue3-recommended',
    'airbnb-base',
    'plugin:prettier/recommended',
    // vite 启动时自动生成
    './.eslintrc-auto-import.json',
  ],
  parser: 'vue-eslint-parser',
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module',
    ecmaFeatures: {
      jsx: true,
    },
  },
  rules: {
    'simple-import-sort/imports': 'error',
    'simple-import-sort/exports': 'error',
    // import 可以从 devDependencies 中导入
    'import/no-extraneous-dependencies': ['error', { devDependencies: true }],
    'import/no-unresolved': 'off',
    'import/extensions': 'off',
    'func-names': 'off',
    'prefer-regex-literals': 'off',
    'no-console': 'off',
    'no-alert': 'off',
    'no-return-assign': 'off',
    'no-unused-expressions': 'off',
    'no-param-reassign': 'off',
    'no-unused-vars': 'warn',
    'no-empty': 'warn',
    'vue/multi-word-component-names': 'off',
    'vue/no-v-html': 'off',
  },
  globals: {
    defineOptions: 'readonly',
  },
};
