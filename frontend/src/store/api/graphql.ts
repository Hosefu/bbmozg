import { createApi } from '@reduxjs/toolkit/query/react';
import { graphqlRequestBaseQuery } from '@rtk-query/graphql-request-base-query';
import { gql } from 'graphql-request';
import type {
  UserDto,
  FlowDto,
  FlowDetailsDto,
  FlowAssignmentDto,
  CreateUserInput,
  UpdateUserInput,
  CreateFlowInput,
  UpdateFlowInput,
  AssignFlowInput,
  CreateFlowStepInput,
  CreateComponentInput,
  GetUsersArgs,
  GetFlowsArgs,
  GetFlowAssignmentsArgs,
  AssignFlowResult,
  CreateFlowStepResult,
  CreateComponentResult,
  UUID
} from '../../types/api';

// GraphQL Queries
const GET_USERS_QUERY = gql`
  query GetUsers {
    users {
      id
      telegramUserId
      firstName
      lastName
      username
      position
      department
      isActive
      language
      timezone
      roles
      createdAt
      lastActivityAt
    }
  }
`;

const GET_USER_QUERY = gql`
  query GetUser($id: UUID!) {
    getUser(id: $id) {
      id
      telegramUserId
      firstName
      lastName
      username
      position
      department
      isActive
      language
      timezone
      roles
      createdAt
      lastActivityAt
    }
  }
`;

const GET_FLOWS_QUERY = gql`
  query GetFlows {
    flows {
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

const GET_FLOW_QUERY = gql`
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

const SEARCH_FLOWS_QUERY = gql`
  query SearchFlows($searchTerm: String!, $skip: Int!, $take: Int!) {
    searchFlows(searchTerm: $searchTerm, skip: $skip, take: $take) {
      id
      name
      description
      status
      priority
      isRequired
      createdAt
      publishedAt
      totalSteps
    }
  }
`;

const GET_FLOW_ASSIGNMENTS_QUERY = gql`
  query GetFlowAssignments($skip: Int!, $take: Int!, $userId: UUID, $flowId: UUID) {
    getFlowAssignments(skip: $skip, take: $take, userId: $userId, flowId: $flowId) {
      id
      userId
      flowId
      status
      assignedAt
      completedAt
      deadline
      notes
      user {
        id
        firstName
        lastName
        username
        position
        department
      }
      flow {
        id
        name
        description
        status
        totalSteps
      }
    }
  }
`;

const GET_ACTIVE_ASSIGNMENTS_QUERY = gql`
  query GetActiveAssignments($userId: UUID!) {
    getActiveAssignments(userId: $userId) {
      id
      userId
      flowId
      status
      assignedAt
      deadline
      flow {
        id
        name
        description
        totalSteps
      }
    }
  }
`;

const GET_OVERDUE_ASSIGNMENTS_QUERY = gql`
  query GetOverdueAssignments {
    getOverdueAssignments {
      id
      userId
      flowId
      status
      assignedAt
      deadline
      user {
        id
        firstName
        lastName
      }
      flow {
        id
        name
        description
      }
    }
  }
`;

// GraphQL Mutations
const CREATE_USER_MUTATION = gql`
  mutation CreateUser($input: CreateUserInput!) {
    createUser(input: $input) {
      id
      firstName
      lastName
      telegramUserId
      isActive
      createdAt
    }
  }
`;

const UPDATE_USER_MUTATION = gql`
  mutation UpdateUser($input: UpdateUserInput!) {
    updateUser(input: $input) {
      id
      firstName
      lastName
      telegramUserId
      isActive
    }
  }
`;

const CREATE_FLOW_MUTATION = gql`
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

const UPDATE_FLOW_MUTATION = gql`
  mutation UpdateFlow($input: UpdateFlowInput!) {
    updateFlow(input: $input) {
      flow {
        id
        name
        description
        status
        updatedAt
      }
    }
  }
`;

const ASSIGN_FLOW_MUTATION = gql`
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

const CREATE_FLOW_STEP_MUTATION = gql`
  mutation CreateFlowStep($input: CreateFlowStepInput!) {
    createFlowStep(input: $input) {
      stepId
      isSuccess
      message
    }
  }
`;

const CREATE_COMPONENT_MUTATION = gql`
  mutation CreateComponent($input: CreateComponentInput!) {
    createComponent(input: $input) {
      ... on CreateComponentResult {
        article {
          isSuccess
          message
          componentId
          component {
            id
            title
            description
            content
            readingTimeMinutes
          }
        }
        quiz {
          isSuccess
          message
          componentId
          component {
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
        }
        task {
          isSuccess
          message
          componentId
          component {
            id
            title
            description
            content
            isCaseSensitive
          }
        }
      }
    }
  }
`;

// API Definition
export const graphqlApi = createApi({
  reducerPath: 'graphqlApi',
  baseQuery: graphqlRequestBaseQuery({
    url: `${import.meta.env.VITE_API_URL}/graphql`,
    prepareHeaders: (headers, { getState }) => {
      const state = getState() as any;
      const token = state.auth?.token;
      
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
      
      // Dev headers for mock authentication
      const devAuth = state.auth?.devAuth;
      if (devAuth) {
        headers.set('X-Dev-User-Id', devAuth.userId);
        headers.set('X-Dev-User-Name', encodeURIComponent(`${devAuth.firstName} ${devAuth.lastName}`));
        if (devAuth.username) {
          headers.set('X-Dev-User-Username', encodeURIComponent(devAuth.username));
        }
        if (devAuth.position) {
          headers.set('X-Dev-User-Position', encodeURIComponent(devAuth.position));
        }
        if (devAuth.department) {
          headers.set('X-Dev-User-Department', encodeURIComponent(devAuth.department));
        }
      }
      
      return headers;
    },
    transformErrorResponse: (response: any) => {
      console.error('GraphQL Error:', response);
      return response;
    },
  }),
  tagTypes: ['User', 'Flow', 'FlowAssignment', 'Progress', 'Component'],
  endpoints: (builder) => ({
    // User Queries
    getUsers: builder.query<UserDto[], void>({
      query: () => ({
        document: GET_USERS_QUERY,
      }),
      transformResponse: (response: { users: UserDto[] }) => response.users,
      providesTags: ['User'],
    }),

    getUser: builder.query<UserDto | null, UUID>({
      query: (id) => ({
        document: GET_USER_QUERY,
        variables: { id },
      }),
      transformResponse: (response: { getUser: UserDto | null }) => response.getUser,
      providesTags: (_result, _error, id) => [{ type: 'User', id }],
    }),

    // Flow Queries
    getFlows: builder.query<FlowDto[], void>({
      query: () => ({
        document: GET_FLOWS_QUERY,
      }),
      transformResponse: (response: { flows: FlowDto[] }) => response.flows,
      providesTags: ['Flow'],
    }),

    getFlow: builder.query<FlowDetailsDto | null, UUID>({
      query: (id) => ({
        document: GET_FLOW_QUERY,
        variables: { id },
      }),
      transformResponse: (response: { getFlow: FlowDetailsDto | null }) => response.getFlow,
      providesTags: (_result, _error, id) => [{ type: 'Flow', id }],
    }),

    searchFlows: builder.query<FlowDto[], { searchTerm: string; skip?: number; take?: number }>({
      query: ({ searchTerm, skip = 0, take = 20 }) => ({
        document: SEARCH_FLOWS_QUERY,
        variables: { searchTerm, skip, take },
      }),
      transformResponse: (response: { searchFlows: FlowDto[] }) => response.searchFlows,
      providesTags: ['Flow'],
    }),

    // Assignment Queries
    getFlowAssignments: builder.query<FlowAssignmentDto[], GetFlowAssignmentsArgs>({
      query: ({ skip = 0, take = 20, userId, flowId }) => ({
        document: GET_FLOW_ASSIGNMENTS_QUERY,
        variables: { skip, take, userId, flowId },
      }),
      transformResponse: (response: { getFlowAssignments: FlowAssignmentDto[] }) => response.getFlowAssignments,
      providesTags: ['FlowAssignment'],
    }),

    getActiveAssignments: builder.query<FlowAssignmentDto[], UUID>({
      query: (userId) => ({
        document: GET_ACTIVE_ASSIGNMENTS_QUERY,
        variables: { userId },
      }),
      transformResponse: (response: { getActiveAssignments: FlowAssignmentDto[] }) => response.getActiveAssignments,
      providesTags: ['FlowAssignment'],
    }),

    getOverdueAssignments: builder.query<FlowAssignmentDto[], void>({
      query: () => ({
        document: GET_OVERDUE_ASSIGNMENTS_QUERY,
      }),
      transformResponse: (response: { getOverdueAssignments: FlowAssignmentDto[] }) => response.getOverdueAssignments,
      providesTags: ['FlowAssignment'],
    }),

    // User Mutations
    createUser: builder.mutation<UserDto, CreateUserInput>({
      query: (input) => ({
        document: CREATE_USER_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { createUser: UserDto }) => response.createUser,
      invalidatesTags: ['User'],
    }),

    updateUser: builder.mutation<UserDto, UpdateUserInput>({
      query: (input) => ({
        document: UPDATE_USER_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { updateUser: UserDto }) => response.updateUser,
      invalidatesTags: (_result, _error, { userId }) => [
        'User',
        { type: 'User', id: userId },
      ],
    }),

    // Flow Mutations
    createFlow: builder.mutation<FlowDto, CreateFlowInput>({
      query: (input) => ({
        document: CREATE_FLOW_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { createFlow: FlowDto }) => response.createFlow,
      invalidatesTags: ['Flow'],
    }),

    updateFlow: builder.mutation<FlowDto, UpdateFlowInput>({
      query: (input) => ({
        document: UPDATE_FLOW_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { updateFlow: { flow: FlowDto } }) => response.updateFlow.flow,
      invalidatesTags: (_result, _error, { id }) => [
        'Flow',
        { type: 'Flow', id },
      ],
    }),

    assignFlow: builder.mutation<AssignFlowResult, AssignFlowInput>({
      query: (input) => ({
        document: ASSIGN_FLOW_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { assignFlow: AssignFlowResult }) => response.assignFlow,
      invalidatesTags: ['FlowAssignment'],
    }),

    createFlowStep: builder.mutation<CreateFlowStepResult, CreateFlowStepInput>({
      query: (input) => ({
        document: CREATE_FLOW_STEP_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { createFlowStep: CreateFlowStepResult }) => response.createFlowStep,
      invalidatesTags: (_result, _error, { flowId }) => [
        'Flow',
        { type: 'Flow', id: flowId },
      ],
    }),

    createComponent: builder.mutation<CreateComponentResult, CreateComponentInput>({
      query: (input) => ({
        document: CREATE_COMPONENT_MUTATION,
        variables: { input },
      }),
      transformResponse: (response: { createComponent: CreateComponentResult }) => response.createComponent,
      invalidatesTags: ['Component', 'Flow'],
    }),
  }),
});

// Export hooks
export const {
  // User hooks
  useGetUsersQuery,
  useGetUserQuery,
  useCreateUserMutation,
  useUpdateUserMutation,

  // Flow hooks
  useGetFlowsQuery,
  useGetFlowQuery,
  useSearchFlowsQuery,
  useCreateFlowMutation,
  useUpdateFlowMutation,
  useCreateFlowStepMutation,
  useCreateComponentMutation,

  // Assignment hooks
  useGetFlowAssignmentsQuery,
  useGetActiveAssignmentsQuery,
  useGetOverdueAssignmentsQuery,
  useAssignFlowMutation,
} = graphqlApi; 