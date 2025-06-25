const axios = require('axios');

class LaufRestClient {
  constructor(baseURL) {
    this.client = axios.create({
      baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });
  }

  async get(endpoint, params = {}, operationName = 'GET Request') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.get(endpoint, { params });
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (GET ${endpoint})`, 
        { method: 'GET', endpoint, params }, 
        { status: response.status, data: response.data }, 
        duration);
        
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (GET ${endpoint})`, 
        { method: 'GET', endpoint, params }, 
        null, 
        duration, 
        err);
        
      throw err;
    }
  }

  async post(endpoint, data = {}, operationName = 'POST Request') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.post(endpoint, data);
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (POST ${endpoint})`, 
        { method: 'POST', endpoint, data }, 
        { status: response.status, data: response.data }, 
        duration);
        
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (POST ${endpoint})`, 
        { method: 'POST', endpoint, data }, 
        null, 
        duration, 
        err);
        
      throw err;
    }
  }

  async put(endpoint, data = {}, operationName = 'PUT Request') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.put(endpoint, data);
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (PUT ${endpoint})`, 
        { method: 'PUT', endpoint, data }, 
        { status: response.status, data: response.data }, 
        duration);
        
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (PUT ${endpoint})`, 
        { method: 'PUT', endpoint, data }, 
        null, 
        duration, 
        err);
        
      throw err;
    }
  }

  async delete(endpoint, operationName = 'DELETE Request') {
    const startTime = Date.now();
    let response = null;
    let error = null;

    try {
      response = await this.client.delete(endpoint);
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (DELETE ${endpoint})`, 
        { method: 'DELETE', endpoint }, 
        { status: response.status, data: response.data }, 
        duration);
        
      return response;
    } catch (err) {
      error = err;
      const duration = Date.now() - startTime;
      
      global.logApiCall('rest', `${operationName} (DELETE ${endpoint})`, 
        { method: 'DELETE', endpoint }, 
        null, 
        duration, 
        err);
        
      throw err;
    }
  }
}

module.exports = LaufRestClient;