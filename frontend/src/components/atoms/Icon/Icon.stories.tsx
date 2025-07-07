import type { Meta, StoryObj } from '@storybook/react';
import { Icon } from './Icon';

const meta = {
  title: 'Atoms/Icon',
  component: Icon,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  argTypes: {
    name: {
      control: { type: 'select' },
      options: ['lock', 'user', 'check', 'close', 'warning', 'info', 'edit', 'plus', 'minus', 'arrow'],
    },
    size: {
      control: { type: 'select' },
      options: ['xs', 'sm', 'md', 'lg', 'xl'],
    },
    color: {
      control: { type: 'select' },
      options: ['primary', 'secondary', 'success', 'warning', 'error', 'white', 'gray'],
    },
    className: {
      control: { type: 'text' },
    },
  },
} satisfies Meta<typeof Icon>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    name: 'user',
    size: 'md',
  },
};

export const Lock: Story = {
  args: {
    name: 'lock',
    size: 'md',
  },
};

export const Check: Story = {
  args: {
    name: 'check',
    size: 'md',
    color: 'success',
  },
};

export const Warning: Story = {
  args: {
    name: 'warning',
    size: 'md',
    color: 'warning',
  },
};

export const Small: Story = {
  args: {
    name: 'user',
    size: 'sm',
  },
};

export const Large: Story = {
  args: {
    name: 'user',
    size: 'lg',
  },
};

export const Primary: Story = {
  args: {
    name: 'info',
    size: 'md',
    color: 'primary',
  },
};

export const Error: Story = {
  args: {
    name: 'close',
    size: 'md',
    color: 'error',
  },
};

export const Clickable: Story = {
  args: {
    name: 'edit',
    size: 'md',
    onClick: () => console.log('Icon clicked'),
  },
}; 