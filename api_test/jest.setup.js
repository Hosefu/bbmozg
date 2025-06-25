// Jest setup file to run after all tests
const generateDetailedReport = require('./generate-report');

// This runs after all test suites complete
afterAll(async () => {
  // Wait a bit for all async operations to complete
  await new Promise(resolve => setTimeout(resolve, 1000));
  
  // Generate detailed report
  try {
    const reportPath = generateDetailedReport();
    console.log(`\n🎉 Test execution completed!`);
    console.log(`📊 Detailed report available at: ${reportPath}`);
    console.log(`\n📈 Summary:`);
    console.log(`   Total requests: ${global.testResults.stats.total}`);
    console.log(`   ✅ Successful: ${global.testResults.stats.passed}`);
    console.log(`   ❌ Failed: ${global.testResults.stats.failed}`);
    console.log(`   📊 Success rate: ${Math.round(global.testResults.stats.passed / global.testResults.stats.total * 100)}%`);
  } catch (error) {
    console.error('Error generating detailed report:', error);
  }
});