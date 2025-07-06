import React, { useState } from 'react';
import { useDispatch } from 'react-redux';
import { setDevAuth } from '../../store/slices/authSlice';
import { Button } from '../atoms/Button/Button';
import { v4 as uuidv4 } from 'uuid';
import './DevAuth.scss';

export const DevAuth: React.FC = () => {
  const dispatch = useDispatch();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    username: '',
    position: '',
    department: '',
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log('Form submitted', formData);
    
    if (!formData.firstName || !formData.lastName) {
      alert('Имя и фамилия обязательны');
      return;
    }

    const devAuth = {
      userId: uuidv4(),
      firstName: formData.firstName,
      lastName: formData.lastName,
      username: formData.username || undefined,
      position: formData.position || undefined,
      department: formData.department || undefined,
      roles: ['User'], // Базовая роль
    };

    console.log('Dispatching devAuth:', devAuth);
    dispatch(setDevAuth(devAuth));
  };

  const handleQuickLogin = (preset: any) => {
    console.log('Quick login clicked:', preset);
    const devAuth = {
      userId: uuidv4(),
      ...preset,
      roles: preset.roles || ['User'],
    };
    
    console.log('Dispatching quick devAuth:', devAuth);
    dispatch(setDevAuth(devAuth));
  };

  const presets = [
    {
      firstName: 'Админ',
      lastName: 'Системы',
      username: 'admin',
      position: 'Системный администратор',
      department: 'IT',
      roles: ['Admin', 'User'],
    },
    {
      firstName: 'Менеджер',
      lastName: 'Проектов',
      username: 'manager',
      position: 'Менеджер проектов',
      department: 'Управление',
      roles: ['Manager', 'User'],
    },
    {
      firstName: 'Обычный',
      lastName: 'Пользователь',
      username: 'user',
      position: 'Специалист',
      department: 'Общий',
      roles: ['User'],
    },
  ];

  return (
    <div className="dev-auth">
      <div className="dev-auth__container">
        <div className="dev-auth__header">
          <h1>Development Authentication</h1>
          <p>Введите данные для входа в систему (режим разработки)</p>
        </div>

        <form onSubmit={handleSubmit} className="dev-auth__form">
          <div className="dev-auth__field">
            <label htmlFor="firstName">Имя *</label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleInputChange}
              required
              placeholder="Введите имя"
            />
          </div>

          <div className="dev-auth__field">
            <label htmlFor="lastName">Фамилия *</label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              value={formData.lastName}
              onChange={handleInputChange}
              required
              placeholder="Введите фамилию"
            />
          </div>

          <div className="dev-auth__field">
            <label htmlFor="username">Имя пользователя</label>
            <input
              type="text"
              id="username"
              name="username"
              value={formData.username}
              onChange={handleInputChange}
              placeholder="Введите имя пользователя"
            />
          </div>

          <div className="dev-auth__field">
            <label htmlFor="position">Должность</label>
            <input
              type="text"
              id="position"
              name="position"
              value={formData.position}
              onChange={handleInputChange}
              placeholder="Введите должность"
            />
          </div>

          <div className="dev-auth__field">
            <label htmlFor="department">Отдел</label>
            <input
              type="text"
              id="department"
              name="department"
              value={formData.department}
              onChange={handleInputChange}
              placeholder="Введите отдел"
            />
          </div>

          <Button type="submit" size="lg" fullWidth>
            Войти в систему
          </Button>
        </form>

        <div className="dev-auth__divider">
          <span>или</span>
        </div>

        <div className="dev-auth__presets">
          <h3>Быстрый вход</h3>
          <div className="dev-auth__presets-grid">
            {presets.map((preset, index) => (
              <div key={index} className="dev-auth__preset">
                <div className="dev-auth__preset-info">
                  <h4>{preset.firstName} {preset.lastName}</h4>
                  <p>{preset.position}</p>
                  <p>{preset.department}</p>
                  <div className="dev-auth__preset-roles">
                    {preset.roles.map(role => (
                      <span key={role} className="dev-auth__role-badge">
                        {role}
                      </span>
                    ))}
                  </div>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => handleQuickLogin(preset)}
                >
                  Войти
                </Button>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}; 