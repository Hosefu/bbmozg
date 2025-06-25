const { GraphQLClient } = require('graphql-request');

class LaufGraphQLClient {
  constructor(endpoint) {
    this.client = new GraphQLClient(endpoint, {
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });
    this.endpoint = endpoint;
  }

  async executeQuery(query, variables = {}, operationName = 'Unknown Query') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.request(query, variables);
      const duration = Date.now() - startTime;
      
      global.logApiCall('query', operationName, { query, variables }, response, duration);
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('query', operationName, { query, variables }, null, duration, err);
      throw err;
    }
  }

  async executeMutation(mutation, variables = {}, operationName = 'Unknown Mutation') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.request(mutation, variables);
      const duration = Date.now() - startTime;
      
      global.logApiCall('mutation', operationName, { mutation, variables }, response, duration);
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('mutation', operationName, { mutation, variables }, null, duration, err);
      throw err;
    }
  }

  // Introspection query to get schema
  async getSchema() {
    const introspectionQuery = `
      query IntrospectionQuery {
        __schema {
          queryType { name }
          mutationType { name }
          types {
            name
            kind
            description
            fields {
              name
              description
              type {
                name
                kind
                ofType {
                  name
                  kind
                }
              }
            }
          }
        }
      }
    `;

    return await this.executeQuery(introspectionQuery, {}, 'Schema Introspection');
  }
}

module.exports = LaufGraphQLClient;