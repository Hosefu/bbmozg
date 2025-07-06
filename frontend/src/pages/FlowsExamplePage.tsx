import React from 'react';
import { FlowsView } from '../components/organisms/FlowsView/FlowsView';

const mockFlows = [
  {
    id: '1',
    title: 'ТИМЛИД',
    description: 'Внутри потока все знания для старта в роли тимлида. Поток помогает закреплять знания из зум-занятий',
    duration: '10 дней',
    stepsCount: 5,
    mentors: [
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
    isAssignable: true,
    isDraft: false,
  },
  {
    id: '2',
    title: 'ТИМЛИД',
    description: 'Внутри потока все знания для старта в роли тимлида. Поток помогает закреплять знания из зум-занятий',
    duration: '10 дней',
    stepsCount: 5,
    mentors: [
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
    isAssignable: false,
    isDraft: true,
  },
];

export const FlowsExamplePage: React.FC = () => {
  const handleEdit = (flowId: string) => {
    console.log('Edit flow:', flowId);
  };

  const handleAssign = (flowId: string) => {
    console.log('Assign flow:', flowId);
  };

  return (
    <FlowsView
      flows={mockFlows}
      onEdit={handleEdit}
      onAssign={handleAssign}
    />
  );
};