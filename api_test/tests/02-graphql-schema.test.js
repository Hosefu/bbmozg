const LaufGraphQLClient = require('../lib/graphql-client');

describe('📋 GraphQL Schema & Introspection', () => {
  let client;

  beforeAll(() => {
    client = new LaufGraphQLClient(global.GRAPHQL_ENDPOINT);
  });

  test('GraphQL schema should be introspectable', async () => {
    const schema = await client.getSchema();
    
    expect(schema).toBeDefined();
    expect(schema.__schema).toBeDefined();
    expect(schema.__schema.queryType).toBeDefined();
    expect(schema.__schema.mutationType).toBeDefined();
  });

  test('Schema should have expected query types', async () => {
    const schema = await client.getSchema();
    const types = schema.__schema.types;
    
    // Find Query type
    const queryType = types.find(type => type.name === 'Query');
    expect(queryType).toBeDefined();
    expect(queryType.fields).toBeDefined();
    
    // Check for expected query fields
    const queryFields = queryType.fields.map(field => field.name);
    expect(queryFields).toContain('users');
    expect(queryFields).toContain('flows');
    expect(queryFields).toContain('flowAssignments');
  });

  test('Schema should have expected mutation types', async () => {
    const schema = await client.getSchema();
    const types = schema.__schema.types;
    
    // Find Mutation type
    const mutationType = types.find(type => type.name === 'Mutation');
    expect(mutationType).toBeDefined();
    expect(mutationType.fields).toBeDefined();
    
    // Check for expected mutation fields
    const mutationFields = mutationType.fields.map(field => field.name);
    expect(mutationFields).toContain('createUser');
    expect(mutationFields).toContain('createFlow');
    expect(mutationFields).toContain('assignFlow');
  });

  test('Schema should have expected custom types', async () => {
    const schema = await client.getSchema();
    const types = schema.__schema.types;
    const typeNames = types.map(type => type.name);
    
    // Check for expected custom types
    expect(typeNames).toContain('UserDto');
    expect(typeNames).toContain('FlowDto');
    expect(typeNames).toContain('FlowAssignmentDto');
  });
});