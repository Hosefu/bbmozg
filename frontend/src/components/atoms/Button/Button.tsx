import React from 'react';
import clsx from 'clsx';
import './Button.scss';

export interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  loading?: boolean;
  fullWidth?: boolean;
  children: React.ReactNode;
  onClick?: () => void;
  type?: 'button' | 'submit' | 'reset';
  className?: string;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  disabled = false,
  loading = false,
  fullWidth = false,
  children,
  onClick,
  type = 'button',
  className,
}) => {
  const handleClick = (e: React.MouseEvent<HTMLButtonElement>) => {
    if (!disabled && !loading && onClick) {
      onClick();
    }
  };

  return (
    <button
      type={type}
      className={clsx(
        'btn',
        `btn--${variant}`,
        `btn--${size}`,
        {
          'btn--disabled': disabled,
          'btn--loading': loading,
          'btn--full-width': fullWidth,
        },
        className
      )}
      disabled={disabled || loading}
      onClick={onClick ? handleClick : undefined}
    >
      {loading && (
        <span className="btn__spinner">
          <svg className="btn__spinner-icon" viewBox="0 0 24 24">
            <circle
              className="btn__spinner-path"
              cx="12"
              cy="12"
              r="10"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            />
          </svg>
        </span>
      )}
      <span className={clsx('btn__content', { 'btn__content--hidden': loading })}>
        {children}
      </span>
    </button>
  );
}; 