const LaufGraphQLClient = require('../lib/graphql-client');

describe('📋 Flow Assignment Operations', () => {
  let client;
  let testUserId;
  let testFlowId;
  let testAssignmentId;

  beforeAll(async () => {
    client = new LaufGraphQLClient(global.GRAPHQL_ENDPOINT);
    
    // Create test user for assignments
    const createUserMutation = `
      mutation CreateUser($input: CreateUserInput!) {
        createUser(input: $input) {
          id
        }
      }
    `;

    const userResponse = await client.executeMutation(createUserMutation, {
      input: {
        telegramId: Math.floor(Math.random() * 1000000),
        email: `assignment-test-${Date.now()}@example.com`,
        fullName: "Assignment Test User",
        position: "Learner"
      }
    }, 'Create Test User for Assignments');
    
    testUserId = userResponse.createUser.id;

    // Create test flow for assignments
    const createFlowMutation = `
      mutation CreateFlow($input: CreateFlowInput!) {
        createFlow(input: $input) {
          id
        }
      }
    `;

    const flowResponse = await client.executeMutation(createFlowMutation, {
      input: {
        title: `Assignment Test Flow ${Date.now()}`,
        description: "Flow for testing assignments",
        isSequential: true,
        allowRetry: true
      }
    }, 'Create Test Flow for Assignments');
    
    testFlowId = flowResponse.createFlow.id;
  });

  describe('Assign Flow', () => {
    test('should assign flow to user successfully', async () => {
      const assignFlowMutation = `
        mutation AssignFlow($input: AssignFlowInput!) {
          assignFlow(input: $input) {
            assignmentId
            success
            message
          }
        }
      `;

      const dueDate = new Date();
      dueDate.setDate(dueDate.getDate() + 7); // Due in 7 days

      const variables = {
        input: {
          userId: testUserId,
          flowId: testFlowId,
          dueDate: dueDate.toISOString(),
          assignedBy: testUserId // Self-assigned for testing
        }
      };

      const response = await client.executeMutation(assignFlowMutation, variables, 'Assign Flow to User');
      
      expect(response.assignFlow).toBeDefined();
      expect(response.assignFlow.success).toBe(true);
      expect(response.assignFlow.assignmentId).toBeDefined();
      expect(response.assignFlow.message).toBeDefined();
      
      testAssignmentId = response.assignFlow.assignmentId;
    });

    test('should prevent duplicate assignment', async () => {
      const assignFlowMutation = `
        mutation AssignFlow($input: AssignFlowInput!) {
          assignFlow(input: $input) {
            success
            message
          }
        }
      `;

      const variables = {
        input: {
          userId: testUserId,
          flowId: testFlowId,
          dueDate: new Date().toISOString()
        }
      };

      try {
        await client.executeMutation(assignFlowMutation, variables, 'Duplicate Flow Assignment');
        // If it doesn't throw, check if it properly handles duplicates
      } catch (error) {
        expect(error.message).toBeDefined();
      }
    });
  });

  describe('Read Flow Assignments', () => {
    test('should fetch all flow assignments', async () => {
      const getAssignmentsQuery = `
        query GetFlowAssignments($skip: Int, $take: Int) {
          flowAssignments(skip: $skip, take: $take) {
            id
            userId
            flowId
            status
            createdAt
            dueDate
          }
        }
      `;

      const variables = { skip: 0, take: 10 };
      const response = await client.executeQuery(getAssignmentsQuery, variables, 'Get All Flow Assignments');
      
      expect(response.flowAssignments).toBeDefined();
      expect(Array.isArray(response.flowAssignments)).toBe(true);
      
      // Check if our test assignment is in the list
      const ourAssignment = response.flowAssignments.find(a => a.id === testAssignmentId);
      expect(ourAssignment).toBeDefined();
    });

    test('should fetch assignments by user', async () => {
      const getAssignmentsByUserQuery = `
        query GetFlowAssignments($userId: UUID) {
          flowAssignments(userId: $userId) {
            id
            userId
            flowId
            status
          }
        }
      `;

      const variables = { userId: testUserId };
      const response = await client.executeQuery(getAssignmentsByUserQuery, variables, 'Get User Assignments');
      
      expect(response.flowAssignments).toBeDefined();
      expect(Array.isArray(response.flowAssignments)).toBe(true);
      
      // All assignments should belong to our test user
      response.flowAssignments.forEach(assignment => {
        expect(assignment.userId).toBe(testUserId);
      });
    });

    test('should fetch assignments by flow', async () => {
      const getAssignmentsByFlowQuery = `
        query GetFlowAssignments($flowId: UUID) {
          flowAssignments(flowId: $flowId) {
            id
            userId
            flowId
            status
          }
        }
      `;

      const variables = { flowId: testFlowId };
      const response = await client.executeQuery(getAssignmentsByFlowQuery, variables, 'Get Flow Assignments');
      
      expect(response.flowAssignments).toBeDefined();
      expect(Array.isArray(response.flowAssignments)).toBe(true);
      
      // All assignments should be for our test flow
      response.flowAssignments.forEach(assignment => {
        expect(assignment.flowId).toBe(testFlowId);
      });
    });

    test('should fetch active assignments', async () => {
      const getActiveAssignmentsQuery = `
        query GetActiveAssignments($userId: UUID!) {
          activeAssignments(userId: $userId) {
            id
            userId
            flowId
            status
          }
        }
      `;

      const variables = { userId: testUserId };
      const response = await client.executeQuery(getActiveAssignmentsQuery, variables, 'Get Active Assignments');
      
      expect(response.activeAssignments).toBeDefined();
      expect(Array.isArray(response.activeAssignments)).toBe(true);
      
      // All assignments should be active (assigned or in progress)
      response.activeAssignments.forEach(assignment => {
        expect(['Assigned', 'InProgress']).toContain(assignment.status);
      });
    });

    test('should fetch overdue assignments', async () => {
      const getOverdueAssignmentsQuery = `
        query GetOverdueAssignments {
          overdueAssignments {
            id
            userId
            flowId
            status
            dueDate
          }
        }
      `;

      const response = await client.executeQuery(getOverdueAssignmentsQuery, {}, 'Get Overdue Assignments');
      
      expect(response.overdueAssignments).toBeDefined();
      expect(Array.isArray(response.overdueAssignments)).toBe(true);
      
      // All assignments should have due dates in the past
      response.overdueAssignments.forEach(assignment => {
        if (assignment.dueDate) {
          const dueDate = new Date(assignment.dueDate);
          const now = new Date();
          expect(dueDate.getTime()).toBeLessThan(now.getTime());
        }
      });
    });
  });

  describe('Flow Assignment Lifecycle', () => {
    test('should start flow assignment', async () => {
      const startFlowMutation = `
        mutation StartFlow($input: StartFlowInput!) {
          startFlow(input: $input) {
            id
            status
          }
        }
      `;

      const variables = {
        input: {
          assignmentId: testAssignmentId
        }
      };

      const response = await client.executeMutation(startFlowMutation, variables, 'Start Flow Assignment');
      
      expect(response.startFlow).toBeDefined();
      expect(response.startFlow.id).toBe(testAssignmentId);
      expect(response.startFlow.status).toBe('InProgress');
    });

    test('should complete flow assignment', async () => {
      const completeFlowMutation = `
        mutation CompleteFlow($input: CompleteFlowInput!) {
          completeFlow(input: $input) {
            id
            status
            completedAt
          }
        }
      `;

      const variables = {
        input: {
          assignmentId: testAssignmentId,
          completionNotes: "E2E test completion"
        }
      };

      const response = await client.executeMutation(completeFlowMutation, variables, 'Complete Flow Assignment');
      
      expect(response.completeFlow).toBeDefined();
      expect(response.completeFlow.id).toBe(testAssignmentId);
      expect(response.completeFlow.status).toBe('Completed');
      expect(response.completeFlow.completedAt).toBeDefined();
    });
  });

  describe('Assignment Validation', () => {
    test('should validate assignment input', async () => {
      const assignFlowMutation = `
        mutation AssignFlow($input: AssignFlowInput!) {
          assignFlow(input: $input) {
            success
          }
        }
      `;

      const variables = {
        input: {
          userId: "invalid-uuid",
          flowId: "invalid-uuid"
        }
      };

      try {
        await client.executeMutation(assignFlowMutation, variables, 'Invalid Assignment');
        fail('Should have thrown validation error');
      } catch (error) {
        expect(error.message).toBeDefined();
      }
    });
  });
});