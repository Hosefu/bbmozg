# Новая архитектура системы после изменений

## Обзор изменений

Проведена кардинальная реструктуризация доменного слоя с целью упрощения архитектуры, устранения дублирования данных и четкого разделения ответственности между сущностями.

## Ключевые принципы новой архитектуры

1. **Разделение метаданных и контента** - координаторы vs исполнители
2. **Упрощение версионирования** - версионируется только контент
3. **Устранение дублирования** - убраны избыточные поля и связи
4. **Четкая ответственность** - каждая сущность имеет определенную роль

---

## 1. Основные сущности

### 1.1 Пользователи

#### **User** - Пользователь системы (упрощенный)
```csharp
public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; } // вычисляемое
    public string? TelegramUsername { get; set; }
    public TelegramUserId TelegramUserId { get; set; }
    public bool IsActive { get; set; } = true
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastActiveAt { get; set; }
    
    // Навигационные свойства
    public ICollection<Role> Roles { get; set; }
    public ICollection<FlowAssignment> FlowAssignments { get; set; }
}
```

**Удалено:** `Language` (не нужно), `Position` (не нужно)

### 1.2 Система потоков (новая архитектура)

#### **Flow** - Координатор потока
```csharp
public class Flow
{
    // Основная идентификация
    public Guid Id { get; set; }
    public string Name { get; set; }             // Неизменное административное имя
    public string Description { get; set; }     // Базовое описание
    
    // Административные метаданные
    public Guid CreatedBy { get; set; }         // Кто создал поток
    public DateTime CreatedAt { get; set; }     // Когда создан
    public DateTime UpdatedAt { get; set; }     // Последнее обновление
    public bool IsActive { get; set; }          // Можно ли создавать назначения
    
    // Управление версиями контента
    public Guid ActiveContentId { get; set; }   // Ссылка на активную версию контента
    
    // Навигационные свойства
    public User CreatedByUser { get; set; }
    public FlowSettings Settings { get; set; }
    public FlowContent ActiveContent { get; set; }
    public ICollection<FlowContent> Contents { get; set; }
    public ICollection<FlowAssignment> Assignments { get; set; }
}
```

**Удалено:** `EstimatedDuration`, `Tags`, `Priority`, `IsRequired`, IsArchived (не нужны на уровне координатора)

#### **FlowSettings** - Настройки потока (не версионируются)
```csharp
public class FlowSettings
{
    public Guid Id { get; set; }
    public Guid FlowId { get; set; }
    
    // Настройки дедлайнов
    public int DaysPerStep { get; set; }        // Дней на один шаг (новое)
    
    // Настройки прохождения
    public bool RequireSequentialCompletionComponents { get; set; }
    public bool AllowSelfRestart { get; set; }
    public bool AllowSelfPause { get; set; }    // Переименовано для ясности
    
    // Настройки уведомлений
    public bool SendStartNotification { get; set; }
    public bool SendProgressReminders { get; set; }
    public bool SendCompletionNotification { get; set; }
    public TimeSpan ReminderInterval { get; set; }
    
    // Навигационные свойства
    public Flow Flow { get; set; }
}
```

**Удалено:** `DaysToComplete`, `ExcludeHolidays`, `ExcludeWeekends`, `RequiresBuddy`, `AutoAssignBuddy`, `AllowSelfPaced`, `CustomSettings`

#### **FlowContent** - Версионируемый контент потока (бывший FlowVersion)
```csharp
public class FlowContent
{
    // Версионирование
    public Guid Id { get; set; }
    public Guid FlowId { get; set; }            // Ссылка на Flow-координатор
    public int Version { get; set; }            // Номер версии
    public DateTime CreatedAt { get; set; }     // Когда создана версия
    public Guid CreatedBy { get; set; }         // Кто создал версию
    
    // Навигационные свойства
    public Flow Flow { get; set; }
    public User CreatedByUser { get; set; }
    public ICollection<FlowStep> Steps { get; set; } // Привязка к контенту
    public ICollection<FlowAssignment> Assignments { get; set; }
}
```

#### **FlowAssignment** - Назначение потока (упрощенное)
```csharp
public class FlowAssignment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FlowId { get; set; }            // Координатор
    public Guid FlowContentId { get; set; }     // Конкретная версия контента
    public Guid AssignedBy { get; set; }        // Кто назначил (админ/бадди)
    
    public AssignmentStatus Status { get; set; }
    public DateTime AssignedAt { get; set; }
    
    // Навигационные свойства
    public User User { get; set; }
    public Flow Flow { get; set; }
    public FlowContent FlowContent { get; set; }
    public User AssignedByUser { get; set; }
    public ICollection<User> Buddies { get; set; }  // Много бадди
    public FlowAssignmentProgress Progress { get; set; }
}
```

**Удалено:** `Priority`, `DueDate` (вычисляется), `AttemptCount` (перенесено в Progress)

#### **FlowAssignmentProgress** - Прогресс назначения (новая сущность)
```csharp
public class FlowAssignmentProgress
{
    public Guid Id { get; set; }
    public Guid FlowAssignmentId { get; set; }
    
    // Прогресс
    public int ProgressPercent { get; set; }
    public int CompletedSteps { get; set; }
    public int TotalSteps { get; set; }
    public int AttemptCount { get; set; }
    public int? FinalScore { get; set; }
    
    // Временные метки
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? PausedAt { get; set; }
    public string? PauseReason { get; set; }
    
    // Обратная связь
    public int? UserRating { get; set; }        // Оценка пользователя
    public string? UserFeedback { get; set; }   // Отзыв пользователя
    
    // Навигационные свойства
    public FlowAssignment FlowAssignment { get; set; }
}
```

### 1.3 Шаги и компоненты (упрощенные)

#### **FlowStep** - Шаг потока (больше не версионируется)
```csharp
public class FlowStep
{
    public Guid Id { get; set; }
    public Guid FlowContentId { get; set; }     // Привязка к версии контента
    public string Name { get; set; }
    public string Description { get; set; }
    public string Order { get; set; }           // LexoRank для сортировки
    public bool IsEnabled { get; set; } = true // Заменяет StepStatus
    
    // Навигационные свойства
    public FlowContent FlowContent { get; set; }
    public ICollection<ComponentBase> Components { get; set; }
}
```

#### **ComponentBase** - Базовый компонент (упрощенный)
```csharp
public abstract class ComponentBase
{
    public Guid Id { get; set; }
    public Guid FlowStepId { get; set; }        // Привязка к шагу
    public ComponentType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public string Order { get; set; }           // LexoRank для сортировки
    public bool IsRequired { get; set; } = true
    public bool IsEnabled { get; set; } = true  // Заменяет ComponentStatus
    
    // Система подсчета очков (новая логика)
    public virtual int GetTotalScore() => 0;    // Переопределяется в наследниках
    public virtual bool HasScore => false;      // Дает ли компонент очки
    
    // Навигационные свойства
    public FlowStep FlowStep { get; set; }
}
```

**Удалено:** `EstimatedDurationMinutes`, `MaxAttempts`, `MinPassingScore`

#### **ArticleComponent** - Компонент статьи
```csharp
public class ArticleComponent : ComponentBase
{
    public override ComponentType Type => ComponentType.Article;
    public override bool HasScore => false;     // Статьи не дают очков
    
    // Вычисляемое свойство
    public int ReadingTimeMinutes => CalculateReadingTime(Content);
    
    private int CalculateReadingTime(string content)
    {
        // Примерно 200 слов в минуту
        var wordCount = content?.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length ?? 0;
        return Math.Max(1, wordCount / 200);
    }
}
```

**Удалено:** `ReadingTimeMinutes` как хранимое поле

#### **QuizComponent** - Компонент теста
```csharp
public class QuizComponent : ComponentBase
{
    public override ComponentType Type => ComponentType.Quiz;
    public override bool HasScore => true;      // Тесты дают очки
    
    public bool AllowMultipleAttempts { get; set; }
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    
    // Навигационные свойства
    public ICollection<QuizQuestion> Questions { get; set; }
    
    public override int GetTotalScore()
    {
        return Questions?.Sum(q => q.GetMaxScore()) ?? 0;
    }
}
```

#### **QuizQuestion** - Вопрос теста
```csharp
public class QuizQuestion
{
    public Guid Id { get; set; }
    public Guid QuizComponentId { get; set; }
    public string Text { get; set; }
    public bool IsRequired { get; set; }
    public string Order { get; set; }
    
    // Навигационные свойства
    public QuizComponent QuizComponent { get; set; }
    public ICollection<QuestionOption> Options { get; set; }
    
    public int GetMaxScore()
    {
        return Options?.Where(o => o.IsCorrect).Sum(o => o.Score) ?? 0;
    }
}
```

#### **QuestionOption** - Вариант ответа
```csharp
public class QuestionOption
{
    public Guid Id { get; set; }
    public Guid QuizQuestionId { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public int Score { get; set; }              // Переименовано с Points
    public string Order { get; set; }
    
    // Навигационные свойства
    public QuizQuestion QuizQuestion { get; set; }
}
```

#### **TaskComponent** - Компонент задания (только кодовое слово)
```csharp
public class TaskComponent : ComponentBase
{
    public override ComponentType Type => ComponentType.Task;
    public override bool HasScore => true;      // Задания дают очки
    
    public string CodeWord { get; set; }        // Правильный ответ
    public int Score { get; set; }              // Очки за правильный ответ
    public bool IsCaseSensitive { get; set; }   // Учитывать ли регистр
    
    public override int GetTotalScore() => Score;
}
```

**Удалено:** `Instruction` (дублирование с ComponentBase.Description), `TaskSubmissionType` (только кодовое слово)

## 2. Enums (обновленные)

### **ProgressStatus** - Статус прогресса (обновленный)
```csharp
public enum ProgressStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4          // Новое значение
}
```

### **NotificationChannel** - Канал уведомлений (упрощенный)
```csharp
public enum NotificationChannel
{
    Telegram = 0,
    System = 1            // Убран Email
}
```

### **NotificationStatus** - Статус уведомления (упрощенный)
```csharp
public enum NotificationStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2           // Убран Read
}
```

## 3. Удаленные сущности

### **ComponentProgress** - Удалена полностью
Причина: Излишняя детализация, прогресс отслеживается на уровне шагов

### **ComponentProgressData** - Удалена полностью
Причина: Избыточность

### **FlowStepVersion, ComponentVersion** - Удалены полностью
Причина: Упрощение версионирования, теперь дублируются шаги и компоненты при создании новой версии контента

## 4. Обновленные сущности

### **Notification** - Упрощенная
```csharp
public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    
    // Навигационные свойства
    public User User { get; set; }
}
```

**Удалено:** `Metadata`, `ReadAt`, `ErrorMessage`

## 5. Стратегия версионирования

### Принцип "Умного версионирования"
```csharp
public async Task<FlowContent> UpdateFlowContentAsync(Guid flowId, UpdateFlowDto dto)
{
    var flow = await _context.Flows
        .Include(f => f.ActiveContent)
        .FirstOrDefaultAsync(f => f.Id == flowId);
    
    var activeContent = flow.ActiveContent;
    
    // Проверяем, есть ли активные или завершенные назначения
    var hasActiveAssignments = await _context.FlowAssignments
        .AnyAsync(fa => fa.FlowContentId == activeContent.Id && 
                       (fa.Status == AssignmentStatus.InProgress || 
                        fa.Status == AssignmentStatus.Completed));
    
    if (!hasActiveAssignments)
    {
        // Можем редактировать текущую версию
        UpdateExistingContent(activeContent, dto);
    }
    else
    {
        // Создаем новую версию
        var newContent = await CreateNewContentVersion(activeContent, dto);
        flow.ActiveContentId = newContent.Id;
    }
    
    await _context.SaveChangesAsync();
    return flow.ActiveContent;
}
```

### Процесс создания новой версии
1. Создается новый `FlowContent`
2. Дублируются все `FlowStep` с привязкой к новой версии
3. Дублируются все `ComponentBase` с привязкой к новым шагам
4. Обновляется `Flow.ActiveContentId`
