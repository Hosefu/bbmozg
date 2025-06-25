const fs = require('fs');
const path = require('path');

// Global test configuration
global.API_BASE_URL = 'http://localhost:8087';
global.GRAPHQL_ENDPOINT = `${global.API_BASE_URL}/graphql`;

// Test results storage
global.testResults = {
  mutations: [],
  queries: [],
  restEndpoints: [],
  startTime: new Date(),
  stats: {
    total: 0,
    passed: 0,
    failed: 0,
    mutations: { total: 0, passed: 0, failed: 0 },
    queries: { total: 0, passed: 0, failed: 0 },
    rest: { total: 0, passed: 0, failed: 0 }
  }
};

// Ensure reports directory exists
const reportsDir = path.join(__dirname, 'reports');
if (!fs.existsSync(reportsDir)) {
  fs.mkdirSync(reportsDir, { recursive: true });
}

// Global helper functions
global.logApiCall = (type, operation, request, response, duration, error = null) => {
  const result = {
    type,
    operation,
    request: JSON.stringify(request, null, 2),
    response: response ? JSON.stringify(response, null, 2) : null,
    duration,
    timestamp: new Date().toISOString(),
    success: !error,
    error: error ? error.message : null,
    statusCode: response?.status || (error ? 500 : 200)
  };

  if (type === 'mutation') {
    global.testResults.mutations.push(result);
    global.testResults.stats.mutations.total++;
    if (result.success) global.testResults.stats.mutations.passed++;
    else global.testResults.stats.mutations.failed++;
  } else if (type === 'query') {
    global.testResults.queries.push(result);
    global.testResults.stats.queries.total++;
    if (result.success) global.testResults.stats.queries.passed++;
    else global.testResults.stats.queries.failed++;
  } else if (type === 'rest') {
    global.testResults.restEndpoints.push(result);
    global.testResults.stats.rest.total++;
    if (result.success) global.testResults.stats.rest.passed++;
    else global.testResults.stats.rest.failed++;
  }

  global.testResults.stats.total++;
  if (result.success) global.testResults.stats.passed++;
  else global.testResults.stats.failed++;
};

console.log('🚀 Lauf API E2E Test Suite Setup Complete');
console.log(`📡 API Base URL: ${global.API_BASE_URL}`);
console.log(`🎯 GraphQL Endpoint: ${global.GRAPHQL_ENDPOINT}`);