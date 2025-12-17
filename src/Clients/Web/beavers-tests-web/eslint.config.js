import js from '@eslint/js'
import globals from 'globals'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import tseslint from 'typescript-eslint'
import { defineConfig, globalIgnores } from 'eslint/config'

export default defineConfig([
  globalIgnores([
    'dist',
    'node_modules',
    '*.config.js',
  ]),
  
  {
    files: ['**/*.{ts,tsx}'],
    
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    
    languageOptions: {
      ecmaVersion: 2020, // ES2020 = ES11
      
      globals: {
        ...globals.browser,
        ...globals.es2020,
        ...globals.node
      },
      
      parserOptions: {
        project: './tsconfig.json',
        sourceType: 'module',
        
        ecmaFeatures: {
          jsx: true,
        },
      },
    },
    
    rules: {
      // Разрешить использование any типа (можно изменить на "error" для строгости)
      '@typescript-eslint/no-explicit-any': 'warn',
      
      '@typescript-eslint/no-unused-vars': [
        'error', 
        { 
          argsIgnorePattern: '^_',
          varsIgnorePattern: '^_',
          caughtErrorsIgnorePattern: '^_',
        }
      ],
      
      '@typescript-eslint/explicit-function-return-type': [
        'warn',
        {
          allowExpressions: true,
          allowHigherOrderFunctions: true,
        },
      ],
      
      'react-refresh/only-export-components': [
        'warn',
        { allowConstantExport: true },
      ],
      
      'no-unused-vars': 'off',
    },
  },
  
  {
    files: ['**/*.js'],
    extends: [js.configs.recommended],
    languageOptions: {
      ecmaVersion: 2020,
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      sourceType: 'module',
    },
  },
])