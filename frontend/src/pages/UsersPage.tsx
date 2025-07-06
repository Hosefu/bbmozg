import React from 'react';
import { useGetUsersQuery } from '../store/api/graphql';

export const UsersPage: React.FC = () => {
  const { data: users = [], isLoading } = useGetUsersQuery();

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="users-page">
      <h1>Пользователи</h1>
      <div className="users-list">
        {users.map(user => (
          <div key={user.id} className="user-card">
            <h3>{user.firstName} {user.lastName}</h3>
            <p>{user.position}</p>
            <p>{user.department}</p>
          </div>
        ))}
      </div>
    </div>
  );
}; 