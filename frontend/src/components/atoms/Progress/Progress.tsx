import React from 'react';
import clsx from 'clsx';
import './Progress.scss';

export interface ProgressProps {
  value: number; // 0-100
  size?: 'sm' | 'md' | 'lg';
  variant?: 'primary' | 'success' | 'warning' | 'error';
  showLabel?: boolean;
  className?: string;
}

export const Progress: React.FC<ProgressProps> = ({
  value,
  size = 'md',
  variant = 'primary',
  showLabel = false,
  className,
}) => {
  const clampedValue = Math.min(Math.max(value, 0), 100);

  return (
    <div className={clsx('progress', `progress--${size}`, className)}>
      <div className="progress__track">
        <div
          className={clsx('progress__bar', `progress__bar--${variant}`)}
          style={{ width: `${clampedValue}%` }}
        />
      </div>
      {showLabel && (
        <span className="progress__label">
          {Math.round(clampedValue)}%
        </span>
      )}
    </div>
  );
}; 