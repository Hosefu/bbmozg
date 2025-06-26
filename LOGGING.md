# 📊 Система логирования ЛАУФ

## Обзор

Система логирования настроена для детального отслеживания всех компонентов приложения с использованием эмодзи для быстрого визуального различения типов логов.

## 🔍 Компоненты логирования

### 1. HTTP запросы (RequestLoggingMiddleware)
```
📡 [REQ-a1b2c3d4] REST POST /api/users | IP: 127.0.0.1 | UA: curl/7.68.0
✅ [REQ-a1b2c3d4] SUCCESS 201 | 45ms | 1234 bytes | application/json
```

**Иконки запросов:**
- 📡 REST API
- 🔸 GraphQL 
- ⚡ SignalR
- 💚 Health checks
- ⚙️ Hangfire
- 📄 Статические файлы

**Иконки ответов:**
- ✅ 2xx Success
- ↩️ 3xx Redirect  
- ⚠️ 4xx Client Error
- 🚨 5xx Server Error

### 2. MediatR команды и запросы (LoggingBehavior)
```
⚡ [MED-b2c3d4e5] START COMMAND: CreateUserCommand | Handler: CreateUserCommandHandler
✅ [MED-b2c3d4e5] SUCCESS COMMAND: CreateUserCommand за 123ms
```

**Типы операций:**
- ⚡ COMMAND (мутации данных)
- 🔍 QUERY (чтение данных)
- 📢 EVENT (доменные события)
- 🔄 REQUEST (прочие запросы)

### 3. SignalR хабы (BaseLoggingHub)
```
⚡ [HUB-Notification] CONNECT 12345 | User: user123 | IP: 127.0.0.1
⚡ [HUB-Notification] GROUP_JOIN 12345 -> User_456 | User: user123
⚡ [HUB-Notification] METHOD 12345 -> SubscribeToNotificationType | Params: {"type":"deadline"}
⚡ [HUB-Notification] SEND 12345 <- SubscriptionConfirmed | Data: "deadline"
```

### 4. Background Jobs (MemoryBackgroundJobService)
```
⚙️ [JOB-job_1_20240120123456] ENQUEUE DailyReminderJob.Execute | Queue size: 3
⚙️ [JOB-job_1_20240120123456] START DailyReminderJob.Execute | Queued for: 25ms
✅ [JOB-job_1_20240120123456] SUCCESS DailyReminderJob.Execute за 98ms
```

### 5. Специальные события
```
🐌 [REQ-a1b2c3d4] МЕДЛЕННЫЙ ЗАПРОС: GET /api/reports выполнялся 2500ms
🐌 [MED-b2c3d4e5] МЕДЛЕННАЯ ОПЕРАЦИЯ: GenerateReportQuery выполнялась 3000ms
🚨 [REQ-a1b2c3d4] ОШИБКА POST /api/users за 15ms
🚨 [JOB-job_2] FAILED за 156ms | Error: Database connection timeout
```

## ⚙️ Конфигурация

### Development (appsettings.Development.json)
- **Уровень:** Debug
- **Консоль:** Цветной вывод с эмодзи
- **Файлы:** logs/lauf-dev-.log (3 дня)

### Production (appsettings.json)  
- **Уровень:** Information
- **Консоль:** Структурированный вывод
- **Файлы:** logs/lauf-.log (7 дней)

## 🎯 Настройка уровней логирования

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Lauf.Api.Middleware.RequestLoggingMiddleware": "Information",
        "Lauf.Application.Behaviors.LoggingBehavior": "Information", 
        "Lauf.Infrastructure.ExternalServices.BackgroundJobs": "Information",
        "Lauf.Api.SignalR": "Information",
        "Microsoft": "Warning",
        "HotChocolate": "Warning"
      }
    }
  }
}
```

## 📈 Мониторинг производительности

Система автоматически отмечает медленные операции:
- **HTTP запросы** > 1000ms
- **MediatR операции** > 1000ms
- **Background Jobs** с таймингами

## 🔧 Использование в коде

### Middleware
Автоматически логирует все HTTP запросы. Настраивается в `Startup.cs`:

```csharp
app.UseMiddleware<RequestLoggingMiddleware>();
```

### MediatR 
Автоматически логирует команды/запросы через `LoggingBehavior`:

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

### SignalR
Наследуйте от `BaseLoggingHub`:

```csharp
public class MyHub : BaseLoggingHub
{
    public MyHub(ILogger<MyHub> logger) : base(logger) { }
    
    public async Task MyMethod(string param)
    {
        LogMethodCall(nameof(MyMethod), new { param });
        // ваш код
        LogClientCall("MyEvent", data: result);
        await Clients.All.SendAsync("MyEvent", result);
    }
}
```

### Background Jobs
Логирование встроено в `MemoryBackgroundJobService`.

## 🚀 Запуск с логированием

```bash
cd src/Lauf.Api
dotnet run

# Вы увидите:
[12:34:56.789] INF Запуск Lauf API приложения
[12:34:57.123] INF Сервер запущен на адресах: http://localhost:5000
[12:34:57.234] INF GraphQL Playground доступен по адресу: http://localhost:5000/playground
```

## 🎨 Примеры вывода

### Полный цикл обработки запроса:
```
[12:34:56.100] INF 📡 [REQ-a1b2c3d4] REST POST /api/users | IP: 127.0.0.1
[12:34:56.101] INF ⚡ [MED-b2c3d4e5] START COMMAND: CreateUserCommand | Handler: CreateUserCommandHandler  
[12:34:56.125] INF ✅ [MED-b2c3d4e5] SUCCESS COMMAND: CreateUserCommand за 24ms
[12:34:56.126] INF ✅ [REQ-a1b2c3d4] SUCCESS 201 | 26ms | 145 bytes | application/json
```

### SignalR активность:
```
[12:35:01.234] INF ⚡ [HUB-Notification] CONNECT abc123 | User: john_doe | IP: 192.168.1.10
[12:35:01.235] INF ⚡ [HUB-Notification] GROUP_JOIN abc123 -> User_456 | User: john_doe
[12:35:05.678] INF ⚡ [HUB-Notification] METHOD abc123 -> MarkNotificationAsRead
[12:35:05.690] INF ⚡ [HUB-Notification] SEND abc123 <- NotificationMarkedAsRead
```

### Background Jobs:
```
[12:36:00.000] INF ⚙️ [JOB-job_1_20240120123600] ENQUEUE DailyReminderJob.Execute | Queue size: 1
[12:36:00.010] INF ⚙️ [JOB-job_1_20240120123600] START DailyReminderJob.Execute | Queued for: 10ms
[12:36:00.156] INF ✅ [JOB-job_1_20240120123600] SUCCESS DailyReminderJob.Execute за 146ms
```

## 🔍 Фильтрация логов

Для фокуса на конкретных компонентах используйте grep:

```bash
# Только HTTP запросы
dotnet run | grep "REQ-"

# Только MediatR операции  
dotnet run | grep "MED-"

# Только медленные операции
dotnet run | grep "🐌"

# Только ошибки
dotnet run | grep "🚨"

# SignalR активность
dotnet run | grep "HUB-"

# Background Jobs
dotnet run | grep "JOB-"
```

Теперь у вас есть полная видимость всех процессов в приложении! 🚀 