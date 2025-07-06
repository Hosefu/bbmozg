import React from 'react';
import { useGetFlowAssignmentsQuery } from '../store/api/graphql';

export const AssignmentsPage: React.FC = () => {
  const { data: assignments = [], isLoading } = useGetFlowAssignmentsQuery({});

  if (isLoading) return <div>Загрузка...</div>;

  return (
    <div className="assignments-page">
      <h1>Назначения</h1>
      <div className="assignments-list">
        {assignments.map(assignment => (
          <div key={assignment.id} className="assignment-card">
            <h3>{assignment.flow.name}</h3>
            <p>Пользователь: {assignment.user.firstName} {assignment.user.lastName}</p>
            <p>Статус: {assignment.status}</p>
            <p>Назначено: {new Date(assignment.assignedAt).toLocaleDateString()}</p>
          </div>
        ))}
      </div>
    </div>
  );
}; 