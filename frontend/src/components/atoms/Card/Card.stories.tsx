import type { Meta, StoryObj } from '@storybook/react';
import { Card } from './Card';

const meta = {
  title: 'Atoms/Card',
  component: Card,
  parameters: {
    layout: 'centered',
  },
  tags: ['autodocs'],
  argTypes: {
    variant: {
      control: { type: 'select' },
      options: ['default', 'elevated', 'outlined'],
    },
    padding: {
      control: { type: 'select' },
      options: ['none', 'sm', 'md', 'lg'],
    },
    rounded: {
      control: { type: 'select' },
      options: ['none', 'sm', 'md', 'lg'],
    },
    shadow: {
      control: { type: 'select' },
      options: ['none', 'sm', 'md', 'lg'],
    },
    children: {
      control: { type: 'text' },
    },
  },
} satisfies Meta<typeof Card>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    children: 'Это обычная карточка с содержимым',
  },
};

export const Elevated: Story = {
  args: {
    variant: 'elevated',
    children: 'Карточка с тенью',
  },
};

export const Outlined: Story = {
  args: {
    variant: 'outlined',
    children: 'Карточка с обводкой',
  },
};

export const NoPadding: Story = {
  args: {
    padding: 'none',
    children: 'Карточка без отступов',
  },
};

export const SmallPadding: Story = {
  args: {
    padding: 'sm',
    children: 'Карточка с маленькими отступами',
  },
};

export const LargePadding: Story = {
  args: {
    padding: 'lg',
    children: 'Карточка с большими отступами',
  },
};

export const Clickable: Story = {
  args: {
    children: 'Кликабельная карточка',
    onClick: () => console.log('Card clicked'),
  },
};

export const WithContent: Story = {
  args: {
    variant: 'elevated',
    padding: 'md',
    rounded: 'md',
    children: (
      <div>
        <h3 style={{ margin: '0 0 8px 0' }}>Заголовок карточки</h3>
        <p style={{ margin: '0 0 16px 0', color: '#666' }}>
          Здесь может быть описание или любое другое содержимое карточки.
        </p>
        <button style={{ padding: '8px 16px', border: 'none', borderRadius: '4px', background: '#007bff', color: 'white' }}>
          Действие
        </button>
      </div>
    ),
  },
}; 