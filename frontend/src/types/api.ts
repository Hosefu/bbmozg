// =============================================================================
// API Types - Generated from GraphQL Schema
// =============================================================================

// Base Types
export type UUID = string;

// Enums
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

export enum NotificationStatus {
  UNREAD = 'Unread',
  READ = 'Read',
  ARCHIVED = 'Archived'
}

export enum NotificationPriority {
  LOW = 'Low',
  MEDIUM = 'Medium',
  HIGH = 'High',
  URGENT = 'Urgent'
}

export enum ProgressStatus {
  NOT_STARTED = 'NotStarted',
  IN_PROGRESS = 'InProgress',
  COMPLETED = 'Completed',
  FAILED = 'Failed'
}

// User Types
export interface UserDto {
  id: UUID;
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

export interface CreateUserInput {
  firstName: string;
  lastName: string;
  telegramUserId?: number;
}

export interface UpdateUserInput {
  userId: UUID;
  firstName?: string;
  lastName?: string;
  telegramUserId?: number;
  roleIds?: UUID[];
}

// Flow Types
export interface FlowDto {
  id: UUID;
  name: string;
  description: string;
  status: FlowStatus;
  priority: number;
  isRequired: boolean;
  createdById: UUID;
  createdAt: string;
  updatedAt?: string;
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
  id: UUID;
  flowContentId: UUID;
  name: string;
  description: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  isEnabled: boolean;
  instructions: string;
  notes: string;
  createdAt: string;
  updatedAt?: string;
  totalComponents: number;
  requiredComponents: number;
  components: FlowStepComponentDto[];
}

export interface FlowStepComponentDto {
  id: UUID;
  flowStepId: UUID;
  componentId: UUID;
  componentType: ComponentType;
  title: string;
  description: string;
  order: number;
  isRequired: boolean;
  isEnabled: boolean;
  component: ArticleComponentDto | QuizComponentDto | TaskComponentDto;
}

// Component Types
export interface ArticleComponentDto {
  id: UUID;
  title: string;
  description: string;
  content: string;
  type: ComponentType;
  readingTimeMinutes: number;
}

export interface QuizComponentDto {
  id: UUID;
  title: string;
  description: string;
  content: string;
  type: ComponentType;
  questions: QuizQuestionDto[];
}

export interface QuizQuestionDto {
  id: UUID;
  text: string;
  isRequired: boolean;
  order: number;
  options: QuestionOptionDto[];
}

export interface QuestionOptionDto {
  id: UUID;
  text: string;
  isCorrect: boolean;
  order: number;
}

export interface TaskComponentDto {
  id: UUID;
  title: string;
  description: string;
  content: string;
  type: ComponentType;
  isCaseSensitive: boolean;
}

// Flow Assignment Types
export interface FlowAssignmentDto {
  id: UUID;
  userId: UUID;
  flowId: UUID;
  status: AssignmentStatus;
  assignedAt: string;
  completedAt?: string;
  deadline?: string;
  notes?: string;
  buddy?: UUID;
  user: UserDto;
  flow: FlowDto;
  assignedBy?: UserDto;
}

// Progress Types
export interface ComponentProgressDto {
  isCompleted: boolean;
  progress: number;
  data?: any;
  completedAt?: string;
}

export interface UserFlowProgressDto {
  overallProgress: number;
  currentStep: number;
  completedSteps: number;
  totalSteps: number;
  startedAt: string;
  lastActivityAt?: string;
  status: AssignmentStatus;
}

// Detailed Flow Types
export interface FlowDetailsDto extends FlowDto {
  statistics: FlowStatisticsDto;
  userProgress?: UserFlowProgressDto;
}

export interface FlowStepDetailsDto {
  id: UUID;
  flowContentId: UUID;
  name: string;
  description: string;
  order: number;
  isRequired: boolean;
  estimatedDurationMinutes: number;
  isEnabled: boolean;
  instructions: string;
  notes: string;
  createdAt: string;
  updatedAt?: string;
  totalComponents: number;
  requiredComponents: number;
  components: FlowStepComponentDetailsDto[];
  isAccessible: boolean;
  isCompleted: boolean;
}

export interface FlowStepComponentDetailsDto {
  id: UUID;
  title: string;
  description: string;
  componentType: ComponentType;
  content: string;
  settings?: any;
  isRequired: boolean;
  order: number;
  progress: ComponentProgressDto;
}

export interface FlowStatisticsDto {
  totalAssignments: number;
  activeAssignments: number;
  completedAssignments: number;
  averageProgress: number;
  averageCompletionTime?: number;
}

// Input Types
export interface CreateFlowInput {
  title: string;
  description: string;
  isSequential?: boolean;
  allowRetry?: boolean;
  timeLimit?: number;
  passingScore?: number;
}

export interface UpdateFlowInput {
  id: UUID;
  title?: string;
  description?: string;
  status?: FlowStatus;
}

export interface AssignFlowInput {
  userId: UUID;
  flowId: UUID;
  dueDate?: string;
  assignedBy?: UUID;
}

export interface CreateFlowStepInput {
  flowId: UUID;
  title: string;
  description: string;
  isRequired?: boolean;
  instructions?: string;
  notes?: string;
}

export interface CreateArticleComponentInput {
  flowStepId: UUID;
  title: string;
  description: string;
  content: string;
  readingTimeMinutes?: number;
  isRequired?: boolean;
}

export interface CreateQuizComponentInput {
  flowStepId: UUID;
  title: string;
  description: string;
  questions: CreateQuizQuestionInput[];
  isRequired?: boolean;
}

export interface CreateQuizQuestionInput {
  text: string;
  isRequired?: boolean;
  options: CreateQuestionOptionInput[];
}

export interface CreateQuestionOptionInput {
  text: string;
  isCorrect: boolean;
  message?: string;
}

export interface CreateTaskComponentInput {
  flowStepId: UUID;
  title: string;
  description: string;
  instruction: string;
  codeWord: string;
  hint?: string;
  isRequired?: boolean;
  estimatedDurationMinutes?: number;
}

export interface CreateComponentInput {
  article?: CreateArticleComponentInput;
  quiz?: CreateQuizComponentInput;
  task?: CreateTaskComponentInput;
}

// Result Types
export interface AssignFlowResult {
  assignmentId: UUID;
  flowContentId: UUID;
  isSuccess: boolean;
  message: string;
  estimatedCompletionDate?: string;
}

export interface CreateFlowStepResult {
  stepId: UUID;
  isSuccess: boolean;
  message: string;
}

export interface CreateComponentResult {
  article?: ArticleComponentResult;
  quiz?: QuizComponentResult;
  task?: TaskComponentResult;
}

export interface ArticleComponentResult {
  isSuccess: boolean;
  message: string;
  componentId?: UUID;
  component?: ArticleComponentDto;
}

export interface QuizComponentResult {
  isSuccess: boolean;
  message: string;
  componentId?: UUID;
  component?: QuizComponentDto;
}

export interface TaskComponentResult {
  isSuccess: boolean;
  message: string;
  componentId?: UUID;
  component?: TaskComponentDto;
}

// Query Response Types
export interface GetUsersResponse {
  users: UserDto[];
  totalCount: number;
  hasMore: boolean;
}

export interface GetFlowsResponse {
  flows: FlowDto[];
  totalCount: number;
  hasMore: boolean;
}

export interface GetFlowAssignmentsResponse {
  assignments: FlowAssignmentDto[];
  totalCount: number;
  hasMore: boolean;
}

// Query Arguments
export interface GetUsersArgs {
  skip?: number;
  take?: number;
  searchTerm?: string;
  isActive?: boolean;
  departmentFilter?: string;
}

export interface GetFlowsArgs {
  skip?: number;
  take?: number;
  searchTerm?: string;
  status?: FlowStatus;
  priority?: number;
  includeSteps?: boolean;
}

export interface GetFlowAssignmentsArgs {
  skip?: number;
  take?: number;
  userId?: UUID;
  flowId?: UUID;
  status?: AssignmentStatus;
  overdue?: boolean;
}

// Notification Types
export interface NotificationDto {
  id: UUID;
  userId: UUID;
  type: string;
  title: string;
  message: string;
  data?: any;
  status: NotificationStatus;
  priority: NotificationPriority;
  createdAt: string;
  readAt?: string;
  archivedAt?: string;
}

// Dev Authentication Types
export interface DevAuthData {
  userId: UUID;
  firstName: string;
  lastName: string;
  username?: string;
  position?: string;
  department?: string;
  roles?: string[];
}

// Error Types
export interface GraphQLError {
  message: string;
  extensions?: {
    code?: string;
    field?: string;
  };
}

export interface ApiError {
  message: string;
  code?: string;
  field?: string;
  details?: any;
}

// Pagination Types
export interface PaginationInfo {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasMore: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  pagination: PaginationInfo;
}

// Search and Filter Types
export interface SearchFilters {
  searchTerm?: string;
  status?: string;
  priority?: number;
  dateFrom?: string;
  dateTo?: string;
  tags?: string[];
}

export interface SortOptions {
  field: string;
  direction: 'asc' | 'desc';
}

// Common Response Types
export interface MutationResponse {
  success: boolean;
  message: string;
  data?: any;
}

export interface BulkOperationResponse {
  successCount: number;
  failedCount: number;
  errors: string[];
}

// Form Types
export interface FormState<T> {
  data: T;
  errors: Record<string, string>;
  isSubmitting: boolean;
  isDirty: boolean;
  isValid: boolean;
}

// Table Types
export interface TableColumn<T> {
  key: keyof T;
  label: string;
  sortable?: boolean;
  width?: string;
  render?: (value: any, row: T) => any;
}

export interface TableProps<T> {
  data: T[];
  columns: TableColumn<T>[];
  loading?: boolean;
  onSort?: (field: keyof T, direction: 'asc' | 'desc') => void;
  onRowClick?: (row: T) => void;
  selection?: {
    selectedRows: string[];
    onSelect: (id: string) => void;
    onSelectAll: (ids: string[]) => void;
  };
} 