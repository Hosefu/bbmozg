# Lauf Frontend

Фронтенд приложение для системы управления образовательными потоками Lauf.

## Технологии

- **React 18.2+** - библиотека для создания пользовательских интерфейсов
- **TypeScript 5.0+** - типизированный JavaScript
- **Redux Toolkit** - управление состоянием
- **RTK Query** - API клиент с кэшированием
- **React Router v6** - клиентская маршрутизация
- **SCSS** - препроцессор CSS
- **Vite** - сборщик и dev-сервер

## Установка и запуск

1. Установите зависимости:
```bash
npm install
```

2. Запустите development сервер:
```bash
npm run dev
```

3. Откройте [http://localhost:3000](http://localhost:3000) в браузере

## Доступные команды

- `npm run dev` - запуск development сервера
- `npm run build` - сборка для production
- `npm run preview` - предварительный просмотр production сборки
- `npm run lint` - проверка кода с ESLint
- `npm run type-check` - проверка типов TypeScript

## Структура проекта

```
frontend/
├── src/
│   ├── components/        # React компоненты
│   │   ├── atoms/         # Атомарные компоненты (Button, Input)
│   │   ├── molecules/     # Молекулы (Form, Card)
│   │   ├── organisms/     # Организмы (Header, Sidebar)
│   │   ├── templates/     # Шаблоны страниц
│   │   ├── auth/          # Компоненты авторизации
│   │   └── layout/        # Layout компоненты
│   ├── pages/             # Страницы приложения
│   ├── store/             # Redux store и slices
│   │   ├── api/           # RTK Query API
│   │   └── slices/        # Redux slices
│   ├── types/             # TypeScript типы
│   ├── utils/             # Утилиты и хелперы
│   ├── hooks/             # Кастомные React хуки
│   ├── styles/            # SCSS стили
│   └── assets/            # Статические ресурсы
├── public/                # Публичные файлы
├── package.json
├── vite.config.ts
├── tsconfig.json
└── README.md
```

## Архитектурные принципы

### Atomic Design
Компоненты организованы по принципу атомарного дизайна:
- **Atoms** - базовые компоненты (Button, Input, Icon)
- **Molecules** - простые составные компоненты (SearchBox, Card)
- **Organisms** - сложные компоненты (Header, Table, Form)
- **Templates** - макеты страниц
- **Pages** - готовые страницы

### State Management
- **Redux Toolkit** для глобального состояния
- **RTK Query** для работы с API и кэширования
- **Local state** для локального состояния компонентов

### Функциональное программирование
- Использование pure функций
- Immutable обновления состояния
- Композиция функций
- Избегание мутаций

## Dev-авторизация

Для разработки реализована специальная система авторизации, которая имитирует аутентификацию без реального бэкенда:

1. На странице входа можно ввести данные пользователя
2. Доступны готовые пресеты (Админ, Менеджер, Пользователь)
3. Данные сохраняются в localStorage
4. Бэкенд получает информацию через специальные заголовки

### Заголовки dev-авторизации

- `X-Dev-User-Id` - ID пользователя
- `X-Dev-User-Name` - Имя пользователя
- `X-Dev-User-Username` - Username
- `X-Dev-User-Position` - Должность
- `X-Dev-User-Department` - Отдел

## Дизайн-система

### Цветовая палитра
Используется система цветов с оттенками от 50 до 950:
- **Primary** - основной цвет интерфейса (синий)
- **Secondary** - вторичный цвет (серый)
- **Success** - успешные действия (зеленый)
- **Warning** - предупреждения (желтый)
- **Error** - ошибки (красный)

### Типографика
- **Font Family**: Inter (с fallback на системные шрифты)
- **Размеры**: от xs (12px) до 6xl (60px)
- **Веса**: от thin (100) до black (900)

### Spacing
Используется 4px grid система:
- 1 = 4px
- 2 = 8px
- 3 = 12px
- 4 = 16px
- и т.д.

### Компоненты
Все компоненты следуют единому стилю и поддерживают:
- Различные размеры (sm, md, lg)
- Различные варианты (primary, secondary, outline, ghost)
- Состояния (disabled, loading, focus)
- Accessibility (ARIA атрибуты, keyboard navigation)

## GraphQL Integration

### API Клиент
RTK Query настроен для работы с GraphQL:
- Автоматическое кэширование запросов
- Optimistic updates
- Error handling
- Loading states

### Типизация
Все GraphQL типы автоматически генерируются из схемы и доступны в `src/types/api.ts`

## Routing

Использует React Router v6 с защищенными маршрутами:
- Публичные маршруты: `/login`
- Защищенные маршруты: `/`, `/flows`, `/users`, `/assignments`
- Автоматический редирект на login при отсутствии авторизации

## Responsive Design

Приложение адаптивное и поддерживает:
- Mobile (< 640px)
- Tablet (640px - 1024px)
- Desktop (> 1024px)

## Production Build

Для production сборки:
```bash
npm run build
```

Результат будет в папке `dist/` и готов для деплоя на любой статический хостинг.

## Развитие

### Добавление новых компонентов
1. Создайте компонент в соответствующей папке
2. Добавьте SCSS стили
3. Экспортируйте из index файла
4. Добавьте типы если необходимо

### Добавление новых страниц
1. Создайте компонент в `src/pages/`
2. Добавьте маршрут в `App.tsx`
3. Добавьте навигацию в `Layout.tsx`

### Добавление API методов
1. Добавьте GraphQL query/mutation в `src/store/api/graphql.ts`
2. Используйте сгенерированный хук в компонентах
3. Обновите типы в `src/types/api.ts` при необходимости 