import React from 'react';
import clsx from 'clsx';
import './Avatar.scss';

export interface AvatarProps {
  src: string;
  alt: string;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

export const Avatar: React.FC<AvatarProps> = ({
  src,
  alt,
  size = 'md',
  className,
}) => {
  return (
    <div className={clsx('avatar', `avatar--${size}`, className)}>
      <img 
        src={src} 
        alt={alt}
        className="avatar__image"
      />
    </div>
  );
};