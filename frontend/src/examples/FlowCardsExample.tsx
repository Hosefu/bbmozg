import React from 'react';
import { FlowCard } from '../components/molecules/FlowCard';
import { FlowAssignedCard } from '../components/molecules/FlowAssignedCard';
import './FlowCardsExample.scss';

export const FlowCardsExample: React.FC = () => {
  return (
    <div className="flow-cards-example">
      <h2>Примеры карточек потоков</h2>
      
      <div className="flow-cards-example__section">
        <h3>Обычные потоки (для админа/бадди)</h3>
        <div className="flow-cards-example__grid">
          {/* Обычный поток */}
          <FlowCard
            title="ТИМЛИД"
            description="Внутри потока все знания для старта в роли тимлида. Поток поможет закреплять знания из зум-занятий"
            duration="10 дней"
            stepsCount={5}
            mentors={[
              { name: "Наталья Банникова", avatar: "/avatars/natalia.jpg" },
              { name: "Константин Константинопольский", avatar: "/avatars/konstantin.jpg" }
            ]}
            backgroundImage="/images/flow-bg-1.jpg"
            onEdit={() => console.log('Edit flow')}
            onAssign={() => console.log('Assign flow')}
          />

          {/* Поток в разработке */}
          <FlowCard
            title="ИИ ВОРКШОП"
            description="Как стать ИИ-master-slaves? Андрей Сергунин ответит на все ваши вопросы"
            duration="12 дней"
            stepsCount={3}
            mentors={[
              { name: "Андрей Сергунин", avatar: "/avatars/andrey.jpg" }
            ]}
            backgroundImage="/images/flow-bg-2.jpg"
            isDraft={true}
            onEdit={() => console.log('Edit draft flow')}
          />
        </div>
      </div>

      <div className="flow-cards-example__section">
        <h3>Назначенные потоки (для пользователя)</h3>
        <div className="flow-cards-example__grid">
          {/* В процессе */}
          <FlowAssignedCard
            title="ТИМЛИД"
            status="in-progress"
            backgroundImage="/images/flow-bg-1.jpg"
            currentStep={{
              number: 3,
              title: "Этап будет доступен завтра",
              description: "Удержание стратегически важного клиента и работа на перспективу"
            }}
            progress={45}
            buddy={{
              name: "Твой бадди",
              avatar: "/avatars/buddy.jpg"
            }}
            onAction={() => console.log('Continue flow')}
          />

          {/* Не начат */}
          <FlowAssignedCard
            title="ТИМЛИД"
            status="not-started"
            backgroundImage="/images/flow-bg-1.jpg"
            currentStep={{
              number: 4,
              title: "Пройди этап до завтра",
              description: "Удержание стратегически важного клиента и работа на перспективу"
            }}
            buddy={{
              name: "Твой бадди",
              avatar: "/avatars/buddy.jpg"
            }}
            onAction={() => console.log('Start flow')}
          />

          {/* Завершен */}
          <FlowAssignedCard
            title="ТИМЛИД"
            status="completed"
            backgroundImage="/images/flow-bg-1.jpg"
            progress={100}
            buddy={{
              name: "Твой радостный бадди",
              avatar: "/avatars/buddy.jpg"
            }}
            onAction={() => console.log('Review flow')}
            actionLabel="Продолжить"
          />
        </div>
      </div>
    </div>
  );
}; 