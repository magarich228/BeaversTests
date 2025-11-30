import React, { useState } from 'react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../../stores';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';

export const LoginForm: React.FC = observer(() => {
  const { authStore } = useStore();
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });

  const handleSubmit = async (e: React.FormEvent): Promise<void> => {
    e.preventDefault();
    await authStore.login(formData);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>): void => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    authStore.clearError();
  };

  return (
    <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-6)' }}>
      <div style={{ display: 'flex', flexDirection: 'column', gap: 'var(--space-4)' }}>
        <Input
          label="Email Address"
          type="email"
          name="email"
          value={formData.email}
          onChange={handleChange}
          placeholder="Enter your email"
          required
          error={authStore.error?.includes('email') ? authStore.error : undefined}
        />
        
        <Input
          label="Password"
          type="password"
          name="password"
          value={formData.password}
          onChange={handleChange}
          placeholder="Enter your password"
          required
        />
      </div>

      <div style={{ textAlign: 'right' }}>
        <button 
          type="button"
          style={{
            background: 'none',
            border: 'none',
            color: 'var(--primary)',
            fontSize: 'var(--font-size-sm)',
            fontWeight: '500',
            cursor: 'pointer'
          }}
        >
          Forgot your password?
        </button>
      </div>

      {authStore.error && !authStore.error.includes('email') && (
        <div style={{
          background: 'rgba(239, 68, 68, 0.1)',
          border: '1px solid rgba(239, 68, 68, 0.2)',
          borderRadius: 'var(--radius-xl)',
          padding: 'var(--space-4)'
        }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-3)' }}>
            <svg style={{ width: '1.25rem', height: '1.25rem', color: 'var(--error)', flexShrink: 0 }} viewBox="0 0 20 20" fill="currentColor">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
            </svg>
            <p style={{ color: 'var(--error)', fontSize: 'var(--font-size-sm)', margin: 0 }}>
              {authStore.error}
            </p>
          </div>
        </div>
      )}

      <Button 
        type="submit" 
        loading={authStore.isLoading}
        style={{ width: '100%', padding: 'var(--space-4)' }}
      >
        Sign In
      </Button>
    </form>
  );
});