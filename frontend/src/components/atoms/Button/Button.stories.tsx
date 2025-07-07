import type { Meta, StoryObj } from '@storybook/react';
import { Button } from './Button';

const meta = {
  title: 'Atoms/Button',
  component: Button,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['primary', 'secondary', 'outline', 'ghost', 'danger'],
    },
    size: {
      control: { type: 'select' },
      options: ['sm', 'md', 'lg'],
    },
    disabled: {
      control: { type: 'boolean' },
    },
    loading: {
      control: { type: 'boolean' },
    },
    fullWidth: {
      control: { type: 'boolean' },
    },
    children: {
      control: { type: 'text' },
    },
  },
  args: {
    onClick: () => console.log('Button clicked'),
  },
} satisfies Meta<typeof Button>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Primary: Story = {
  args: {
    variant: 'primary',
    children: 'Кнопка',
  },
};

export const Secondary: Story = {
  args: {
    variant: 'secondary',
    children: 'Вторичная кнопка',
  },
};

export const Outline: Story = {
  args: {
    variant: 'outline',
    children: 'Обведенная кнопка',
  },
};

export const Ghost: Story = {
  args: {
    variant: 'ghost',
    children: 'Призрачная кнопка',
  },
};

export const Danger: Story = {
  args: {
    variant: 'danger',
    children: 'Опасная кнопка',
  },
};

export const Small: Story = {
  args: {
    size: 'sm',
    children: 'Маленькая',
  },
};

export const Large: Story = {
  args: {
    size: 'lg',
    children: 'Большая кнопка',
  },
};

export const Loading: Story = {
  args: {
    loading: true,
    children: 'Загрузка',
  },
};

export const Disabled: Story = {
  args: {
    disabled: true,
    children: 'Недоступна',
  },
};

export const FullWidth: Story = {
  args: {
    fullWidth: true,
    children: 'На всю ширину',
  },
  parameters: {
    layout: 'padded',
  },
}; 