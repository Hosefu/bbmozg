const fs = require('fs');
const path = require('path');

function generateDetailedReport() {
  const results = global.testResults;
  const endTime = new Date();
  const duration = endTime - results.startTime;

  const html = `
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Lauf API E2E Test Report</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
        }
        
        .container {
            max-width: 1400px;
            margin: 0 auto;
            padding: 20px;
        }
        
        .header {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border-radius: 20px;
            padding: 30px;
            margin-bottom: 30px;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
            text-align: center;
        }
        
        .header h1 {
            color: #2c3e50;
            font-size: 2.5em;
            margin-bottom: 10px;
            background: linear-gradient(45deg, #667eea, #764ba2);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }
        
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .stat-card {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border-radius: 15px;
            padding: 25px;
            text-align: center;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
            transition: transform 0.3s ease;
        }
        
        .stat-card:hover {
            transform: translateY(-5px);
        }
        
        .stat-number {
            font-size: 2.5em;
            font-weight: bold;
            margin-bottom: 10px;
        }
        
        .stat-label {
            color: #666;
            font-size: 0.9em;
            text-transform: uppercase;
            letter-spacing: 1px;
        }
        
        .success { color: #27ae60; }
        .error { color: #e74c3c; }
        .info { color: #3498db; }
        .warning { color: #f39c12; }
        
        .section {
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(10px);
            border-radius: 15px;
            margin-bottom: 30px;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }
        
        .section-header {
            background: linear-gradient(45deg, #667eea, #764ba2);
            color: white;
            padding: 20px 30px;
            font-size: 1.3em;
            font-weight: bold;
        }
        
        .section-content {
            padding: 30px;
        }
        
        .api-call {
            background: #f8f9fa;
            border-radius: 10px;
            margin-bottom: 20px;
            border-left: 4px solid #667eea;
            overflow: hidden;
        }
        
        .api-call.success {
            border-left-color: #27ae60;
        }
        
        .api-call.error {
            border-left-color: #e74c3c;
        }
        
        .api-call-header {
            padding: 15px 20px;
            background: rgba(102, 126, 234, 0.1);
            border-bottom: 1px solid #eee;
            display: flex;
            justify-content: space-between;
            align-items: center;
            cursor: pointer;
        }
        
        .api-call.success .api-call-header {
            background: rgba(39, 174, 96, 0.1);
        }
        
        .api-call.error .api-call-header {
            background: rgba(231, 76, 60, 0.1);
        }
        
        .api-call-title {
            font-weight: bold;
            font-size: 1.1em;
        }
        
        .api-call-meta {
            display: flex;
            gap: 15px;
            font-size: 0.9em;
            color: #666;
        }
        
        .status-badge {
            padding: 4px 12px;
            border-radius: 20px;
            font-size: 0.8em;
            font-weight: bold;
            text-transform: uppercase;
        }
        
        .status-success {
            background: #d4edda;
            color: #155724;
        }
        
        .status-error {
            background: #f8d7da;
            color: #721c24;
        }
        
        .api-call-details {
            padding: 20px;
            display: none;
        }
        
        .api-call-details.show {
            display: block;
        }
        
        .code-block {
            background: #2d3748;
            color: #e2e8f0;
            padding: 15px;
            border-radius: 8px;
            font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
            font-size: 0.9em;
            overflow-x: auto;
            margin: 10px 0;
        }
        
        .tabs {
            display: flex;
            background: #f8f9fa;
            border-radius: 8px 8px 0 0;
            overflow: hidden;
        }
        
        .tab {
            padding: 12px 20px;
            background: #e9ecef;
            border: none;
            cursor: pointer;
            font-weight: bold;
            transition: background 0.3s ease;
        }
        
        .tab.active {
            background: #667eea;
            color: white;
        }
        
        .tab-content {
            display: none;
        }
        
        .tab-content.active {
            display: block;
        }
        
        .summary-table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        
        .summary-table th,
        .summary-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid #eee;
        }
        
        .summary-table th {
            background: #f8f9fa;
            font-weight: bold;
        }
        
        .progress-bar {
            width: 100%;
            height: 8px;
            background: #e9ecef;
            border-radius: 4px;
            overflow: hidden;
        }
        
        .progress-fill {
            height: 100%;
            background: linear-gradient(90deg, #27ae60, #2ecc71);
            transition: width 0.3s ease;
        }
        
        @media (max-width: 768px) {
            .stats-grid {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .api-call-meta {
                flex-direction: column;
                gap: 5px;
            }
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>🚀 Lauf API E2E Test Report</h1>
            <p><strong>Тестирование выполнено:</strong> ${endTime.toLocaleString('ru-RU')}</p>
            <p><strong>Продолжительность:</strong> ${Math.round(duration / 1000)} секунд</p>
        </div>

        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-number success">${results.stats.passed}</div>
                <div class="stat-label">Успешно</div>
                <div class="progress-bar">
                    <div class="progress-fill" style="width: ${(results.stats.passed / results.stats.total * 100)}%"></div>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-number error">${results.stats.failed}</div>
                <div class="stat-label">Ошибок</div>
            </div>
            <div class="stat-card">
                <div class="stat-number info">${results.stats.total}</div>
                <div class="stat-label">Всего запросов</div>
            </div>
            <div class="stat-card">
                <div class="stat-number warning">${Math.round(results.stats.total / (duration / 1000))}</div>
                <div class="stat-label">Запросов/сек</div>
            </div>
        </div>

        <div class="section">
            <div class="section-header">📊 Статистика по типам операций</div>
            <div class="section-content">
                <table class="summary-table">
                    <thead>
                        <tr>
                            <th>Тип операции</th>
                            <th>Всего</th>
                            <th>Успешно</th>
                            <th>Ошибок</th>
                            <th>Успешность</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><strong>GraphQL Мутации</strong></td>
                            <td>${results.stats.mutations.total}</td>
                            <td class="success">${results.stats.mutations.passed}</td>
                            <td class="error">${results.stats.mutations.failed}</td>
                            <td>${results.stats.mutations.total > 0 ? Math.round(results.stats.mutations.passed / results.stats.mutations.total * 100) : 0}%</td>
                        </tr>
                        <tr>
                            <td><strong>GraphQL Запросы</strong></td>
                            <td>${results.stats.queries.total}</td>
                            <td class="success">${results.stats.queries.passed}</td>
                            <td class="error">${results.stats.queries.failed}</td>
                            <td>${results.stats.queries.total > 0 ? Math.round(results.stats.queries.passed / results.stats.queries.total * 100) : 0}%</td>
                        </tr>
                        <tr>
                            <td><strong>REST API</strong></td>
                            <td>${results.stats.rest.total}</td>
                            <td class="success">${results.stats.rest.passed}</td>
                            <td class="error">${results.stats.rest.failed}</td>
                            <td>${results.stats.rest.total > 0 ? Math.round(results.stats.rest.passed / results.stats.rest.total * 100) : 0}%</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        ${generateApiCallsSection('🔄 GraphQL Мутации', results.mutations)}
        ${generateApiCallsSection('📋 GraphQL Запросы', results.queries)}
        ${generateApiCallsSection('🌐 REST API Запросы', results.restEndpoints)}
    </div>

    <script>
        // Toggle API call details
        document.querySelectorAll('.api-call-header').forEach(header => {
            header.addEventListener('click', () => {
                const details = header.nextElementSibling;
                details.classList.toggle('show');
                
                const arrow = header.querySelector('.arrow');
                if (arrow) {
                    arrow.textContent = details.classList.contains('show') ? '▼' : '▶';
                }
            });
        });

        // Tab functionality
        document.querySelectorAll('.tab').forEach(tab => {
            tab.addEventListener('click', () => {
                const tabGroup = tab.closest('.tabs').parentElement;
                const targetId = tab.dataset.tab;
                
                // Remove active class from all tabs and contents
                tabGroup.querySelectorAll('.tab').forEach(t => t.classList.remove('active'));
                tabGroup.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));
                
                // Add active class to clicked tab and corresponding content
                tab.classList.add('active');
                tabGroup.querySelector(\`[id="\${targetId}"]\`).classList.add('active');
            });
        });
    </script>
</body>
</html>
  `;

  const reportPath = path.join(__dirname, 'reports', 'detailed-api-report.html');
  fs.writeFileSync(reportPath, html);
  
  console.log(`📊 Detailed API report generated: ${reportPath}`);
  return reportPath;
}

function generateApiCallsSection(title, calls) {
  if (!calls || calls.length === 0) {
    return `
      <div class="section">
          <div class="section-header">${title}</div>
          <div class="section-content">
              <p>Нет данных для отображения</p>
          </div>
      </div>
    `;
  }

  const callsHtml = calls.map((call, index) => {
    const statusClass = call.success ? 'success' : 'error';
    const statusBadge = call.success ? 'status-success' : 'status-error';
    const statusText = call.success ? 'Успешно' : 'Ошибка';
    
    return `
      <div class="api-call ${statusClass}">
          <div class="api-call-header">
              <div class="api-call-title">${call.operation}</div>
              <div class="api-call-meta">
                  <span class="status-badge ${statusBadge}">${statusText}</span>
                  <span>${call.duration}ms</span>
                  <span>${new Date(call.timestamp).toLocaleTimeString('ru-RU')}</span>
                  <span class="arrow">▶</span>
              </div>
          </div>
          <div class="api-call-details">
              <div class="tabs">
                  <button class="tab active" data-tab="request-${index}">Запрос</button>
                  <button class="tab" data-tab="response-${index}">Ответ</button>
                  ${call.error ? `<button class="tab" data-tab="error-${index}">Ошибка</button>` : ''}
              </div>
              <div class="tab-content active" id="request-${index}">
                  <div class="code-block">${escapeHtml(call.request)}</div>
              </div>
              <div class="tab-content" id="response-${index}">
                  <div class="code-block">${call.response ? escapeHtml(call.response) : 'Нет ответа'}</div>
              </div>
              ${call.error ? `
                  <div class="tab-content" id="error-${index}">
                      <div class="code-block error">${escapeHtml(call.error)}</div>
                  </div>
              ` : ''}
          </div>
      </div>
    `;
  }).join('');

  return `
    <div class="section">
        <div class="section-header">${title} (${calls.length})</div>
        <div class="section-content">
            ${callsHtml}
        </div>
    </div>
  `;
}

function escapeHtml(text) {
  if (!text) return '';
  return text
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;');
}

// Export for Jest afterAll hook
if (typeof module !== 'undefined' && module.exports) {
  module.exports = generateDetailedReport;
}

// Run if called directly
if (require.main === module) {
  generateDetailedReport();
}