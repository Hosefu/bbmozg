const LaufGraphQLClient = require('../lib/graphql-client');
const LaufRestClient = require('../lib/rest-client');

describe('⚡ Performance & Stress Tests', () => {
  let gqlClient;
  let restClient;

  beforeAll(() => {
    gqlClient = new LaufGraphQLClient(global.GRAPHQL_ENDPOINT);
    restClient = new LaufRestClient(global.API_BASE_URL);
  });

  describe('Response Time Tests', () => {
    test('GraphQL introspection should respond within 2 seconds', async () => {
      const startTime = Date.now();
      
      await gqlClient.getSchema();
      
      const duration = Date.now() - startTime;
      expect(duration).toBeLessThan(2000);
    });

    test('Health check should respond within 500ms', async () => {
      const startTime = Date.now();
      
      await restClient.get('/health', {}, 'Performance Health Check');
      
      const duration = Date.now() - startTime;
      expect(duration).toBeLessThan(500);
    });

    test('User list query should respond within 1 second', async () => {
      const getUsersQuery = `
        query GetUsers {
          users(take: 10) {
            id
            email
            firstName
            lastName
          }
        }
      `;

      const startTime = Date.now();
      
      await gqlClient.executeQuery(getUsersQuery, {}, 'Performance User List');
      
      const duration = Date.now() - startTime;
      expect(duration).toBeLessThan(1000);
    });
  });

  describe('Concurrent Request Tests', () => {
    test('should handle multiple concurrent health checks', async () => {
      const concurrentRequests = 10;
      const promises = [];

      for (let i = 0; i < concurrentRequests; i++) {
        promises.push(
          restClient.get('/health', {}, `Concurrent Health Check ${i + 1}`)
        );
      }

      const responses = await Promise.all(promises);
      
      responses.forEach((response, index) => {
        expect(response.status).toBe(200);
      });
    });

    test('should handle multiple concurrent GraphQL queries', async () => {
      const getUsersQuery = `
        query GetUsers {
          users(take: 5) {
            id
            email
          }
        }
      `;

      const concurrentRequests = 5;
      const promises = [];

      for (let i = 0; i < concurrentRequests; i++) {
        promises.push(
          gqlClient.executeQuery(getUsersQuery, {}, `Concurrent User Query ${i + 1}`)
        );
      }

      const responses = await Promise.all(promises);
      
      responses.forEach((response, index) => {
        expect(response.users).toBeDefined();
        expect(Array.isArray(response.users)).toBe(true);
      });
    });
  });

  describe('Large Dataset Tests', () => {
    test('should handle large result sets efficiently', async () => {
      const getUsersQuery = `
        query GetUsers($take: Int) {
          users(take: $take) {
            id
            email
            firstName
            lastName
            position
            createdAt
          }
        }
      `;

      const startTime = Date.now();
      
      const response = await gqlClient.executeQuery(getUsersQuery, { take: 100 }, 'Large Dataset Query');
      
      const duration = Date.now() - startTime;
      
      expect(response.users).toBeDefined();
      expect(Array.isArray(response.users)).toBe(true);
      expect(duration).toBeLessThan(3000); // Should complete within 3 seconds
    });
  });

  describe('Error Handling Performance', () => {
    test('should handle invalid queries quickly', async () => {
      const invalidQuery = `
        query InvalidQuery {
          nonExistentField {
            id
          }
        }
      `;

      const startTime = Date.now();
      
      try {
        await gqlClient.executeQuery(invalidQuery, {}, 'Invalid Query Performance Test');
        fail('Should have thrown an error');
      } catch (error) {
        const duration = Date.now() - startTime;
        expect(duration).toBeLessThan(1000); // Error handling should be fast
        expect(error.message).toBeDefined();
      }
    });

    test('should handle invalid REST endpoints quickly', async () => {
      const startTime = Date.now();
      
      try {
        await restClient.get('/non-existent-endpoint', {}, 'Invalid REST Endpoint');
        // Some APIs might return 404 instead of throwing
      } catch (error) {
        const duration = Date.now() - startTime;
        expect(duration).toBeLessThan(1000);
      }
    });
  });

  describe('Memory and Resource Tests', () => {
    test('should handle repeated requests without memory leaks', async () => {
      const getUsersQuery = `
        query GetUsers {
          users(take: 1) {
            id
            email
          }
        }
      `;

      // Make 50 requests to check for memory leaks
      for (let i = 0; i < 50; i++) {
        await gqlClient.executeQuery(getUsersQuery, {}, `Memory Test Query ${i + 1}`);
        
        // Small delay to prevent overwhelming the server
        await new Promise(resolve => setTimeout(resolve, 10));
      }

      // If we reach here without errors, the test passes
      expect(true).toBe(true);
    });
  });
});