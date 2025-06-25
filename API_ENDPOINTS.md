# 🔗 API Endpoints - Lauf System

## 📋 Обзор новых endpoint'ов

Документация по API endpoints, добавленным в систему Lauf для интеграции с Telegram Bot и мониторинга фоновых задач.

---

## 🤖 Telegram Bot API

### Webhook для сообщений от Telegram

**Endpoint**: `POST /api/telegram/webhook`  
**Описание**: Основной webhook для обработки всех сообщений и команд от Telegram Bot  
**Авторизация**: Не требуется (AllowAnonymous)

**Пример запроса**:
```json
POST /api/telegram/webhook
Content-Type: application/json

{
  "update_id": 123456789,
  "message": {
    "message_id": 123,
    "from": {
      "id": 987654321,
      "first_name": "John",
      "username": "john_doe"
    },
    "chat": {
      "id": 987654321,
      "type": "private"
    },
    "date": 1635724800,
    "text": "/start"
  }
}
```

**Ответ**: `200 OK` (всегда возвращает 200, чтобы Telegram не повторял отправку)

---

### Проверка статуса webhook'а

**Endpoint**: `GET /api/telegram/webhook/status`  
**Описание**: Проверка работоспособности Telegram webhook'а

**Пример ответа**:
```json
{
  "status": "Active",
  "timestamp": "2023-11-01T12:00:00Z",
  "environment": "Development"
}
```

---

### Тестирование webhook'а

**Endpoint**: `POST /api/telegram/webhook/test`  
**Описание**: Endpoint для тестирования webhook'а в development окружении

**Пример запроса**:
```json
POST /api/telegram/webhook/test
Content-Type: application/json

"Тестовое сообщение для проверки webhook'а"
```

**Пример ответа**:
```json
{
  "message": "Webhook тест прошел успешно",
  "receivedMessage": "Тестовое сообщение для проверки webhook'а",
  "timestamp": "2023-11-01T12:00:00Z"
}
```

---

## 📊 Hangfire Dashboard

### Мониторинг фоновых задач

**Endpoint**: `GET /hangfire`  
**Описание**: Web-интерфейс для мониторинга и управления фоновыми задачами  
**Доступность**: Только в Development окружении  
**Авторизация**: Открытый доступ в Development

**Функции Dashboard:**
- 📈 Мониторинг выполнения задач
- ⏰ Просмотр расписания регулярных задач  
- 🔄 Ручной запуск задач
- 📊 Статистика выполнения
- 🚫 Управление failed задачами

---

## 🎯 Поддерживаемые команды Telegram Bot

### Основные команды

| Команда | Описание | Пример ответа |
|---------|----------|---------------|
| `/start` | Приветствие и инструкции | "👋 Добро пожаловать в Lauf!" |
| `/help` | Справка по командам | "📚 Справка по командам Lauf Bot..." |
| `/progress` | Текущий прогресс обучения | "📈 Ваш прогресс: Активные потоки: 2..." |
| `/pause` | Приостановка уведомлений | "⏸️ Уведомления приостановлены" |
| `/resume` | Возобновление уведомлений | "▶️ Уведомления возобновлены" |

### Callback кнопки (inline клавиатура)

| Callback Data | Описание | Действие |
|---------------|----------|----------|
| `start_flow_<ID>` | Начать поток обучения | Переход в Web App для начала потока |
| `pause_flow_<ID>` | Приостановить поток | Пауза активного потока |
| `view_achievement_<ID>` | Просмотр достижения | Переход в Web App к достижениям |

---

## ⏰ Расписание фоновых задач

### Настроенные регулярные задачи

| Задача | Расписание | Описание |
|--------|------------|----------|
| `daily-reminders` | Ежедневно в 9:00 | Отправка напоминаний о незавершенных потоках |
| `deadline-checks` | Каждые 4 часа | Проверка приближающихся и просроченных дедлайнов |

### Cron выражения

```
"0 9 * * *"    - Каждый день в 9:00
"0 */4 * * *"  - Каждые 4 часа
```

---

## 🔧 Конфигурация

### Настройки Telegram в appsettings.json

```json
{
  "TelegramBot": {
    "Token": "YOUR_BOT_TOKEN",
    "WebhookUrl": "https://yourdomain.com/api/telegram/webhook"
  }
}
```

### Настройки Hangfire

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "ваша_строка_подключения"
  }
}
```

---

## 🚀 Примеры использования

### Настройка webhook'а в Telegram

```bash
curl -X POST "https://api.telegram.org/bot<YOUR_BOT_TOKEN>/setWebhook" \
  -H "Content-Type: application/json" \
  -d '{"url": "https://yourdomain.com/api/telegram/webhook"}'
```

### Проверка состояния webhook'а

```bash
curl -X GET "https://yourdomain.com/api/telegram/webhook/status"
```

### Доступ к Hangfire Dashboard

```
http://localhost:5000/hangfire
```

---

## 📝 Логирование

Все API endpoints логируют следующую информацию:

- ✅ Успешные обработки запросов
- ❌ Ошибки выполнения  
- 📊 Метрики производительности
- 🔍 Debug информация в Development

---

## 🔒 Безопасность

### Telegram Webhook
- Проверка подписи запросов (в TelegramAuthValidator)
- Валидация Web App данных
- Rate limiting (рекомендуется добавить в production)

### Hangfire Dashboard  
- В Development: открытый доступ
- В Production: требуется настройка авторизации

---

## 📈 Мониторинг

### Health Checks

**Endpoint**: `GET /health`  
**Описание**: Проверка состояния системы

### Metrics (рекомендуется добавить)

- Количество обработанных webhook'ов
- Время выполнения фоновых задач  
- Количество отправленных уведомлений
- Статистика команд Telegram Bot

---

*Документация актуальна на момент релиза системы Lauf v1.0* 