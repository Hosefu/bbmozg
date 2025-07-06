# Архитектура фронтенда Lauf

## Обзор системы

Фронтенд Lauf представляет собой комплексное веб-приложение на базе React и TypeScript, построенное по принципам функционального программирования и атомарного дизайна. Система предназначена для управления образовательными потоками, пользователями и контентом через интуитивный интерфейс.

## Технический стек

### Основные технологии
- **React** - библиотека для создания пользовательских интерфейсов
- **TypeScript** - типизированный JavaScript для надежности кода
- **Redux Toolkit** - управление состоянием приложения
- **RTK Query** - декларативная работа с GraphQL API
- **React Router** - клиентская маршрутизация
- **Vite** - сборщик и dev-сервер для быстрой разработки

### Дизайн-система и UI
- **Styled Components** - CSS-in-JS для стилизации компонентов
- **Storybook** - изолированная разработка компонентов
- **Framer Motion** - анимации и переходы
- **React Spring** - физические анимации
- **SCSS** — препроссеор для всего style
- **Polished** - CSS-утилиты для styled-components

### Утилиты и инструменты
- **Lodash/FP** - функциональные утилиты
- **Date-fns** - работа с датами
- **React Hook Form** - управление формами
- **Zod** - валидация схем TypeScript
- **React Testing Library** - тестирование компонентов
- **MSW** - мокирование API для тестов

## Архитектурные принципы

### 1. Функциональное программирование
```typescript
// Использование pure функций
const calculateProgress = (completedSteps: number, totalSteps: number): number => 
  Math.round((completedSteps / totalSteps) * 100);

// Immutable обновления состояния
const updateUserProgress = (state: UserProgressState, action: UpdateProgressAction) => ({
  ...state,
  progress: {
    ...state.progress,
    [action.payload.flowId]: action.payload.progress
  }
});

// Композиция функций
const pipe = <T>(...fns: Array<(arg: T) => T>) => (value: T) => fns.reduce((acc, fn) => fn(acc), value);
```

### 2. Атомарный дизайн
```
atoms/          # Базовые элементы (кнопки, поля ввода)
molecules/      # Комбинации атомов (поисковая строка, карточка)
organisms/      # Сложные компоненты (хедер, форма)
templates/      # Макеты страниц
pages/          # Готовые страницы
```

### 3. Контейнер/Презентация разделение
```typescript
// Презентационный компонент
interface FlowCardProps {
  flow: FlowDto;
  onAssign: (flowId: string) => void;
  onEdit: (flowId: string) => void;
}

// Контейнер
const FlowCardContainer: React.FC<{ flowId: string }> = ({ flowId }) => {
  const { data: flow } = useGetFlowQuery(flowId);
  const [assignFlow] = useAssignFlowMutation();
  
  return (
    <FlowCard 
      flow={flow} 
      onAssign={assignFlow}
      onEdit={navigateToEdit}
    />
  );
};
```

## Структура проекта

```
src/
├── components/           # Компоненты дизайн-системы
│   ├── atoms/           # Атомы
│   │   ├── Button/
│   │   ├── Input/
│   │   ├── Icon/
│   │   └── Typography/
│   ├── molecules/       # Молекулы
│   │   ├── SearchBar/
│   │   ├── FlowCard/
│   │   └── UserAvatar/
│   ├── organisms/       # Организмы
│   │   ├── Header/
│   │   ├── Sidebar/
│   │   └── FlowList/
│   └── templates/       # Шаблоны
│       ├── DashboardTemplate/
│       └── FlowTemplate/
├── pages/               # Страницы приложения
│   ├── Dashboard/
│   ├── Flows/
│   ├── Users/
│   └── Settings/
├── hooks/               # Кастомные хуки
│   ├── useAuth.ts
│   ├── useProgress.ts
│   └── useDebounce.ts
├── store/               # Redux store
│   ├── api/             # RTK Query API
│   ├── slices/          # Redux slices
│   └── index.ts
├── utils/               # Утилитарные функции
│   ├── formatters.ts
│   ├── validators.ts
│   └── constants.ts
├── types/               # TypeScript типы
│   ├── api.ts
│   ├── entities.ts
│   └── common.ts
├── theme/               # Дизайн-система
│   ├── colors.ts
│   ├── typography.ts
│   ├── spacing.ts
│   └── breakpoints.ts
└── assets/              # Статические файлы
    ├── icons/
    ├── images/
    └── fonts/
```

## Дизайн-система

### Цветовая палитра
```typescript
export const colors = {
  primary: {
    50: '#eff6ff',
    100: '#dbeafe',
    500: '#3b82f6',
    600: '#2563eb',
    900: '#1e3a8a'
  },
  secondary: {
    50: '#f8fafc',
    100: '#f1f5f9',
    500: '#64748b',
    600: '#475569',
    900: '#0f172a'
  },
  success: {
    50: '#f0fdf4',
    500: '#22c55e',
    600: '#16a34a'
  },
  warning: {
    50: '#fffbeb',
    500: '#f59e0b',
    600: '#d97706'
  },
  error: {
    50: '#fef2f2',
    500: '#ef4444',
    600: '#dc2626'
  }
} as const;
```

### Типографика
```typescript
export const typography = {
  fontFamily: {
    sans: ['Inter', 'system-ui', 'sans-serif'],
    mono: ['SF Mono', 'Monaco', 'monospace']
  },
  fontSize: {
    xs: '0.75rem',
    sm: '0.875rem',
    base: '1rem',
    lg: '1.125rem',
    xl: '1.25rem',
    '2xl': '1.5rem',
    '3xl': '1.875rem',
    '4xl': '2.25rem'
  },
  fontWeight: {
    normal: '400',
    medium: '500',
    semibold: '600',
    bold: '700'
  },
  lineHeight: {
    tight: '1.25',
    normal: '1.5',
    relaxed: '1.75'
  }
} as const;
```

### Компоненты базового уровня

#### Button
```typescript
interface ButtonProps {
  variant?: 'primary' | 'secondary' | 'outline' | 'ghost';
  size?: 'sm' | 'md' | 'lg';
  disabled?: boolean;
  loading?: boolean;
  children: React.ReactNode;
  onClick?: () => void;
}

export const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  disabled = false,
  loading = false,
  children,
  onClick
}) => {
  const variants = {
    primary: css`
      background: ${colors.primary[500]};
      color: white;
      &:hover { background: ${colors.primary[600]}; }
    `,
    secondary: css`
      background: ${colors.secondary[100]};
      color: ${colors.secondary[900]};
      &:hover { background: ${colors.secondary[200]}; }
    `,
    outline: css`
      border: 1px solid ${colors.primary[500]};
      color: ${colors.primary[500]};
      background: transparent;
      &:hover { background: ${colors.primary[50]}; }
    `,
    ghost: css`
      background: transparent;
      color: ${colors.secondary[600]};
      &:hover { background: ${colors.secondary[100]}; }
    `
  };

  const sizes = {
    sm: css`
      padding: 0.5rem 1rem;
      font-size: ${typography.fontSize.sm};
    `,
    md: css`
      padding: 0.75rem 1.5rem;
      font-size: ${typography.fontSize.base};
    `,
    lg: css`
      padding: 1rem 2rem;
      font-size: ${typography.fontSize.lg};
    `
  };

  return (
    <StyledButton
      variant={variant}
      size={size}
      disabled={disabled || loading}
      onClick={onClick}
    >
      {loading && <Spinner />}
      {children}
    </StyledButton>
  );
};
```

#### Input
```typescript
interface InputProps {
  label?: string;
  placeholder?: string;
  value?: string;
  onChange?: (value: string) => void;
  error?: string;
  disabled?: boolean;
  type?: 'text' | 'email' | 'password' | 'number';
  icon?: React.ReactNode;
}

export const Input: React.FC<InputProps> = ({
  label,
  placeholder,
  value,
  onChange,
  error,
  disabled,
  type = 'text',
  icon
}) => (
  <InputWrapper>
    {label && <Label>{label}</Label>}
    <InputContainer hasError={!!error}>
      {icon && <IconContainer>{icon}</IconContainer>}
      <StyledInput
        type={type}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange?.(e.target.value)}
        disabled={disabled}
        hasIcon={!!icon}
      />
    </InputContainer>
    {error && <ErrorMessage>{error}</ErrorMessage>}
  </InputWrapper>
);
```

## GraphQL Integration

### API Layer
```typescript
// store/api/graphql.ts
import { createApi } from '@reduxjs/toolkit/query/react';
import { graphqlRequestBaseQuery } from '@rtk-query/graphql-request-base-query';

export const graphqlApi = createApi({
  reducerPath: 'graphqlApi',
  baseQuery: graphqlRequestBaseQuery({
    url: '/graphql',
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).auth.token;
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
      return headers;
    }
  }),
  tagTypes: ['Flow', 'User', 'Assignment', 'Progress'],
  endpoints: (builder) => ({
    // Queries
    getFlows: builder.query<FlowsResponse, GetFlowsArgs>({
      query: ({ skip = 0, take = 20 }) => ({
        document: GET_FLOWS_QUERY,
        variables: { skip, take }
      }),
      providesTags: ['Flow']
    }),
    
    getFlow: builder.query<FlowDetailsResponse, string>({
      query: (id) => ({
        document: GET_FLOW_QUERY,
        variables: { id }
      }),
      providesTags: (result, error, id) => [{ type: 'Flow', id }]
    }),
    
    getUsers: builder.query<UsersResponse, GetUsersArgs>({
      query: ({ skip = 0, take = 20 }) => ({
        document: GET_USERS_QUERY,
        variables: { skip, take }
      }),
      providesTags: ['User']
    }),
    
    // Mutations
    createFlow: builder.mutation<CreateFlowResponse, CreateFlowInput>({
      query: (input) => ({
        document: CREATE_FLOW_MUTATION,
        variables: { input }
      }),
      invalidatesTags: ['Flow']
    }),
    
    assignFlow: builder.mutation<AssignFlowResponse, AssignFlowInput>({
      query: (input) => ({
        document: ASSIGN_FLOW_MUTATION,
        variables: { input }
      }),
      invalidatesTags: ['Assignment']
    }),
    
    updateProgress: builder.mutation<UpdateProgressResponse, UpdateProgressInput>({
      query: (input) => ({
        document: UPDATE_PROGRESS_MUTATION,
        variables: { input }
      }),
      invalidatesTags: ['Progress']
    })
  })
});

export const {
  useGetFlowsQuery,
  useGetFlowQuery,
  useGetUsersQuery,
  useCreateFlowMutation,
  useAssignFlowMutation,
  useUpdateProgressMutation
} = graphqlApi;
```

### GraphQL Queries
```typescript
// store/api/queries.ts
import { gql } from 'graphql-request';

export const GET_FLOWS_QUERY = gql`
  query GetFlows($skip: Int!, $take: Int!) {
    getFlows(skip: $skip, take: $take) {
      id
      name
      description
      status
      priority
      isRequired
      createdAt
      publishedAt
      totalSteps
      settings {
        daysPerStep
        requireSequentialCompletionComponents
        allowSelfRestart
        allowSelfPause
      }
      steps {
        id
        name
        description
        order
        isRequired
        estimatedDurationMinutes
        totalComponents
        requiredComponents
      }
    }
  }
`;

export const GET_FLOW_QUERY = gql`
  query GetFlow($id: UUID!) {
    getFlow(id: $id) {
      id
      name
      description
      status
      priority
      isRequired
      createdAt
      publishedAt
      totalSteps
      settings {
        daysPerStep
        requireSequentialCompletionComponents
        allowSelfRestart
        allowSelfPause
      }
      steps {
        id
        name
        description
        order
        isRequired
        estimatedDurationMinutes
        instructions
        notes
        totalComponents
        requiredComponents
        components {
          id
          title
          description
          componentType
          order
          isRequired
          component {
            ... on ArticleComponent {
              id
              title
              description
              content
              readingTimeMinutes
            }
            ... on QuizComponent {
              id
              title
              description
              questions {
                id
                text
                isRequired
                order
                options {
                  id
                  text
                  isCorrect
                  order
                }
              }
            }
            ... on TaskComponent {
              id
              title
              description
              content
              isCaseSensitive
            }
          }
        }
      }
      statistics {
        totalAssignments
        activeAssignments
        completedAssignments
        averageProgress
        averageCompletionTime
      }
      userProgress {
        overallProgress
        currentStep
        completedSteps
        totalSteps
        startedAt
        lastActivityAt
        status
      }
    }
  }
`;

export const CREATE_FLOW_MUTATION = gql`
  mutation CreateFlow($input: CreateFlowInput!) {
    createFlow(input: $input) {
      id
      name
      description
      status
      createdAt
    }
  }
`;

export const ASSIGN_FLOW_MUTATION = gql`
  mutation AssignFlow($input: AssignFlowInput!) {
    assignFlow(input: $input) {
      assignmentId
      flowContentId
      isSuccess
      message
      estimatedCompletionDate
    }
  }
`;
```

### Type Definitions
```typescript
// types/api.ts
export interface FlowDto {
  id: string;
  name: string;
  description: string;
  status: FlowStatus;
  priority: number;
  isRequired: boolean;
  createdAt: string;
  publishedAt?: string;
  totalSteps: number;
  settings: FlowSettingsDto;
  steps: FlowStepDto[];
}

export interface FlowSettingsDto {
  daysPerStep: number;
  requireSequentialCompletionComponents: boolean;
  allowSelfRestart: boolean;
  allowSelfPause: boolean;
}

export interface FlowStepDto {
  id: string;
  name: string;
  description: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  instructions: string;
  notes: string;
  totalComponents: number;
  requiredComponents: number;
  components: FlowStepComponentDto[];
}

export interface FlowStepComponentDto {
  id: string;
  title: string;
  description: string;
  componentType: ComponentType;
  order: number;
  isRequired: boolean;
  component: ArticleComponentDto | QuizComponentDto | TaskComponentDto;
}

export interface ArticleComponentDto {
  id: string;
  title: string;
  description: string;
  content: string;
  readingTimeMinutes: number;
}

export interface QuizComponentDto {
  id: string;
  title: string;
  description: string;
  questions: QuizQuestionDto[];
}

export interface QuizQuestionDto {
  id: string;
  text: string;
  isRequired: boolean;
  order: number;
  options: QuestionOptionDto[];
}

export interface QuestionOptionDto {
  id: string;
  text: string;
  isCorrect: boolean;
  order: number;
}

export interface TaskComponentDto {
  id: string;
  title: string;
  description: string;
  content: string;
  isCaseSensitive: boolean;
}

export interface UserDto {
  id: string;
  telegramUserId?: number;
  firstName: string;
  lastName: string;
  username?: string;
  position?: string;
  department?: string;
  isActive: boolean;
  language: string;
  timezone: string;
  roles: string[];
  createdAt: string;
  lastActivityAt?: string;
}

export interface FlowAssignmentDto {
  id: string;
  userId: string;
  flowId: string;
  status: AssignmentStatus;
  assignedAt: string;
  completedAt?: string;
  deadline?: string;
  notes?: string;
  buddy?: string;
  user: UserDto;
  flow: FlowDto;
}

export enum FlowStatus {
  DRAFT = 'Draft',
  PUBLISHED = 'Published',
  ARCHIVED = 'Archived'
}

export enum AssignmentStatus {
  ASSIGNED = 'Assigned',
  IN_PROGRESS = 'InProgress',
  PAUSED = 'Paused',
  COMPLETED = 'Completed',
  CANCELLED = 'Cancelled',
  OVERDUE = 'Overdue'
}

export enum ComponentType {
  ARTICLE = 'Article',
  QUIZ = 'Quiz',
  TASK = 'Task'
}
```

## Страницы и навигация

### Routing (React Router v6)
```typescript
// App.tsx
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ProtectedRoute } from './components/auth/ProtectedRoute';
import { useEffect } from 'react';
import { useDispatch } from 'react-redux';
import { useLocation } from 'react-router-dom';
import { setCurrentPath } from './store/slices/routerSlice';

function AppRouter() {
  const dispatch = useDispatch();
  const location = useLocation();

  useEffect(() => {
    dispatch(setCurrentPath(location.pathname));
  }, [location.pathname, dispatch]);

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/" element={<ProtectedRoute />}>
        <Route index element={<DashboardPage />} />
        <Route path="flows">
          <Route index element={<FlowsPage />} />
          <Route path="create" element={<CreateFlowPage />} />
          <Route path=":id" element={<FlowDetailPage />} />
          <Route path=":id/edit" element={<EditFlowPage />} />
        </Route>
        <Route path="users">
          <Route index element={<UsersPage />} />
          <Route path="create" element={<CreateUserPage />} />
          <Route path=":id" element={<UserDetailPage />} />
          <Route path=":id/edit" element={<EditUserPage />} />
        </Route>
        <Route path="assignments">
          <Route index element={<AssignmentsPage />} />
          <Route path=":id" element={<AssignmentDetailPage />} />
        </Route>
        <Route path="progress" element={<ProgressPage />} />
        <Route path="settings" element={<SettingsPage />} />
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AppRouter />
    </BrowserRouter>
  );
}
```

### Основные страницы

#### 1. Dashboard (Главная)
```typescript
// pages/Dashboard/DashboardPage.tsx
export const DashboardPage: React.FC = () => {
  const { data: activeAssignments } = useGetActiveAssignmentsQuery();
  const { data: recentFlows } = useGetFlowsQuery({ take: 5 });
  const { data: overdueAssignments } = useGetOverdueAssignmentsQuery();

  return (
    <DashboardTemplate>
      <DashboardHeader />
      <StatsGrid>
        <StatCard 
          title="Активные задания" 
          value={activeAssignments?.length || 0}
          trend={+5}
        />
        <StatCard 
          title="Просроченные" 
          value={overdueAssignments?.length || 0}
          trend={-2}
          variant="warning"
        />
        <StatCard 
          title="Завершенные" 
          value={42}
          trend={+12}
          variant="success"
        />
      </StatsGrid>
      
      <ContentGrid>
        <Section>
          <SectionTitle>Мои активные задания</SectionTitle>
          <AssignmentList assignments={activeAssignments} />
        </Section>
        
        <Section>
          <SectionTitle>Недавние потоки</SectionTitle>
          <FlowList flows={recentFlows} />
        </Section>
      </ContentGrid>
    </DashboardTemplate>
  );
};
```

#### 2. Flows (Потоки)
```typescript
// pages/Flows/FlowsPage.tsx
export const FlowsPage: React.FC = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [filters, setFilters] = useState<FlowFilters>({});
  const { data: flows, isLoading } = useGetFlowsQuery({ 
    searchTerm, 
    filters 
  });

  return (
    <FlowsTemplate>
      <FlowsHeader>
        <PageTitle>Потоки обучения</PageTitle>
        <Button 
          variant="primary" 
          onClick={() => navigate('/flows/create')}
        >
          Создать поток
        </Button>
      </FlowsHeader>
      
      <FlowsFilters>
        <SearchBar 
          value={searchTerm}
          onChange={setSearchTerm}
          placeholder="Поиск потоков..."
        />
        <FilterDropdown 
          value={filters.status}
          onChange={(status) => setFilters({ ...filters, status })}
          options={flowStatusOptions}
        />
      </FlowsFilters>
      
      <FlowGrid>
        {isLoading ? (
          <LoadingGrid />
        ) : (
          flows?.map(flow => (
            <FlowCard 
              key={flow.id}
              flow={flow}
              onView={() => navigate(`/flows/${flow.id}`)}
              onEdit={() => navigate(`/flows/${flow.id}/edit`)}
              onAssign={() => openAssignModal(flow)}
            />
          ))
        )}
      </FlowGrid>
    </FlowsTemplate>
  );
};
```

#### 3. Flow Detail (Детали потока)
```typescript
// pages/Flows/FlowDetailPage.tsx
export const FlowDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const { data: flow, isLoading } = useGetFlowQuery(id!);
  const [activeStepId, setActiveStepId] = useState<string | null>(null);

  if (isLoading) return <LoadingPage />;
  if (!flow) return <NotFoundPage />;

  return (
    <FlowDetailTemplate>
      <FlowHeader>
        <FlowTitle>{flow.name}</FlowTitle>
        <FlowActions>
          <Button 
            variant="outline" 
            onClick={() => navigate(`/flows/${id}/edit`)}
          >
            Редактировать
          </Button>
          <Button 
            variant="primary"
            onClick={() => openAssignModal(flow)}
          >
            Назначить
          </Button>
        </FlowActions>
      </FlowHeader>

      <FlowContent>
        <FlowSidebar>
          <FlowInfo flow={flow} />
          <FlowStatistics statistics={flow.statistics} />
        </FlowSidebar>

        <FlowMain>
          <StepsList>
            {flow.steps.map(step => (
              <StepCard
                key={step.id}
                step={step}
                isActive={activeStepId === step.id}
                onClick={() => setActiveStepId(step.id)}
              />
            ))}
          </StepsList>
          
          {activeStepId && (
            <StepDetail 
              step={flow.steps.find(s => s.id === activeStepId)!}
            />
          )}
        </FlowMain>
      </FlowContent>
    </FlowDetailTemplate>
  );
};
```

#### 4. Users (Пользователи)
```typescript
// pages/Users/UsersPage.tsx
export const UsersPage: React.FC = () => {
  const { data: users, isLoading } = useGetUsersQuery();
  const [selectedUsers, setSelectedUsers] = useState<string[]>([]);

  return (
    <UsersTemplate>
      <UsersHeader>
        <PageTitle>Пользователи</PageTitle>
        <UserActions>
          <Button 
            variant="outline"
            disabled={selectedUsers.length === 0}
            onClick={() => openBulkAssignModal(selectedUsers)}
          >
            Назначить поток ({selectedUsers.length})
          </Button>
          <Button 
            variant="primary"
            onClick={() => navigate('/users/create')}
          >
            Добавить пользователя
          </Button>
        </UserActions>
      </UsersHeader>

      <UsersTable>
        <UserTableHeader>
          <Checkbox 
            checked={selectedUsers.length === users?.length}
            onChange={toggleSelectAll}
          />
          <SortableHeader column="name">Имя</SortableHeader>
          <SortableHeader column="position">Должность</SortableHeader>
          <SortableHeader column="department">Отдел</SortableHeader>
          <SortableHeader column="status">Статус</SortableHeader>
          <Header>Действия</Header>
        </UserTableHeader>

        {users?.map(user => (
          <UserTableRow key={user.id}>
            <Checkbox 
              checked={selectedUsers.includes(user.id)}
              onChange={() => toggleUser(user.id)}
            />
            <UserCell>
              <UserAvatar user={user} />
              <UserInfo>
                <UserName>{user.firstName} {user.lastName}</UserName>
                <UserEmail>{user.username}</UserEmail>
              </UserInfo>
            </UserCell>
            <Cell>{user.position}</Cell>
            <Cell>{user.department}</Cell>
            <Cell>
              <StatusBadge status={user.isActive ? 'active' : 'inactive'} />
            </Cell>
            <Cell>
              <ActionsMenu>
                <MenuItem onClick={() => navigate(`/users/${user.id}`)}>
                  Просмотр
                </MenuItem>
                <MenuItem onClick={() => navigate(`/users/${user.id}/edit`)}>
                  Редактировать
                </MenuItem>
                <MenuItem onClick={() => openAssignModal(user)}>
                  Назначить поток
                </MenuItem>
              </ActionsMenu>
            </Cell>
          </UserTableRow>
        ))}
      </UsersTable>
    </UsersTemplate>
  );
};
```

## Управление состоянием

### Redux Store
```typescript
// store/index.ts
import { configureStore } from '@reduxjs/toolkit';
import { graphqlApi } from './api/graphql';
import authSlice from './slices/authSlice';
import uiSlice from './slices/uiSlice';
import progressSlice from './slices/progressSlice';

export const store = configureStore({
  reducer: {
    [graphqlApi.reducerPath]: graphqlApi.reducer,
    auth: authSlice,
    ui: uiSlice,
    progress: progressSlice
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: [graphqlApi.util.resetApiState.type]
      }
    }).concat(graphqlApi.middleware)
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
```

### Auth Slice
```typescript
// store/slices/authSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { UserDto } from '../../types/api';

interface AuthState {
  isAuthenticated: boolean;
  user: UserDto | null;
  token: string | null;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  isAuthenticated: false,
  user: null,
  token: localStorage.getItem('token'),
  loading: false,
  error: null
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    loginStart: (state) => {
      state.loading = true;
      state.error = null;
    },
    loginSuccess: (state, action: PayloadAction<{ user: UserDto; token: string }>) => {
      state.isAuthenticated = true;
      state.user = action.payload.user;
      state.token = action.payload.token;
      state.loading = false;
      state.error = null;
      localStorage.setItem('token', action.payload.token);
    },
    loginError: (state, action: PayloadAction<string>) => {
      state.loading = false;
      state.error = action.payload;
    },
    logout: (state) => {
      state.isAuthenticated = false;
      state.user = null;
      state.token = null;
      localStorage.removeItem('token');
    }
  }
});

export const { loginStart, loginSuccess, loginError, logout } = authSlice.actions;
export default authSlice.reducer;
```

### UI Slice
```typescript
// store/slices/uiSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface UIState {
  sidebarOpen: boolean;
  theme: 'light' | 'dark';
  notifications: Notification[];
  modals: {
    assignFlow: boolean;
    createFlow: boolean;
    editUser: boolean;
  };
}

interface Notification {
  id: string;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
}

const initialState: UIState = {
  sidebarOpen: true,
  theme: 'light',
  notifications: [],
  modals: {
    assignFlow: false,
    createFlow: false,
    editUser: false
  }
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    toggleSidebar: (state) => {
      state.sidebarOpen = !state.sidebarOpen;
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.theme = action.payload;
    },
    addNotification: (state, action: PayloadAction<Omit<Notification, 'id'>>) => {
      const notification: Notification = {
        ...action.payload,
        id: Date.now().toString()
      };
      state.notifications.push(notification);
    },
    removeNotification: (state, action: PayloadAction<string>) => {
      state.notifications = state.notifications.filter(n => n.id !== action.payload);
    },
    openModal: (state, action: PayloadAction<keyof UIState['modals']>) => {
      state.modals[action.payload] = true;
    },
    closeModal: (state, action: PayloadAction<keyof UIState['modals']>) => {
      state.modals[action.payload] = false;
    }
  }
});

export const { 
  toggleSidebar, 
  setTheme, 
  addNotification, 
  removeNotification, 
  openModal, 
  closeModal 
} = uiSlice.actions;
export default uiSlice.reducer;
```

## Кастомные хуки

### useAuth
```typescript
// hooks/useAuth.ts
import { useSelector, useDispatch } from 'react-redux';
import { RootState } from '../store';
import { loginStart, loginSuccess, loginError, logout } from '../store/slices/authSlice';

export const useAuth = () => {
  const dispatch = useDispatch();
  const { isAuthenticated, user, loading, error } = useSelector((state: RootState) => state.auth);

  const login = async (credentials: LoginCredentials) => {
    dispatch(loginStart());
    try {
      const response = await authService.login(credentials);
      dispatch(loginSuccess(response));
    } catch (error) {
      dispatch(loginError(error.message));
    }
  };

  const handleLogout = () => {
    dispatch(logout());
  };

  return {
    isAuthenticated,
    user,
    loading,
    error,
    login,
    logout: handleLogout
  };
};
```

### useProgress
```typescript
// hooks/useProgress.ts
import { useCallback } from 'react';
import { useUpdateProgressMutation } from '../store/api/graphql';

export const useProgress = () => {
  const [updateProgress] = useUpdateProgressMutation();

  const markComponentComplete = useCallback(async (
    assignmentId: string,
    componentId: string,
    data?: any
  ) => {
    try {
      await updateProgress({
        assignmentId,
        componentId,
        isCompleted: true,
        data
      }).unwrap();
    } catch (error) {
      console.error('Failed to update progress:', error);
    }
  }, [updateProgress]);

  const updateComponentProgress = useCallback(async (
    assignmentId: string,
    componentId: string,
    progress: number,
    data?: any
  ) => {
    try {
      await updateProgress({
        assignmentId,
        componentId,
        progress,
        data
      }).unwrap();
    } catch (error) {
      console.error('Failed to update progress:', error);
    }
  }, [updateProgress]);

  return {
    markComponentComplete,
    updateComponentProgress
  };
};
```

## Форматирование и валидация

### Formatters
```typescript
// utils/formatters.ts
import { format, formatDistance, formatRelative } from 'date-fns';
import { ru } from 'date-fns/locale';

export const formatDate = (date: string | Date): string => {
  return format(new Date(date), 'dd.MM.yyyy', { locale: ru });
};

export const formatDateTime = (date: string | Date): string => {
  return format(new Date(date), 'dd.MM.yyyy HH:mm', { locale: ru });
};

export const formatRelativeTime = (date: string | Date): string => {
  return formatDistance(new Date(date), new Date(), { 
    addSuffix: true,
    locale: ru 
  });
};

export const formatProgress = (value: number): string => {
  return `${Math.round(value)}%`;
};

export const formatDuration = (minutes: number): string => {
  const hours = Math.floor(minutes / 60);
  const mins = minutes % 60;
  
  if (hours > 0) {
    return `${hours}ч ${mins}мин`;
  }
  return `${mins}мин`;
};
```

### Validators
```typescript
// utils/validators.ts
import { z } from 'zod';

export const createFlowSchema = z.object({
  name: z.string().min(1, 'Название обязательно').max(100, 'Слишком длинное название'),
  description: z.string().min(10, 'Описание должно быть не менее 10 символов'),
  isSequential: z.boolean(),
  allowRetry: z.boolean(),
  timeLimit: z.number().positive().optional(),
  passingScore: z.number().min(0).max(100).optional()
});

export const createUserSchema = z.object({
  firstName: z.string().min(1, 'Имя обязательно'),
  lastName: z.string().min(1, 'Фамилия обязательна'),
  telegramUserId: z.number().positive().optional(),
  position: z.string().optional(),
  department: z.string().optional()
});

export const assignFlowSchema = z.object({
  userId: z.string().uuid('Выберите пользователя'),
  flowId: z.string().uuid('Выберите поток'),
  dueDate: z.date().optional(),
  notes: z.string().optional()
});
```

## Тестирование

### Unit Tests
```typescript
// components/atoms/Button/Button.test.tsx
import { render, screen, fireEvent } from '@testing-library/react';
import { Button } from './Button';

describe('Button', () => {
  it('renders correctly', () => {
    render(<Button>Click me</Button>);
    expect(screen.getByRole('button')).toBeInTheDocument();
  });

  it('calls onClick when clicked', () => {
    const handleClick = jest.fn();
    render(<Button onClick={handleClick}>Click me</Button>);
    
    fireEvent.click(screen.getByRole('button'));
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('shows loading state', () => {
    render(<Button loading>Loading</Button>);
    expect(screen.getByTestId('spinner')).toBeInTheDocument();
  });

  it('is disabled when loading', () => {
    render(<Button loading>Loading</Button>);
    expect(screen.getByRole('button')).toBeDisabled();
  });
});
```

### Integration Tests
```typescript
// pages/Flows/FlowsPage.test.tsx
import { render, screen, waitFor } from '@testing-library/react';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { FlowsPage } from './FlowsPage';
import { store } from '../../store';

const renderWithProviders = (component: React.ReactElement) => {
  return render(
    <Provider store={store}>
      <BrowserRouter>
        {component}
      </BrowserRouter>
    </Provider>
  );
};

describe('FlowsPage', () => {
  it('renders flows list', async () => {
    renderWithProviders(<FlowsPage />);
    
    await waitFor(() => {
      expect(screen.getByText('Потоки обучения')).toBeInTheDocument();
    });
  });

  it('shows create flow button', () => {
    renderWithProviders(<FlowsPage />);
    
    expect(screen.getByText('Создать поток')).toBeInTheDocument();
  });

  it('filters flows by search term', async () => {
    renderWithProviders(<FlowsPage />);
    
    const searchInput = screen.getByPlaceholderText('Поиск потоков...');
    fireEvent.change(searchInput, { target: { value: 'React' } });
    
    await waitFor(() => {
      // Verify filtered results
    });
  });
});
```

## Развертывание и сборка

### Vite Configuration
```typescript
// vite.config.ts
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@components': resolve(__dirname, './src/components'),
      '@pages': resolve(__dirname, './src/pages'),
      '@hooks': resolve(__dirname, './src/hooks'),
      '@utils': resolve(__dirname, './src/utils'),
      '@types': resolve(__dirname, './src/types'),
      '@store': resolve(__dirname, './src/store'),
      '@theme': resolve(__dirname, './src/theme')
    }
  },
  server: {
    port: 3000,
    proxy: {
      '/graphql': {
        target: 'http://localhost:5000',
        changeOrigin: true
      }
    }
  },
  build: {
    outDir: 'dist',
    sourcemap: true,
    rollupOptions: {
      output: {
        manualChunks: {
          vendor: ['react', 'react-dom'],
          redux: ['@reduxjs/toolkit', 'react-redux'],
          router: ['react-router-dom'],
          ui: ['styled-components', 'framer-motion']
        }
      }
    }
  }
});
```

### Package.json Scripts
```json
{
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "preview": "vite preview",
    "test": "vitest",
    "test:ui": "vitest --ui",
    "test:coverage": "vitest --coverage",
    "storybook": "storybook dev -p 6006",
    "build-storybook": "storybook build",
    "lint": "eslint src --ext ts,tsx --report-unused-disable-directives --max-warnings 0",
    "lint:fix": "eslint src --ext ts,tsx --fix",
    "type-check": "tsc --noEmit"
  }
}
```

## Заключение

Данная архитектура обеспечивает:

1. **Масштабируемость** - модульная структура позволяет легко добавлять новые компоненты и страницы
2. **Типобезопасность** - полное покрытие TypeScript обеспечивает надежность кода
3. **Производительность** - оптимизированная работа с API через RTK Query и кэширование
4. **Поддерживаемость** - четкое разделение ответственности и функциональный подход
5. **Тестируемость** - изолированные компоненты легко тестируются
6. **Консистентность** - единая дизайн-система обеспечивает согласованность интерфейса

Архитектура следует принципам DDD и чистой архитектуры, обеспечивая четкое разделение между бизнес-логикой, представлением и инфраструктурой. 