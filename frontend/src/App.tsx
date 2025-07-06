import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Provider } from 'react-redux';
import { store } from './store';
import { useSelector } from 'react-redux';
import { selectIsAuthenticated } from './store/slices/authSlice';
import { DevAuth } from './components/auth/DevAuth';
import { DashboardPage } from './pages/DashboardPage';
import { FlowsPage } from './pages/FlowsPage';
import { FlowDetailPage } from './pages/FlowDetailPage';
import { FlowsExamplePage } from './pages/FlowsExamplePage';
import { UsersPage } from './pages/UsersPage';
import { UserDetailPage } from './pages/UserDetailPage';
import { AssignmentsPage } from './pages/AssignmentsPage';
import { Layout } from './components/layout/Layout';
import { Toaster } from 'react-hot-toast';
import './styles/globals.scss';

// Protected Route wrapper
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const isAuthenticated = useSelector(selectIsAuthenticated);
  
  if (!isAuthenticated) {
    return <DevAuth />;
  }
  
  return <Layout>{children}</Layout>;
};

// Main App Router
const AppRouter: React.FC = () => {
  return (
    <BrowserRouter>
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<DevAuth />} />
        
        {/* Protected routes */}
        <Route path="/" element={
          <ProtectedRoute>
            <DashboardPage />
          </ProtectedRoute>
        } />
        
        <Route path="/flows" element={
          <ProtectedRoute>
            <FlowsPage />
          </ProtectedRoute>
        } />
        
        <Route path="/flows-example" element={
          <ProtectedRoute>
            <FlowsExamplePage />
          </ProtectedRoute>
        } />
        
        <Route path="/flows/:id" element={
          <ProtectedRoute>
            <FlowDetailPage />
          </ProtectedRoute>
        } />
        
        <Route path="/users" element={
          <ProtectedRoute>
            <UsersPage />
          </ProtectedRoute>
        } />
        
        <Route path="/users/:id" element={
          <ProtectedRoute>
            <UserDetailPage />
          </ProtectedRoute>
        } />
        
        <Route path="/assignments" element={
          <ProtectedRoute>
            <AssignmentsPage />
          </ProtectedRoute>
        } />
        
        {/* Fallback route */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
};

// Main App Component
const App: React.FC = () => {
  return (
    <Provider store={store}>
      <div className="app">
        <AppRouter />
        <Toaster
          position="top-right"
          toastOptions={{
            duration: 4000,
            style: {
              background: '#fff',
              color: '#333',
              boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
              borderRadius: '8px',
              border: '1px solid #e5e7eb',
            },
            success: {
              iconTheme: {
                primary: '#10b981',
                secondary: '#fff',
              },
            },
            error: {
              iconTheme: {
                primary: '#ef4444',
                secondary: '#fff',
              },
            },
          }}
        />
      </div>
    </Provider>
  );
};

export default App; 