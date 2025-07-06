import React from 'react';
import clsx from 'clsx';
import { Button } from '../../atoms/Button/Button';
import './CardActions.scss';

export interface CardAction {
  label: string;
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  onClick: () => void;
  disabled?: boolean;
  loading?: boolean;
}

export interface CardActionsProps {
  actions: CardAction[];
  layout?: 'horizontal' | 'vertical';
  size?: 'sm' | 'md' | 'lg';
  fullWidth?: boolean;
  className?: string;
}

export const CardActions: React.FC<CardActionsProps> = ({
  actions,
  layout = 'horizontal',
  size = 'md',
  fullWidth = false,
  className,
}) => {
  if (actions.length === 0) return null;

  return (
    <div
      className={clsx(
        'card-actions',
        `card-actions--${layout}`,
        `card-actions--${size}`,
        {
          'card-actions--full-width': fullWidth,
        },
        className
      )}
    >
      {actions.map((action, index) => (
        <Button
          key={index}
          variant={action.variant || 'primary'}
          size={size}
          onClick={action.onClick}
          disabled={action.disabled}
          loading={action.loading}
          fullWidth={fullWidth || layout === 'vertical'}
          className="card-actions__button"
        >
          {action.label}
        </Button>
      ))}
    </div>
  );
}; 