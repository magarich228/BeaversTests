import React, { useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { observer } from 'mobx-react-lite';
import { StoreContext, rootStore } from './shared/stores';
import { Landing } from './shared/pages/Landing';
import { AuthPage } from './auth/AuthPage';
import { ProjectPage } from './shared/pages/ProjectPage';

// Компонент для защищенных маршрутов
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = observer(({ children }) => {
  const { authStore } = rootStore;

  if (authStore.isLoading && !authStore.user) {
    // TODO: нормальный Loader
    return (
      <div style={{ minHeight: '100vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <div>Loading</div>
      </div>
    );
  }

  if (!authStore.isAuthenticated) {
    return <Navigate to="/auth" replace />;
  }

  return <>{children}</>;
});

// Компонент для публичных маршрутов
const PublicRoute: React.FC<{ children: React.ReactNode }> = observer(({ children }) => {
  const { authStore } = rootStore;

  if (authStore.isAuthenticated) {
    return <Navigate to="/test" replace />;
  }

  return <>{children}</>;
});

const App: React.FC = observer(() => {
  const { authStore } = rootStore;

  useEffect(() => {
    authStore.initializeAuth();
  }, [authStore]);

  return (
    <StoreContext.Provider value={rootStore}>
      <Router>
        <div className="App">
          <Routes>
            <Route 
              path="/" 
              element={
                <PublicRoute>
                  <Landing />
                </PublicRoute>
              } 
            />
            <Route 
              path="/auth" 
              element={
                <PublicRoute>
                  <AuthPage />
                </PublicRoute>
              } 
            />
            <Route 
              path="/test" 
              element={
                <ProtectedRoute>
                  <ProjectPage />
                </ProtectedRoute>
              } 
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </div>
      </Router>
    </StoreContext.Provider>
  );
});

export default App;