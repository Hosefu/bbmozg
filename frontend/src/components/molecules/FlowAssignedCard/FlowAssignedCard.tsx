import React from 'react';
import clsx from 'clsx';
import { Card } from '../../atoms/Card/Card';
import { Status } from '../../atoms/Status/Status';
import { Progress } from '../../atoms/Progress/Progress';
import { Typography } from '../../atoms/Typography/Typography';
import { Badge } from '../../atoms/Badge/Badge';
import { Avatar } from '../../atoms/Avatar/Avatar';
import { Button } from '../../atoms/Button/Button';
import { CardHeader } from '../CardHeader/CardHeader';
import './FlowAssignedCard.scss';

export interface FlowAssignedCardProps {
  title: string;
  status: 'in-progress' | 'not-started' | 'completed' | 'overdue' | 'paused';
  backgroundImage?: string;
  currentStep?: {
    number: number;
    title: string;
    description: string;
  };
  progress?: number; // 0-100
  buddy?: {
    name: string;
    avatar: string;
  };
  deadline?: string;
  onAction?: () => void;
  actionLabel?: string;
  className?: string;
}

export const FlowAssignedCard: React.FC<FlowAssignedCardProps> = ({
  title,
  status,
  backgroundImage,
  currentStep,
  progress = 0,
  buddy,
  deadline,
  onAction,
  actionLabel,
  className,
}) => {
  const getActionLabel = () => {
    if (actionLabel) return actionLabel;
    
    switch (status) {
      case 'not-started':
        return 'Начать';
      case 'in-progress':
        return 'Продолжить';
      case 'completed':
        return 'Повторить';
      case 'paused':
        return 'Возобновить';
      case 'overdue':
        return 'Продолжить';
      default:
        return 'Открыть';
    }
  };

  const getProgressVariant = () => {
    switch (status) {
      case 'completed':
        return 'success';
      case 'overdue':
        return 'error';
      case 'paused':
        return 'warning';
      default:
        return 'primary';
    }
  };

  return (
    <Card className={clsx('flow-assigned-card', className)} shadow="sm" rounded="lg">
      {/* Header with background image */}
      <CardHeader
        title=""
        backgroundImage={backgroundImage}
        badge={{
          text: status === 'completed' ? 'Завершен' : '',
          variant: 'dark'
        }}
        className="flow-assigned-card__header"
        height="md"
      />

      {/* Content */}
      <div className="flow-assigned-card__content">
        {/* Status */}
        <div className="flow-assigned-card__status-row">
          <Status variant={status} size="md" />
          {status === 'completed' && (
            <Typography variant="caption" color="success" weight="medium">
              Ты всё закончил!
            </Typography>
          )}
        </div>

        {/* Title */}
        <Typography variant="h6" color="inherit" className="flow-assigned-card__title">
          {title}
        </Typography>

        {/* Current Step */}
        {currentStep && status !== 'completed' && (
          <div className="flow-assigned-card__step">
            <Typography variant="h5" color="gray" weight="normal" className="flow-assigned-card__step-number">
              {currentStep.number}
            </Typography>
            <div className="flow-assigned-card__step-content">
              <Typography variant="body2" color="gray" className="flow-assigned-card__step-description">
                {currentStep.description}
              </Typography>
            </div>
          </div>
        )}

        {/* Completion message */}
        {status === 'completed' && (
          <Typography variant="body2" color="gray" className="flow-assigned-card__completion-message">
            Можно пересмотреть материалы всего потока и статистику прохождения
          </Typography>
        )}

        {/* Progress */}
        {status !== 'not-started' && (
          <div className="flow-assigned-card__progress">
            <Progress
              value={progress}
              variant={getProgressVariant()}
              showLabel
              size="md"
            />
          </div>
        )}

        {/* Buddy */}
        {buddy && (
          <div className="flow-assigned-card__buddy">
            <Avatar src={buddy.avatar} alt={buddy.name} size="sm" />
            <Typography variant="body2" color="gray" className="flow-assigned-card__buddy-name">
              {buddy.name}
            </Typography>
          </div>
        )}

        {/* Action Button */}
        {onAction && (
          <div className="flow-assigned-card__action">
            <Button
              variant={status === 'completed' ? 'outline' : 'primary'}
              size="md"
              onClick={onAction}
              fullWidth
            >
              {getActionLabel()}
            </Button>
          </div>
        )}
      </div>
    </Card>
  );
}; 