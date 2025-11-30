import React from 'react';
import { Link } from 'react-router-dom';

export const Landing: React.FC = () => {
  return (
    <div className="min-h-screen" style={{ background: 'var(--gradient-bg)' }}>
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
              <Link
                to="/auth"
                style={{
                  color: 'var(--gray-600)',
                  textDecoration: 'none',
                  fontWeight: '500',
                  transition: 'var(--transition)'
                }}
                onMouseEnter={(e) => e.currentTarget.style.color = 'var(--primary)'}
                onMouseLeave={(e) => e.currentTarget.style.color = 'var(--gray-600)'}
              >
                Sign In
              </Link>
              <Link
                to="/auth"
                className="btn btn-primary"
                style={{ textDecoration: 'none' }}
              >
                Get Started
              </Link>
            </div>
          </div>
        </div>
      </nav>

      {/* Герой-секция */}
      <section className="landing-hero">
        <div className="container">
          <div style={{
            display: 'inline-flex',
            alignItems: 'center',
            gap: 'var(--space-2)',
            background: 'var(--gray-100)',
            borderRadius: '9999px',
            padding: 'var(--space-2) var(--space-4)',
            marginBottom: 'var(--space-8)'
          }}>
            <span style={{
              width: '0.5rem',
              height: '0.5rem',
              background: 'var(--primary)',
              borderRadius: '50%'
            }}></span>
            <span style={{ 
              fontSize: 'var(--font-size-sm)',
              fontWeight: '500',
              color: 'var(--primary)'
            }}>
              Next Generation Testing Platform
            </span>
          </div>
          
          <h1 className="hero-title">
            Assess Skills, 
            <span style={{ display: 'block' }}>Drive Success</span>
          </h1>
          
          <p className="hero-subtitle">
            BeaverTests empowers organizations with comprehensive assessment solutions. 
            Create, manage, and analyze tests with our cutting-edge platform designed 
            for modern educational and corporate needs.
          </p>
          
          <div style={{ 
            display: 'flex', 
            flexDirection: 'column',
            alignItems: 'center',
            gap: 'var(--space-4)',
            marginTop: 'var(--space-8)'
          }}>
            <div style={{ 
              display: 'flex', 
              flexDirection: 'row',
              gap: 'var(--space-4)',
              flexWrap: 'wrap',
              justifyContent: 'center'
            }}>
              <Link
                to="/auth"
                className="btn btn-primary"
                style={{ 
                  padding: 'var(--space-4) var(--space-8)',
                  fontSize: 'var(--font-size-lg)',
                  textDecoration: 'none'
                }}
              >
                Start Testing Today
              </Link>
              <button className="btn btn-secondary" style={{ padding: 'var(--space-4) var(--space-8)', fontSize: 'var(--font-size-lg)' }}>
                <span style={{ display: 'flex', alignItems: 'center', gap: 'var(--space-2)' }}>
                  <span>Watch Demo</span>
                  <span style={{ transition: 'var(--transition)' }}>→</span>
                </span>
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* Секции с фичами */}
      <section style={{ padding: 'var(--space-16) 0' }}>
        <div className="container">
          <div style={{ textAlign: 'center', marginBottom: 'var(--space-16)' }}>
            <h2 style={{ 
              fontSize: 'var(--font-size-4xl)',
              fontWeight: 'bold',
              color: 'var(--gray-900)',
              marginBottom: 'var(--space-4)'
            }}>
              Why Choose BeaverTests?
            </h2>
            <p style={{ 
              fontSize: 'var(--font-size-xl)',
              color: 'var(--gray-600)',
              maxWidth: '600px',
              margin: '0 auto'
            }}>
              Our platform offers everything you need to create effective assessments 
              and make data-driven decisions.
            </p>
          </div>

          <div className="features-grid">
            {[
              {
                icon: '📊',
                title: 'Advanced Analytics',
                description: 'Gain deep insights with comprehensive reporting and real-time analytics.',
                color: 'from-blue-500 to-cyan-500'
              },
              {
                icon: '⚡',
                title: 'Lightning Fast',
                description: 'Built for performance with instant results and seamless user experience.',
                color: 'from-green-500 to-emerald-500'
              },
              {
                icon: '🔒',
                title: 'Secure & Reliable',
                description: 'Enterprise-grade security with 99.9% uptime guarantee.',
                color: 'from-purple-500 to-pink-500'
              }
            ].map((feature, index) => (
              <div key={index} className="feature-card">
                <div style={{
                  width: '3.5rem',
                  height: '3.5rem',
                  background: 'var(--gradient-primary)',
                  borderRadius: 'var(--radius-xl)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  fontSize: '1.5rem',
                  margin: '0 auto var(--space-6) auto',
                  transition: 'var(--transition)'
                }}>
                  {feature.icon}
                </div>
                <h3 style={{ 
                  fontSize: 'var(--font-size-xl)',
                  fontWeight: '600',
                  color: 'var(--gray-900)',
                  marginBottom: 'var(--space-3)'
                }}>
                  {feature.title}
                </h3>
                <p style={{ 
                  color: 'var(--gray-600)',
                  lineHeight: '1.6'
                }}>
                  {feature.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
};