import React from 'react';
import clsx from 'clsx';
import { Avatar } from '../Avatar/Avatar';
import './AvatarGroup.scss';

export interface AvatarGroupProps {
  avatars: Array<{
    src: string;
    alt: string;
  }>;
  size?: 'sm' | 'md' | 'lg';
  max?: number;
  className?: string;
}

export const AvatarGroup: React.FC<AvatarGroupProps> = ({
  avatars,
  size = 'md',
  max = 5,
  className,
}) => {
  const visibleAvatars = avatars.slice(0, max);
  const remainingCount = avatars.length - max;

  return (
    <div className={clsx('avatar-group', `avatar-group--${size}`, className)}>
      {visibleAvatars.map((avatar, index) => (
        <Avatar
          key={index}
          src={avatar.src}
          alt={avatar.alt}
          size={size}
          className="avatar-group__avatar"
        />
      ))}
      {remainingCount > 0 && (
        <div className={clsx('avatar-group__more', `avatar-group__more--${size}`)}>
          +{remainingCount}
        </div>
      )}
    </div>
  );
};