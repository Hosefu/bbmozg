import React from 'react';
import { useParams } from 'react-router-dom';
import { useGetUserQuery } from '../store/api/graphql';

export const UserDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { data: user, isLoading } = useGetUserQuery(id!);

  if (isLoading) return <div>Загрузка...</div>;
  if (!user) return <div>Пользователь не найден</div>;

  return (
    <div className="user-detail">
      <h1>{user.firstName} {user.lastName}</h1>
      <p>Должность: {user.position}</p>
      <p>Отдел: {user.department}</p>
      <p>Активен: {user.isActive ? 'Да' : 'Нет'}</p>
    </div>
  );
}; 