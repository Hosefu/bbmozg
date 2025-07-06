import React from 'react';
import clsx from 'clsx';
import './Typography.scss';

export interface TypographyProps {
  children: React.ReactNode;
  variant?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'body1' | 'body2' | 'caption' | 'overline';
  component?: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'h6' | 'p' | 'span' | 'div';
  color?: 'primary' | 'secondary' | 'success' | 'warning' | 'error' | 'white' | 'gray' | 'inherit';
  align?: 'left' | 'center' | 'right' | 'justify';
  weight?: 'normal' | 'medium' | 'semibold' | 'bold';
  truncate?: boolean;
  className?: string;
}

const defaultComponents = {
  h1: 'h1',
  h2: 'h2',
  h3: 'h3',
  h4: 'h4',
  h5: 'h5',
  h6: 'h6',
  body1: 'p',
  body2: 'p',
  caption: 'span',
  overline: 'span'
} as const;

export const Typography: React.FC<TypographyProps> = ({
  children,
  variant = 'body1',
  component,
  color = 'inherit',
  align = 'left',
  weight = 'normal',
  truncate = false,
  className,
}) => {
  const Component = component || defaultComponents[variant];

  return (
    <Component
      className={clsx(
        'typography',
        `typography--${variant}`,
        `typography--color-${color}`,
        `typography--align-${align}`,
        `typography--weight-${weight}`,
        {
          'typography--truncate': truncate,
        },
        className
      )}
    >
      {children}
    </Component>
  );
}; 