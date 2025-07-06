import React from 'react';
import clsx from 'clsx';
import { Icon } from '../Icon/Icon';
import './Alert.scss';

export interface AlertProps {
  children: React.ReactNode;
  variant?: 'info' | 'success' | 'warning' | 'error';
  size?: 'sm' | 'md' | 'lg';
  icon?: boolean;
  onClose?: () => void;
  className?: string;
}

const alertIcons = {
  info: 'info',
  success: 'check',
  warning: 'warning',
  error: 'close'
};

const alertIconColors = {
  info: 'primary',
  success: 'success',
  warning: 'warning',
  error: 'error'
} as const;

export const Alert: React.FC<AlertProps> = ({
  children,
  variant = 'info',
  size = 'md',
  icon = true,
  onClose,
  className,
}) => {
  return (
    <div
      className={clsx(
        'alert',
        `alert--${variant}`,
        `alert--${size}`,
        className
      )}
    >
      {icon && (
        <div className="alert__icon">
          <Icon 
            name={alertIcons[variant]} 
            size={size === 'sm' ? 'sm' : 'md'}
            color={alertIconColors[variant]}
          />
        </div>
      )}
      <div className="alert__content">
        {children}
      </div>
      {onClose && (
        <button 
          className="alert__close"
          onClick={onClose}
          aria-label="Закрыть"
        >
          <Icon name="close" size="sm" />
        </button>
      )}
    </div>
  );
}; 