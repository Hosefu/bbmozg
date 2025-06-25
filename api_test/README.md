# 🚀 Lauf API E2E Test Suite

Comprehensive End-to-End testing suite for the Lauf API with detailed HTML reports and performance metrics.

## 🎯 Features

- **Complete API Coverage**: Tests all GraphQL mutations, queries, and REST endpoints
- **Beautiful HTML Reports**: Detailed reports with request/response details, timing, and statistics
- **Performance Testing**: Response time monitoring and stress testing
- **Real-time Logging**: Every API call is logged with full details
- **Comprehensive Statistics**: Success rates, performance metrics, and error analysis

## 📋 Test Coverage

### GraphQL Operations
- ✅ **Mutations**: createUser, updateUser, createFlow, updateFlow, assignFlow, startFlow, completeFlow
- ✅ **Queries**: users, flows, searchFlows, flowAssignments, activeAssignments, overdueAssignments
- ✅ **Schema Introspection**: Complete schema validation

### REST Endpoints
- ✅ **Health Check**: `/health`
- ✅ **GraphQL Endpoint**: `/graphql`

### Performance Tests
- ✅ **Response Time**: All endpoints under 2 seconds
- ✅ **Concurrent Requests**: Multiple simultaneous requests
- ✅ **Large Datasets**: Handling of large result sets
- ✅ **Error Handling**: Fast error response times

## 🚀 Quick Start

### Prerequisites
- Node.js 16+ installed
- Lauf API server running on http://localhost:5000

### Installation
\`\`\`bash
cd api_test
npm install
\`\`\`

### Run Tests
\`\`\`bash
# Run all tests with HTML report
npm start

# Run tests only
npm test

# Run with coverage
npm run test:coverage

# Watch mode for development
npm run test:watch
\`\`\`

## 📊 Reports

After running tests, you'll find reports in the `reports/` directory:

- **`detailed-api-report.html`** - Beautiful, comprehensive report with all API calls
- **`test-report.html`** - Jest HTML report with test results
- **Console output** - Real-time statistics and summary

## 📁 Project Structure

\`\`\`
api_test/
├── lib/                    # Client libraries
│   ├── graphql-client.js  # GraphQL client with logging
│   └── rest-client.js     # REST client with logging
├── tests/                 # Test suites
│   ├── 01-health-check.test.js
│   ├── 02-graphql-schema.test.js
│   ├── 03-user-operations.test.js
│   ├── 04-flow-operations.test.js
│   ├── 05-flow-assignment-operations.test.js
│   └── 06-performance-stress.test.js
├── reports/               # Generated reports
├── setup.js              # Global test setup
├── jest.setup.js         # Post-test report generation
├── generate-report.js    # HTML report generator
└── package.json          # Dependencies and scripts
\`\`\`

## 🔧 Configuration

### API Endpoints
Edit `setup.js` to change API endpoints:
\`\`\`javascript
global.API_BASE_URL = 'http://localhost:5000';
global.GRAPHQL_ENDPOINT = \`\${global.API_BASE_URL}/graphql\`;
\`\`\`

### Test Timeout
Modify `package.json` jest configuration:
\`\`\`json
"testTimeout": 30000
\`\`\`

## 📈 What Gets Tested

### User Operations
- Create user with validation
- Fetch all users
- Fetch user by ID
- Update user information
- Handle duplicate users
- Validate required fields

### Flow Operations
- Create learning flows
- Fetch all flows
- Search flows by title
- Update flow details
- Handle categories and tags
- Validate flow data

### Flow Assignments
- Assign flows to users
- Start flow assignments
- Complete assignments
- Fetch assignments by user/flow
- Get active/overdue assignments
- Assignment lifecycle management

### Performance & Reliability
- Response time monitoring
- Concurrent request handling
- Large dataset processing
- Error handling performance
- Memory leak detection

## 🎨 Report Features

The HTML reports include:
- **Real-time Statistics**: Success rates, response times
- **Interactive Details**: Click to expand request/response details
- **Performance Metrics**: Timing for every operation
- **Error Analysis**: Full error messages and stack traces
- **Visual Indicators**: Color-coded success/failure status
- **Responsive Design**: Works on all devices

## 🔍 Troubleshooting

### API Not Accessible
1. Ensure Lauf API is running on http://localhost:5000
2. Check database is connected (PostgreSQL)
3. Verify GraphQL playground at http://localhost:5000/playground

### Test Failures
1. Check API logs for errors
2. Review detailed HTML report for specific failures
3. Ensure test data doesn't conflict with existing data

### Performance Issues
1. Check system resources
2. Review database performance
3. Monitor network latency

## 🤝 Contributing

To add new tests:
1. Create new test file in `tests/` directory
2. Use the provided GraphQL and REST clients
3. Follow existing naming conventions
4. Update this README with new test coverage

## 📝 License

Part of the Lauf project - Internal testing suite.