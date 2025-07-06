import React from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate, useLocation } from 'react-router-dom';
import { selectCurrentUser, logout } from '../../store/slices/authSlice';
import { Button } from '../atoms/Button/Button';
import './Layout.scss';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const currentUser = useSelector(selectCurrentUser);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const navigation = [
    { path: '/', label: 'Дашборд', icon: '📊' },
    { path: '/flows', label: 'Потоки', icon: '📚' },
    { path: '/users', label: 'Пользователи', icon: '👥' },
    { path: '/assignments', label: 'Назначения', icon: '📋' },
  ];

  return (
    <div className="layout">
      <aside className="layout__sidebar">
        <div className="sidebar__header">
          <h2>Lauf</h2>
        </div>
        
        <nav className="sidebar__nav">
          {navigation.map(item => (
            <button
              key={item.path}
              className={`nav-item ${location.pathname === item.path ? 'nav-item--active' : ''}`}
              onClick={() => navigate(item.path)}
            >
              <span className="nav-item__icon">{item.icon}</span>
              <span className="nav-item__label">{item.label}</span>
            </button>
          ))}
        </nav>

        <div className="sidebar__footer">
          <div className="user-info">
            <div className="user-info__details">
              <h4>{currentUser?.firstName} {currentUser?.lastName}</h4>
              <p>{currentUser?.position || 'Пользователь'}</p>
            </div>
            <Button variant="ghost" size="sm" onClick={handleLogout}>
              Выход
            </Button>
          </div>
        </div>
      </aside>

      <main className="layout__main">
        <header className="layout__header">
          <div className="header__title">
            <h1>Система управления потоками</h1>
          </div>
          <div className="header__actions">
            <span>Добро пожаловать, {currentUser?.firstName}!</span>
          </div>
        </header>

        <div className="layout__content">
          {children}
        </div>
      </main>
    </div>
  );
}; 