import React from 'react';
import clsx from 'clsx';
import './Card.scss';

export interface CardProps {
  children: React.ReactNode;
  variant?: 'default' | 'elevated' | 'outlined';
  padding?: 'none' | 'sm' | 'md' | 'lg';
  rounded?: 'none' | 'sm' | 'md' | 'lg';
  shadow?: 'none' | 'sm' | 'md' | 'lg';
  className?: string;
  onClick?: () => void;
}

export const Card: React.FC<CardProps> = ({
  children,
  variant = 'default',
  padding = 'md',
  rounded = 'md',
  shadow = 'sm',
  className,
  onClick,
}) => {
  return (
    <div
      className={clsx(
        'card',
        `card--${variant}`,
        `card--padding-${padding}`,
        `card--rounded-${rounded}`,
        `card--shadow-${shadow}`,
        {
          'card--clickable': !!onClick,
        },
        className
      )}
      onClick={onClick}
    >
      {children}
    </div>
  );
}; 