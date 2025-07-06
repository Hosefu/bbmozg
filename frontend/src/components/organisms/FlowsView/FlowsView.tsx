import React from 'react';
import clsx from 'clsx';
import { FlowCard } from '../../molecules/FlowCard/FlowCard';
import './FlowsView.scss';

export interface Flow {
  id: string;
  title: string;
  description: string;
  duration: string;
  stepsCount: number;
  mentors: Array<{
    name: string;
    avatar: string;
  }>;
  backgroundImage?: string;
  isEditable?: boolean;
  isAssignable?: boolean;
  isDraft?: boolean;
}

export interface FlowsViewProps {
  flows: Flow[];
  onEdit?: (flowId: string) => void;
  onAssign?: (flowId: string) => void;
  className?: string;
}

export const FlowsView: React.FC<FlowsViewProps> = ({
  flows,
  onEdit,
  onAssign,
  className,
}) => {
  return (
    <div className={clsx('flows-view', className)}>
      <div className="flows-view__header">
        <h1 className="flows-view__title">Потоки</h1>
      </div>
      
      <div className="flows-view__grid">
        {flows.map((flow) => (
          <FlowCard
            key={flow.id}
            title={flow.title}
            description={flow.description}
            duration={flow.duration}
            stepsCount={flow.stepsCount}
            mentors={flow.mentors}
            backgroundImage={flow.backgroundImage}
            isEditable={flow.isEditable}
            isAssignable={flow.isAssignable}
            isDraft={flow.isDraft}
            onEdit={() => onEdit?.(flow.id)}
            onAssign={() => onAssign?.(flow.id)}
          />
        ))}
      </div>
    </div>
  );
};