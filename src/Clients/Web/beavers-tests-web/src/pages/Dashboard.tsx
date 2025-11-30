import React from 'react';
import { observer } from 'mobx-react-lite';
import { useStore } from '../stores';
import { Button } from '../components/ui/Button';
import { useNavigate } from 'react-router-dom';

export const Dashboard: React.FC = observer(() => {
  const { authStore } = useStore();
  const navigate = useNavigate();

  const handleLogout = async (): Promise<void> => {
    await authStore.logout();
    navigate('/');
  };

  if (!authStore.user) {
    return (
      <div className="auth-container">
        <div style={{ textAlign: 'center' }}>
          <div className="loader" style={{ margin: '0 auto var(--space-4) auto' }}></div>
          <p style={{ color: 'var(--gray-600)' }}>Loading your dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: '100vh', background: 'var(--gradient-bg)' }}>
      {/* Навигация */}
      <nav className="dashboard-nav">
        <div className="container">
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', height: '4rem' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-3)' }}>
              <div style={{
                width: '2rem',
                height: '2rem',
                background: 'var(--gradient-primary)',
                borderRadius: 'var(--radius-lg)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: 'var(--white)',
                fontWeight: 'bold',
                fontSize: 'var(--font-size-sm)'
              }}>
                BT
              </div>
              <span style={{ 
                fontWeight: 'bold', 
                fontSize: 'var(--font-size-xl)',
                background: 'var(--gradient-primary)',
                backgroundClip: 'text',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent'
              }}>
                BeaverTests
              </span>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-4)' }}>
              <div style={{ 
                display: 'flex', 
                alignItems: 'center', 
                gap: 'var(--space-3)', 
                background: 'var(--gray-100)',
                borderRadius: 'var(--radius-xl)',
                padding: 'var(--space-2) var(--space-4)'
              }}>
                <div style={{
                  width: '2rem',
                  height: '2rem',
                  background: 'linear-gradient(135deg, var(--success), var(--primary))',
                  borderRadius: '50%',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  color: 'var(--white)',
                  fontWeight: 'bold',
                  fontSize: 'var(--font-size-sm)'
                }}>
                  {authStore.user.displayName?.charAt(0).toUpperCase() || 'U'}
                </div>
                <div style={{ display: 'none', sm: 'block' } as any}>
                  <p style={{ fontSize: 'var(--font-size-sm)', fontWeight: '500', color: 'var(--gray-900)', margin: 0 }}>
                    {authStore.user.displayName}
                  </p>
                  <p style={{ fontSize: 'var(--font-size-xs)', color: 'var(--gray-500)', margin: 0 }}>
                    {authStore.user.email}
                  </p>
                </div>
              </div>
              <Button 
                variant="secondary" 
                onClick={handleLogout}
              >
                Sign Out
              </Button>
            </div>
          </div>
        </div>
      </nav>

      {/* Основной контент */}
      <div className="dashboard-content">
        <div className="container">
          {/* Приветствие */}
          <div style={{ marginBottom: 'var(--space-8)' }}>
            <h1 style={{ 
              fontSize: 'var(--font-size-4xl)',
              fontWeight: 'bold',
              color: 'var(--gray-900)',
              marginBottom: 'var(--space-2)'
            }}>
              Welcome back, {authStore.user.displayName}!
            </h1>
            <p style={{ 
              fontSize: 'var(--font-size-xl)',
              color: 'var(--gray-600)'
            }}>
              Here's what's happening with your tests today.
            </p>
          </div>

          {/* Статистика */}
          <div className="stats-grid">
            {[
              {
                title: 'Total Tests',
                value: '12',
                change: '+2',
                color: 'from-blue-500 to-cyan-500',
                icon: '📊'
              },
              {
                title: 'Completed',
                value: '8',
                change: '+3',
                color: 'from-green-500 to-emerald-500',
                icon: '✅'
              },
              {
                title: 'In Progress',
                value: '3',
                change: '-1',
                color: 'from-yellow-500 to-orange-500',
                icon: '⏳'
              },
              {
                title: 'Avg Score',
                value: '87%',
                change: '+5%',
                color: 'from-purple-500 to-pink-500',
                icon: '🎯'
              }
            ].map((stat, index) => (
              <div key={index} className="stat-card">
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 'var(--space-4)' }}>
                  <div style={{
                    width: '3rem',
                    height: '3rem',
                    background: 'var(--gradient-primary)',
                    borderRadius: 'var(--radius-xl)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '1.25rem'
                  }}>
                    {stat.icon}
                  </div>
                  <span style={{ 
                    fontSize: 'var(--font-size-sm)',
                    fontWeight: '500',
                    color: stat.change.startsWith('+') ? 'var(--success)' : 'var(--error)'
                  }}>
                    {stat.change}
                  </span>
                </div>
                <h3 style={{ 
                  fontSize: 'var(--font-size-2xl)',
                  fontWeight: 'bold',
                  color: 'var(--gray-900)',
                  marginBottom: 'var(--space-1)'
                }}>
                  {stat.value}
                </h3>
                <p style={{ color: 'var(--gray-600)', fontSize: 'var(--font-size-sm)' }}>
                  {stat.title}
                </p>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
});