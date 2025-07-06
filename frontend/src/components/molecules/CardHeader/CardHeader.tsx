import React from 'react';
import clsx from 'clsx';
import { Badge } from '../../atoms/Badge/Badge';
import { Typography } from '../../atoms/Typography/Typography';
import './CardHeader.scss';

export interface CardHeaderProps {
  title: string;
  subtitle?: string;
  backgroundImage?: string;
  badge?: {
    text: string;
    variant?: 'primary' | 'secondary' | 'dark';
  };
  overlay?: boolean;
  height?: 'sm' | 'md' | 'lg';
  className?: string;
}

export const CardHeader: React.FC<CardHeaderProps> = ({
  title,
  subtitle,
  backgroundImage,
  badge,
  overlay = false,
  height = 'md',
  className,
}) => {
  return (
    <div
      className={clsx(
        'card-header',
        `card-header--${height}`,
        {
          'card-header--with-overlay': overlay,
          'card-header--with-image': !!backgroundImage,
        },
        className
      )}
    >
      {backgroundImage && (
        <div 
          className="card-header__background"
          style={{ backgroundImage: `url(${backgroundImage})` }}
        />
      )}
      
      {badge && (
        <Badge 
          variant={badge.variant || 'dark'} 
          className="card-header__badge"
        >
          {badge.text}
        </Badge>
      )}
      
      <div className="card-header__content">
        <Typography 
          variant="h3" 
          color={backgroundImage ? 'white' : 'inherit'}
          className="card-header__title"
        >
          {title}
        </Typography>
        
        {subtitle && (
          <Typography 
            variant="body2" 
            color={backgroundImage ? 'white' : 'gray'}
            className="card-header__subtitle"
          >
            {subtitle}
          </Typography>
        )}
      </div>
    </div>
  );
}; 