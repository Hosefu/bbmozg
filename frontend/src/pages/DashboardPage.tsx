import React from 'react';
import { useSelector } from 'react-redux';
import { selectCurrentUser } from '../store/slices/authSlice';
import { useGetFlowsQuery, useGetOverdueAssignmentsQuery } from '../store/api/graphql';
import { Button } from '../components/atoms/Button/Button';
import './DashboardPage.scss';

export const DashboardPage: React.FC = () => {
  const currentUser = useSelector(selectCurrentUser);
  const { data: flows = [], isLoading: flowsLoading } = useGetFlowsQuery({});
  const { data: overdueAssignments = [], isLoading: overdueLoading } = useGetOverdueAssignmentsQuery();

  const stats = {
    totalFlows: flows.length,
    publishedFlows: flows.filter(f => f.status === 'Published').length,
    overdueAssignments: overdueAssignments.length,
    activeUsers: 0, // Будет заполнено позже
  };

  if (flowsLoading || overdueLoading) {
    return (
      <div className="dashboard">
        <div className="dashboard__loading">
          <div className="loading-spinner">Загрузка...</div>
        </div>
      </div>
    );
  }

  return (
    <div className="dashboard">
      <div className="dashboard__header">
        <div>
          <h1>Добро пожаловать, {currentUser?.firstName}!</h1>
          <p>Обзор системы управления потоками</p>
        </div>
        <div className="dashboard__actions">
          <Button variant="primary">
            Создать поток
          </Button>
        </div>
      </div>

      <div className="dashboard__stats">
        <div className="stat-card">
          <div className="stat-card__icon">📚</div>
          <div className="stat-card__content">
            <h3>{stats.totalFlows}</h3>
            <p>Всего потоков</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-card__icon">✅</div>
          <div className="stat-card__content">
            <h3>{stats.publishedFlows}</h3>
            <p>Опубликованных</p>
          </div>
        </div>

        <div className="stat-card stat-card--warning">
          <div className="stat-card__icon">⚠️</div>
          <div className="stat-card__content">
            <h3>{stats.overdueAssignments}</h3>
            <p>Просроченных</p>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-card__icon">👥</div>
          <div className="stat-card__content">
            <h3>{stats.activeUsers}</h3>
            <p>Активных пользователей</p>
          </div>
        </div>
      </div>

      <div className="dashboard__content">
        <div className="dashboard__section">
          <h2>Последние потоки</h2>
          <div className="flow-list">
            {flows.slice(0, 5).map(flow => (
              <div key={flow.id} className="flow-item">
                <div className="flow-item__content">
                  <h3>{flow.name}</h3>
                  <p>{flow.description}</p>
                  <div className="flow-item__meta">
                    <span className={`status status--${flow.status.toLowerCase()}`}>
                      {flow.status}
                    </span>
                    <span className="flow-item__steps">
                      {flow.totalSteps} шагов
                    </span>
                  </div>
                </div>
                <div className="flow-item__actions">
                  <Button variant="outline" size="sm">
                    Просмотр
                  </Button>
                </div>
              </div>
            ))}
          </div>
          <Button variant="ghost" className="dashboard__see-all">
            Показать все потоки →
          </Button>
        </div>

        {overdueAssignments.length > 0 && (
          <div className="dashboard__section">
            <h2>Просроченные назначения</h2>
            <div className="assignment-list">
              {overdueAssignments.slice(0, 3).map(assignment => (
                <div key={assignment.id} className="assignment-item assignment-item--overdue">
                  <div className="assignment-item__content">
                    <h4>{assignment.flow.name}</h4>
                    <p>Пользователь: {assignment.user.firstName} {assignment.user.lastName}</p>
                    <p>Просрочено: {new Date(assignment.deadline!).toLocaleDateString()}</p>
                  </div>
                  <div className="assignment-item__actions">
                    <Button variant="danger" size="sm">
                      Уведомить
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}; 