import React from 'react';
import clsx from 'clsx';
import './Status.scss';

export interface StatusProps {
  variant: 'in-progress' | 'not-started' | 'completed' | 'overdue' | 'paused';
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

const statusLabels = {
  'in-progress': 'В процессе',
  'not-started': 'Не начат',
  'completed': 'Завершен',
  'overdue': 'Просрочен',
  'paused': 'Приостановлен'
};

export const Status: React.FC<StatusProps> = ({
  variant,
  size = 'md',
  className,
}) => {
  return (
    <div
      className={clsx(
        'status',
        `status--${variant}`,
        `status--${size}`,
        className
      )}
    >
      <div className="status__indicator" />
      <span className="status__label">{statusLabels[variant]}</span>
    </div>
  );
}; 