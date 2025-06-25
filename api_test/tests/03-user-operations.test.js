const LaufGraphQLClient = require('../lib/graphql-client');
const { v4: uuidv4 } = require('uuid');

describe('👤 User Operations (CRUD)', () => {
  let client;
  let testUserId;
  const testUserEmail = `test-${Date.now()}@example.com`;

  beforeAll(() => {
    client = new LaufGraphQLClient(global.GRAPHQL_ENDPOINT);
  });

  describe('Create User', () => {
    test('should create a new user successfully', async () => {
      const createUserMutation = `
        mutation CreateUser($input: CreateUserInput!) {
          createUser(input: $input) {
            id
            email
            firstName
            lastName
            position
            language
            isActive
            createdAt
          }
        }
      `;

      const variables = {
        input: {
          telegramId: Math.floor(Math.random() * 1000000),
          email: testUserEmail,
          fullName: "Test User E2E",
          position: "QA Engineer"
        }
      };

      const response = await client.executeMutation(createUserMutation, variables, 'Create User');
      
      expect(response.createUser).toBeDefined();
      expect(response.createUser.id).toBeDefined();
      expect(response.createUser.email).toBe(testUserEmail);
      expect(response.createUser.firstName).toBe("Test");
      expect(response.createUser.lastName).toBe("User E2E");
      expect(response.createUser.position).toBe("QA Engineer");
      expect(response.createUser.isActive).toBe(true);
      
      testUserId = response.createUser.id;
    });

    test('should fail to create user with duplicate telegram ID', async () => {
      const createUserMutation = `
        mutation CreateUser($input: CreateUserInput!) {
          createUser(input: $input) {
            id
            email
          }
        }
      `;

      const variables = {
        input: {
          telegramId: 123456, // Using same ID as before
          email: `duplicate-${Date.now()}@example.com`,
          fullName: "Duplicate User",
          position: "Test"
        }
      };

      try {
        await client.executeMutation(createUserMutation, variables, 'Create Duplicate User');
        fail('Should have thrown an error for duplicate telegram ID');
      } catch (error) {
        expect(error.message).toContain('уже существует');
      }
    });
  });

  describe('Read Users', () => {
    test('should fetch all users', async () => {
      const getUsersQuery = `
        query GetUsers($skip: Int, $take: Int) {
          users(skip: $skip, take: $take) {
            id
            email
            firstName
            lastName
            position
            isActive
            createdAt
          }
        }
      `;

      const variables = { skip: 0, take: 10 };
      const response = await client.executeQuery(getUsersQuery, variables, 'Get All Users');
      
      expect(response.users).toBeDefined();
      expect(Array.isArray(response.users)).toBe(true);
      expect(response.users.length).toBeGreaterThan(0);
      
      // Check if our test user is in the list
      const ourUser = response.users.find(user => user.email === testUserEmail);
      expect(ourUser).toBeDefined();
    });

    test('should fetch user by ID', async () => {
      const getUserQuery = `
        query GetUser($id: UUID!) {
          user(id: $id) {
            id
            email
            firstName
            lastName
            position
            isActive
            createdAt
          }
        }
      `;

      const variables = { id: testUserId };
      const response = await client.executeQuery(getUserQuery, variables, 'Get User By ID');
      
      expect(response.user).toBeDefined();
      expect(response.user.id).toBe(testUserId);
      expect(response.user.email).toBe(testUserEmail);
    });
  });

  describe('Update User', () => {
    test('should update user successfully', async () => {
      const updateUserMutation = `
        mutation UpdateUser($input: UpdateUserInput!) {
          updateUser(input: $input) {
            id
            email
            fullName
            position
            isActive
          }
        }
      `;

      const variables = {
        input: {
          id: testUserId,
          email: testUserEmail,
          fullName: "Updated Test User",
          position: "Senior QA Engineer",
          isActive: true
        }
      };

      const response = await client.executeMutation(updateUserMutation, variables, 'Update User');
      
      expect(response.updateUser).toBeDefined();
      expect(response.updateUser.id).toBe(testUserId);
      expect(response.updateUser.fullName).toBe("Updated Test User");
      expect(response.updateUser.position).toBe("Senior QA Engineer");
    });
  });

  describe('User Validation', () => {
    test('should validate required fields on user creation', async () => {
      const createUserMutation = `
        mutation CreateUser($input: CreateUserInput!) {
          createUser(input: $input) {
            id
          }
        }
      `;

      const variables = {
        input: {
          telegramId: null,
          email: "",
          fullName: "",
          position: ""
        }
      };

      try {
        await client.executeMutation(createUserMutation, variables, 'Create Invalid User');
        fail('Should have thrown validation error');
      } catch (error) {
        expect(error.message).toBeDefined();
      }
    });
  });
});