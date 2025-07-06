import React from 'react';
import clsx from 'clsx';
import { Card } from '../../atoms/Card/Card';
import { AvatarGroup } from '../../atoms/AvatarGroup/AvatarGroup';
import { Alert } from '../../atoms/Alert/Alert';
import { Typography } from '../../atoms/Typography/Typography';
import { CardHeader } from '../CardHeader/CardHeader';
import { CardActions, CardAction } from '../CardActions/CardActions';
import './FlowCard.scss';

export interface FlowCardProps {
  title: string;
  description: string;
  duration: string;
  stepsCount: number;
  mentors: Array<{
    name: string;
    avatar: string;
  }>;
  backgroundImage?: string;
  isEditable?: boolean;
  isAssignable?: boolean;
  isDraft?: boolean;
  onEdit?: () => void;
  onAssign?: () => void;
  className?: string;
}

export const FlowCard: React.FC<FlowCardProps> = ({
  title,
  description,
  duration,
  stepsCount,
  mentors,
  backgroundImage,
  isEditable = true,
  isAssignable = true,
  isDraft = false,
  onEdit,
  onAssign,
  className,
}) => {
  const avatars = mentors.map(mentor => ({
    src: mentor.avatar,
    alt: mentor.name,
  }));

  const actions: CardAction[] = [];
  
  if (isEditable) {
    actions.push({
      label: 'Редактировать',
      variant: 'outline',
      onClick: onEdit || (() => {}),
    });
  }
  
  if (isAssignable && !isDraft) {
    actions.push({
      label: 'Назначить',
      variant: 'primary',
      onClick: onAssign || (() => {}),
    });
  }

  return (
    <Card className={clsx('flow-card', className)} shadow="sm" rounded="lg">
      {/* Header with background image */}
      <CardHeader
        title=""
        backgroundImage={backgroundImage}
        badge={{
          text: `${duration} · ${stepsCount} этапов`,
          variant: 'dark'
        }}
        className="flow-card__header"
      />

      {/* Content */}
      <div className="flow-card__content">
        <div className="flow-card__info">
          <Typography variant="h6" color="inherit" className="flow-card__title">
            {title}
          </Typography>
          
          <Typography variant="body2" color="gray" className="flow-card__description">
            {description}
          </Typography>
          
          <div className="flow-card__mentors">
            <AvatarGroup avatars={avatars} size="md" />
            <Typography variant="body2" color="gray" className="flow-card__mentor-names">
              {mentors.map(mentor => mentor.name).join(', ')}
            </Typography>
          </div>
        </div>

        {/* Draft warning */}
        {isDraft && (
          <Alert variant="warning" className="flow-card__draft-warning">
            Поток дорабатывается, его пока нельзя назначить
          </Alert>
        )}

        {/* Actions */}
        <CardActions
          actions={actions}
          layout={isDraft ? 'vertical' : 'horizontal'}
          fullWidth={isDraft}
          className="flow-card__actions"
        />
      </div>
    </Card>
  );
};