import React from 'react';
import { useParams } from 'react-router-dom';
import { useGetFlowQuery } from '../store/api/graphql';

export const FlowDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { data: flow, isLoading } = useGetFlowQuery(id!);

  if (isLoading) return <div>Загрузка...</div>;
  if (!flow) return <div>Поток не найден</div>;

  return (
    <div className="flow-detail">
      <h1>{flow.name}</h1>
      <p>{flow.description}</p>
      <div>Статус: {flow.status}</div>
      <div>Шагов: {flow.totalSteps}</div>
    </div>
  );
}; 