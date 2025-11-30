import React, { useEffect } from 'react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../stores';
import { AuthTabs } from '../components/auth/AuthTabs';
import { Loader } from '../components/ui/Loader';
import { useNavigate } from 'react-router-dom';

export const AuthPage: React.FC = observer(() => {
  const { authStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    if (authStore.isAuthenticated) {
      navigate('/dashboard');
    }
  }, [authStore.isAuthenticated, navigate]);

  if (authStore.isLoading && !authStore.error) {
    return (
      <div className="auth-container">
        <div style={{ textAlign: 'center' }}>
          <Loader />
          <p style={{ marginTop: 'var(--space-4)', color: 'var(--gray-600)' }}>Checking authentication...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <div style={{ textAlign: 'center', marginBottom: 'var(--space-8)' }}>
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 'var(--space-3)', marginBottom: 'var(--space-4)' }}>
            <div style={{
              width: '2.5rem',
              height: '2.5rem',
              background: 'var(--gradient-primary)',
              borderRadius: 'var(--radius-xl)',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              color: 'var(--white)',
              fontWeight: 'bold',
              fontSize: 'var(--font-size-lg)'
            }}>
              BT
            </div>
            <h1 style={{ 
              fontSize: 'var(--font-size-3xl)',
              fontWeight: 'bold',
              background: 'var(--gradient-primary)',
              backgroundClip: 'text',
              WebkitBackgroundClip: 'text',
              WebkitTextFillColor: 'transparent'
            }}>
              BeaverTests
            </h1>
          </div>
          <h2 style={{ 
            fontSize: 'var(--font-size-2xl)',
            fontWeight: 'bold',
            color: 'var(--gray-900)',
            marginBottom: 'var(--space-2)'
          }}>
            Welcome Back
          </h2>
          <p style={{ color: 'var(--gray-600)' }}>
            Sign in to your account or create a new one
          </p>
        </div>

        <AuthTabs />
      </div>
    </div>
  );
});