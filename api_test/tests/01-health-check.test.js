const LaufRestClient = require('../lib/rest-client');

describe('🏥 Health Check & Basic Connectivity', () => {
  let restClient;

  beforeAll(() => {
    restClient = new LaufRestClient(global.API_BASE_URL);
  });

  test('API server should be accessible', async () => {
    const response = await restClient.get('/health', {}, 'Health Check');
    
    expect(response.status).toBe(200);
    expect(response.data).toBeDefined();
  });

  test('GraphQL endpoint should be accessible', async () => {
    const response = await restClient.post('/graphql', {
      query: '{ __typename }'
    }, 'GraphQL Connectivity Check');
    
    expect(response.status).toBe(200);
    expect(response.data).toBeDefined();
  });

  test('API should return proper headers', async () => {
    const response = await restClient.get('/health', {}, 'Headers Check');
    
    expect(response.headers).toBeDefined();
    console.log('Actual headers:', response.headers);
    console.log('Content-Type:', response.headers['content-type']);
    expect(response.headers['content-type']).toContain('application/json');
  });
});