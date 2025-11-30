import React, { useState } from 'react';
import { LoginForm } from './LoginForm';
import { RegisterForm } from './RegisterForm';

type AuthTab = 'login' | 'register';

export const AuthTabs: React.FC = () => {
  const [activeTab, setActiveTab] = useState<AuthTab>('login');

  return (
    <div style={{ padding: 'var(--space-8)' }}>
      <div className="auth-tabs">
        <button
          className={`auth-tab ${activeTab === 'login' ? 'active' : ''}`}
          onClick={() => setActiveTab('login')}
        >
          Sign In
        </button>
        <button
          className={`auth-tab ${activeTab === 'register' ? 'active' : ''}`}
          onClick={() => setActiveTab('register')}
        >
          Create Account
        </button>
      </div>

      <div style={{ animation: 'fadeIn 0.3s ease-out' }}>
        {activeTab === 'login' ? <LoginForm /> : <RegisterForm />}
      </div>
    </div>
  );
};