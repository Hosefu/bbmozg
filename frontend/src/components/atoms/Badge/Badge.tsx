import React from 'react';
import clsx from 'clsx';
import './Badge.scss';

export interface BadgeProps {
  children: React.ReactNode;
  variant?: 'primary' | 'secondary' | 'dark';
  size?: 'sm' | 'md';
  className?: string;
}

export const Badge: React.FC<BadgeProps> = ({
  children,
  variant = 'dark',
  size = 'md',
  className,
}) => {
  return (
    <div className={clsx('badge', `badge--${variant}`, `badge--${size}`, className)}>
      {children}
    </div>
  );
};