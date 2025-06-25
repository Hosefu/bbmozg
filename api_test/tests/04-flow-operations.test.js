const LaufGraphQLClient = require('../lib/graphql-client');

describe('🔄 Flow Operations (CRUD)', () => {
  let client;
  let testFlowId;
  const testFlowTitle = `Test Flow E2E ${Date.now()}`;

  beforeAll(() => {
    client = new LaufGraphQLClient(global.GRAPHQL_ENDPOINT);
  });

  describe('Create Flow', () => {
    test('should create a new flow successfully', async () => {
      const createFlowMutation = `
        mutation CreateFlow($input: CreateFlowInput!) {
          createFlow(input: $input) {
            id
            title
            status
          }
        }
      `;

      const variables = {
        input: {
          title: testFlowTitle,
          description: "Comprehensive E2E test flow for API validation",
          isSequential: true,
          allowRetry: true,
          timeLimit: 30,
          passingScore: 80
        }
      };

      const response = await client.executeMutation(createFlowMutation, variables, 'Create Flow');
      
      expect(response.createFlow).toBeDefined();
      expect(response.createFlow.id).toBeDefined();
      expect(response.createFlow.title).toBe(testFlowTitle);
      expect(response.createFlow.status).toBe('Draft');
      
      testFlowId = response.createFlow.id;
    });

    test('should validate required fields on flow creation', async () => {
      const createFlowMutation = `
        mutation CreateFlow($input: CreateFlowInput!) {
          createFlow(input: $input) {
            id
          }
        }
      `;

      const variables = {
        input: {
          title: "",
          description: ""
        }
      };

      try {
        await client.executeMutation(createFlowMutation, variables, 'Create Invalid Flow');
        fail('Should have thrown validation error');
      } catch (error) {
        expect(error.message).toBeDefined();
      }
    });
  });

  describe('Read Flows', () => {
    test('should fetch all flows', async () => {
      const getFlowsQuery = `
        query GetFlows($skip: Int, $take: Int) {
          flows(skip: $skip, take: $take) {
            id
            title
            description
            status
            category
            priority
            isRequired
            createdAt
            updatedAt
          }
        }
      `;

      const variables = { skip: 0, take: 10 };
      const response = await client.executeQuery(getFlowsQuery, variables, 'Get All Flows');
      
      expect(response.flows).toBeDefined();
      expect(Array.isArray(response.flows)).toBe(true);
      
      // Check if our test flow is in the list
      const ourFlow = response.flows.find(flow => flow.title === testFlowTitle);
      expect(ourFlow).toBeDefined();
    });

    test('should fetch flow by ID', async () => {
      const getFlowQuery = `
        query GetFlow($id: UUID!) {
          flow(id: $id) {
            id
            title
            description
            status
            createdAt
            updatedAt
          }
        }
      `;

      const variables = { id: testFlowId };
      const response = await client.executeQuery(getFlowQuery, variables, 'Get Flow By ID');
      
      expect(response.flow).toBeDefined();
      expect(response.flow.id).toBe(testFlowId);
      expect(response.flow.title).toBe(testFlowTitle);
    });

    test('should search flows by title', async () => {
      const searchFlowsQuery = `
        query SearchFlows($searchTerm: String!, $skip: Int, $take: Int) {
          searchFlows(searchTerm: $searchTerm, skip: $skip, take: $take) {
            id
            title
            description
            status
          }
        }
      `;

      const variables = { 
        searchTerm: "E2E",
        skip: 0, 
        take: 10 
      };
      
      const response = await client.executeQuery(searchFlowsQuery, variables, 'Search Flows');
      
      expect(response.searchFlows).toBeDefined();
      expect(Array.isArray(response.searchFlows)).toBe(true);
      
      // Should find our test flow
      const foundFlow = response.searchFlows.find(flow => flow.title === testFlowTitle);
      expect(foundFlow).toBeDefined();
    });
  });

  describe('Update Flow', () => {
    test('should update flow successfully', async () => {
      const updateFlowMutation = `
        mutation UpdateFlow($input: UpdateFlowInput!) {
          updateFlow(input: $input) {
            id
            title
            description
            status
          }
        }
      `;

      const updatedTitle = `${testFlowTitle} - Updated`;
      const variables = {
        input: {
          id: testFlowId,
          title: updatedTitle,
          description: "Updated description for E2E testing",
          status: "Draft"
        }
      };

      const response = await client.executeMutation(updateFlowMutation, variables, 'Update Flow');
      
      expect(response.updateFlow).toBeDefined();
      expect(response.updateFlow.id).toBe(testFlowId);
      expect(response.updateFlow.title).toBe(updatedTitle);
      expect(response.updateFlow.description).toBe("Updated description for E2E testing");
    });
  });

  describe('Flow Categories and Tags', () => {
    test('should handle flow with categories and tags', async () => {
      const updateFlowMutation = `
        mutation UpdateFlow($input: UpdateFlowInput!) {
          updateFlow(input: $input) {
            id
            title
            category
            tags
          }
        }
      `;

      const variables = {
        input: {
          id: testFlowId,
          category: "Testing",
          tags: "e2e,automation,api"
        }
      };

      const response = await client.executeMutation(updateFlowMutation, variables, 'Update Flow with Categories');
      
      expect(response.updateFlow).toBeDefined();
      expect(response.updateFlow.category).toBe("Testing");
      expect(response.updateFlow.tags).toBeDefined();
    });
  });
});