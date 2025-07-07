import type { Meta, StoryObj } from '@storybook/react';
import { FlowCard } from './FlowCard';

const meta = {
  title: 'Molecules/FlowCard',
  component: FlowCard,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  argTypes: {
    title: {
      control: { type: 'text' },
    },
    description: {
      control: { type: 'text' },
    },
    duration: {
      control: { type: 'text' },
    },
    stepsCount: {
      control: { type: 'number' },
    },
    mentors: {
      control: { type: 'object' },
    },
    backgroundImage: {
      control: { type: 'text' },
    },
    isEditable: {
      control: { type: 'boolean' },
    },
    isAssignable: {
      control: { type: 'boolean' },
    },
    isDraft: {
      control: { type: 'boolean' },
    },
  },
} satisfies Meta<typeof FlowCard>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    title: 'Основы программирования',
    description: 'Изучите основы программирования на Python с нуля',
    duration: '6 недель',
    stepsCount: 15,
    mentors: [
      { name: 'Иван Петров', avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=32&h=32&fit=crop&crop=face' },
      { name: 'Анна Смирнова', avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=32&h=32&fit=crop&crop=face' },
    ],
    isEditable: true,
    isAssignable: true,
    isDraft: false,
    onEdit: () => console.log('Edit flow'),
    onAssign: () => console.log('Assign flow'),
  },
};

export const WithBackground: Story = {
  args: {
    title: 'Веб-разработка',
    description: 'Полный курс по созданию современных веб-приложений',
    duration: '8 недель',
    stepsCount: 12,
    mentors: [
      { name: 'Мария Сидорова', avatar: 'https://images.unsplash.com/photo-1494790108755-2616b2e9cd48?w=32&h=32&fit=crop&crop=face' },
    ],
    backgroundImage: 'https://images.unsplash.com/photo-1547658719-da2b51169166?w=400&h=240&fit=crop',
    isEditable: true,
    isAssignable: true,
    isDraft: false,
    onEdit: () => console.log('Edit flow'),
    onAssign: () => console.log('Assign flow'),
  },
};

export const Draft: Story = {
  args: {
    title: 'Machine Learning',
    description: 'Глубокое погружение в алгоритмы машинного обучения',
    duration: '12 недель',
    stepsCount: 20,
    mentors: [
      { name: 'Алексей Козлов', avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=32&h=32&fit=crop&crop=face' },
    ],
    isEditable: true,
    isAssignable: false,
    isDraft: true,
    onEdit: () => console.log('Edit flow'),
    onAssign: () => console.log('Assign flow'),
  },
};

export const ReadOnly: Story = {
  args: {
    title: 'Git basics',
    description: 'Краткий курс по основам Git',
    duration: '1 неделя',
    stepsCount: 5,
    mentors: [
      { name: 'Анна Волкова', avatar: 'https://images.unsplash.com/photo-1517841905240-472988babdf9?w=32&h=32&fit=crop&crop=face' },
    ],
    isEditable: false,
    isAssignable: true,
    isDraft: false,
    onEdit: () => console.log('Edit flow'),
    onAssign: () => console.log('Assign flow'),
  },
}; 