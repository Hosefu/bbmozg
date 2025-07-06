import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useGetFlowsQuery, useAssignFlowMutation } from '../store/api/graphql';
import { FlowsView } from '../components/organisms/FlowsView/FlowsView';
import type { Flow } from '../components/organisms/FlowsView/FlowsView';
import toast from 'react-hot-toast';

export const FlowsPage: React.FC = () => {
  const navigate = useNavigate();
  const { data: flows = [], isLoading, error } = useGetFlowsQuery();
  const [assignFlow] = useAssignFlowMutation();

  const handleEdit = (flowId: string) => {
    navigate(`/flows/${flowId}`);
  };

  const handleAssign = async (flowId: string) => {
    try {
      // В реальном приложении здесь будет модальное окно для выбора пользователя
      // Пока просто показываем сообщение
      toast.success('Функция назначения будет доступна после выбора пользователя');
      console.log('Assign flow:', flowId);
    } catch (error) {
      toast.error('Ошибка при назначении потока');
      console.error('Error assigning flow:', error);
    }
  };

  if (isLoading) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '200px',
        fontSize: '16px',
        color: '#333740'
      }}>
        Загрузка потоков...
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ 
        display: 'flex', 
        justifyContent: 'center', 
        alignItems: 'center', 
        height: '200px',
        fontSize: '16px',
        color: '#ef4444'
      }}>
        Ошибка загрузки потоков
      </div>
    );
  }

  // Преобразуем данные из GraphQL в формат для компонента
  const flowsForView: Flow[] = flows.map(flow => ({
    id: flow.id,
    title: flow.name,
    description: flow.description || 'Описание потока',
    duration: `${flow.settings?.daysPerStep || 1} дней`,
    stepsCount: flow.totalSteps,
    mentors: [
      // Временные данные для наставников - в реальном приложении это будет из API
      {
        name: 'Наталья Банникова',
        avatar: 'http://localhost:3845/assets/6a3a21e1c19fa2301ff2130218c2d8e210ad89fa.png',
      },
      {
        name: 'Константин Константинопольский', 
        avatar: 'http://localhost:3845/assets/9497ddf4bf8ecdd14bf14a1baba1122d119eb37d.png',
      },
    ],
    backgroundImage: 'http://localhost:3845/assets/040213130098ce678363e5c8b1ea0a488730ece3.png',
    isEditable: true,
    isAssignable: flow.status === 'PUBLISHED',
    isDraft: flow.status === 'DRAFT',
  }));

  return (
    <FlowsView
      flows={flowsForView}
      onEdit={handleEdit}
      onAssign={handleAssign}
    />
  );
}; 