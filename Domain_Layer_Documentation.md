# Документация Domain слоя - Lauf

## Обзор
Domain слой проекта Lauf содержит основную бизнес-логику системы онбординга сотрудников. Слой реализует паттерны Clean Architecture, включая сущности, value objects, доменные события, сервисы и интерфейсы.

## Архитектурные особенности
- **Версионирование**: Полная система физического версионирования всех сущностей
- **Domain Events**: Событийно-ориентированная архитектура
- **Value Objects**: Типизированные значения для предметной области
- **CQRS**: Разделение команд и запросов
- **Repository Pattern**: Абстракция доступа к данным

---

## 1. Основные сущности (Entities)

### 1.1 Пользователи и роли

#### **User** - Пользователь системы
```csharp
public class User
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор пользователя
- `FirstName: string` - имя пользователя
- `LastName: string` - фамилия пользователя
- `FullName: string` - полное имя (вычисляемое свойство)
- `TelegramUsername: string?` - username в Telegram
- `TelegramUserId: TelegramUserId` - идентификатор пользователя в Telegram (value object)
- `Position: string?` - должность пользователя
- `Language: string` - язык интерфейса (по умолчанию "ru")
- `IsActive: bool` - активен ли пользователь (по умолчанию true)
- `CreatedAt: DateTime` - дата создания аккаунта
- `UpdatedAt: DateTime` - дата последнего обновления
- `LastActiveAt: DateTime?` - дата последней активности
- `Roles: ICollection<Role>` - роли пользователя
- `FlowAssignments: ICollection<FlowAssignment>` - назначения потоков

**Методы:**
- `HasRole(string roleName): bool` - проверяет наличие роли
- `UpdateLastActivity()` - обновляет время последней активности
- `Activate()` - активирует пользователя
- `Deactivate()` - деактивирует пользователя

#### **Role** - Роль пользователя в системе
```csharp
public class Role
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор роли
- `Name: string` - название роли
- `Description: string` - описание роли
- `IsActive: bool` - активна ли роль
- `IsSystem: bool` - системная роль (не может быть удалена)
- `CreatedAt: DateTime` - дата создания роли
- `UpdatedAt: DateTime` - дата последнего обновления
- `Users: ICollection<User>` - пользователи с данной ролью

**Методы:**
- `CanBeDeleted(): bool` - проверяет возможность удаления
- `Activate()` - активирует роль
- `Deactivate()` - деактивирует роль

#### **Achievement** - Достижение
```csharp
public class Achievement
```

**Свойства:**
- `Id: Guid` - идентификатор достижения
- `Title: string` - название достижения
- `Description: string` - описание достижения
- `Rarity: AchievementRarity` - редкость достижения
- `IconUrl: string?` - URL иконки достижения
- `IsActive: bool` - активно ли достижение
- `CreatedAt: DateTime` - дата создания
- `UpdatedAt: DateTime` - дата обновления

#### **UserAchievement** - Связь пользователя и достижения
```csharp
public class UserAchievement
```

**Свойства:**
- `Id: Guid` - идентификатор записи
- `UserId: Guid` - идентификатор пользователя
- `AchievementId: Guid` - идентификатор достижения
- `UnlockedAt: DateTime` - дата получения достижения

### 1.2 Потоки обучения

#### **Flow** - Поток обучения (шаблон обучающей программы)
```csharp
public class Flow
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор потока
- `Title: string` - название потока
- `Description: string` - описание потока
- `Tags: string` - теги для поиска
- `Status: FlowStatus` - статус потока (Draft, Published, Archived)
- `Priority: int` - приоритет отображения
- `IsRequired: bool` - обязательный ли поток
- `Settings: FlowSettings` - настройки потока
- `Steps: ICollection<FlowStep>` - шаги потока
- `Assignments: ICollection<FlowAssignment>` - назначения потока
- `CreatedById: Guid` - автор потока
- `CreatedAt: DateTime` - дата создания
- `UpdatedAt: DateTime` - дата последнего обновления
- `PublishedAt: DateTime?` - дата публикации

**Вычисляемые свойства:**
- `TotalSteps: int` - общее количество шагов в потоке

**Методы:**
- `CanBePublished(): bool` - проверяет готовность к публикации
- `Publish()` - публикует поток
- `Archive()` - архивирует поток
- `ReturnToDraft()` - возвращает в черновик
- `AddStep(FlowStep step)` - добавляет шаг
- `RemoveStep(Guid stepId)` - удаляет шаг

#### **FlowSettings** - Настройки потока обучения
```csharp
public class FlowSettings
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор настроек
- `FlowId: Guid` - идентификатор потока
- `Flow: Flow` - поток, к которому относятся настройки
- `DaysToComplete: int?` - количество дней на прохождение
- `ExcludeWeekends: bool` - учитывать ли выходные дни при расчете дедлайна
- `ExcludeHolidays: bool` - учитывать ли праздничные дни
- `RequiresBuddy: bool` - требуется ли назначение бадди
- `AutoAssignBuddy: bool` - автоматически назначать бадди
- `AllowSelfPaced: bool` - разрешить самостоятельное прохождение
- `AllowPause: bool` - можно ли ставить поток на паузу
- `SendDeadlineReminders: bool` - отправлять уведомления о приближении дедлайна
- `FirstReminderDaysBefore: int` - за сколько дней отправлять первое напоминание
- `FinalReminderDaysBefore: int` - за сколько дней отправлять финальное напоминание
- `SendDailyProgress: bool` - отправлять ежедневные уведомления о прогрессе
- `SendStepCompletionNotifications: bool` - отправлять уведомления при завершении шагов
- `CustomSettings: string` - дополнительные настройки в формате JSON
- `CreatedAt: DateTime` - дата создания настроек
- `UpdatedAt: DateTime` - дата последнего обновления

**Методы:**
- `CalculateDeadline(DateTime assignmentDate): DateTime?` - рассчитывает дедлайн
- `ShouldSendDeadlineReminder(DateTime deadline, DateTime currentDate): bool` - проверяет необходимость напоминания
- `IsOverdue(DateTime deadline, DateTime currentDate): bool` - проверяет просрочку дедлайна

#### **FlowStep** - Шаг в потоке обучения
```csharp
public class FlowStep
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор шага
- `FlowId: Guid` - идентификатор потока
- `Flow: Flow` - поток, которому принадлежит шаг
- `Title: string` - название шага
- `Description: string` - описание шага
- `Order: string` - LexoRank позиция шага для динамической сортировки
- `IsRequired: bool` - обязательный ли шаг
- `EstimatedDurationMinutes: int` - приблизительное время выполнения в минутах
- `Status: StepStatus` - статус шага (Draft, Active, Inactive)
- `Instructions: string` - инструкции для прохождения шага
- `Notes: string` - дополнительные заметки
- `Components: ICollection<ComponentBase>` - компоненты шага
- `CreatedAt: DateTime` - дата создания
- `UpdatedAt: DateTime` - дата последнего обновления

**Вычисляемые свойства:**
- `TotalComponents: int` - общее количество компонентов в шаге
- `RequiredComponents: int` - количество обязательных компонентов

**Методы:**
- `CanBeActivated(): bool` - проверяет возможность активации
- `Activate()` - активирует шаг
- `Deactivate()` - деактивирует шаг
- `ReturnToDraft()` - возвращает в черновик
- `AddComponent(ComponentBase component)` - добавляет компонент
- `RemoveComponent(Guid componentId)` - удаляет компонент

#### **FlowAssignment** - Назначение потока пользователю
```csharp
public class FlowAssignment
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор назначения
- `UserId: Guid` - идентификатор пользователя
- `User: User` - пользователь, которому назначен поток
- `FlowId: Guid` - идентификатор потока
- `Flow: Flow` - поток, который назначен пользователю
- `OriginalFlowId: Guid` - идентификатор оригинального потока
- `FlowVersionId: Guid` - идентификатор версии потока (неизменяемая связь)
- `FlowVersion: FlowVersion` - версия потока, назначенная пользователю
- `BuddyId: Guid?` - идентификатор бадди (наставника)
- `Buddy: User?` - бадди (наставник)
- `AssignedById: Guid` - идентификатор администратора, который сделал назначение
- `AssignedBy: User` - администратор, который сделал назначение
- `Status: AssignmentStatus` - статус назначения
- `Priority: int` - приоритет выполнения
- `AssignedAt: DateTime` - дата назначения
- `StartedAt: DateTime?` - дата начала прохождения
- `DueDate: DateTime?` - дедлайн завершения
- `CompletedAt: DateTime?` - дата завершения
- `LastActivityAt: DateTime?` - дата последней активности
- `PausedAt: DateTime?` - дата постановки на паузу
- `PauseReason: string?` - причина постановки на паузу
- `ProgressPercent: int` - общий прогресс в процентах (0-100)
- `CompletedSteps: int` - количество завершенных шагов
- `TotalSteps: int` - общее количество шагов в потоке
- `AttemptCount: int` - количество попыток прохождения
- `FinalScore: int?` - финальная оценка
- `AdminNotes: string` - заметки администратора
- `UserFeedback: string` - обратная связь от пользователя
- `UserRating: int?` - оценка потока пользователем (1-5)
- `CreatedAt: DateTime` - дата создания записи
- `UpdatedAt: DateTime` - дата последнего обновления

**Методы:**
- `CanStart(): bool` - проверяет возможность запуска
- `Start()` - запускает прохождение потока
- `Pause(string reason)` - ставит прохождение на паузу
- `Resume()` - возобновляет прохождение с паузы
- `Complete(int? finalScore)` - завершает прохождение потока
- `Cancel(string reason)` - отменяет назначение
- `ExtendDeadline(DateTime newDueDate)` - продлевает дедлайн
- `UpdateProgress(int completedSteps, int totalSteps)` - обновляет прогресс
- `AssignBuddy(Guid buddyId)` - назначает бадди
- `IsOverdue(): bool` - проверяет просрочку
- `IsDeadlineApproaching(int daysThreshold): bool` - проверяет приближение дедлайна
- `AddUserFeedback(string feedback, int? rating)` - добавляет обратную связь

### 1.3 Компоненты контента

#### **ComponentBase** - Базовый класс для всех компонентов
```csharp
public abstract class ComponentBase
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор компонента
- `Type: ComponentType` - тип компонента (абстрактное свойство)
- `Title: string` - название компонента
- `Description: string` - описание компонента
- `Status: ComponentStatus` - статус компонента
- `EstimatedDurationMinutes: int` - приблизительное время выполнения в минутах
- `MaxAttempts: int?` - максимальное количество попыток
- `MinPassingScore: int?` - минимальный проходной балл
- `Instructions: string` - дополнительные инструкции
- `CreatedAt: DateTime` - дата создания
- `UpdatedAt: DateTime` - дата последнего обновления
- `FlowStepId: Guid` - идентификатор шага потока
- `Order: string` - порядковый номер компонента (LexoRank)
- `IsRequired: bool` - обязательный ли компонент

**Методы:**
- `CanBeActivated(): bool` - проверяет возможность активации
- `Activate()` - активирует компонент
- `Deactivate()` - деактивирует компонент
- `ReturnToDraft()` - возвращает в черновик
- `UpdateBasicInfo(string title, string description, string? instructions)` - обновляет основную информацию
- `SetScoringSettings(int? maxAttempts, int? minPassingScore)` - устанавливает настройки оценивания
- `IsInteractive(): bool` - проверяет интерактивность компонента
- `SupportsProgress(): bool` - проверяет поддержку отслеживания прогресса
- `HasScoring(): bool` - проверяет наличие системы оценок

#### **ArticleComponent** - Компонент статьи для чтения
```csharp
public class ArticleComponent : ComponentBase
```

**Дополнительные свойства:**
- `Type: ComponentType.Article` - тип компонента (статья)
- `Content: string` - содержимое статьи в формате Markdown
- `ReadingTimeMinutes: int` - время чтения в минутах

**Методы:**
- `UpdateContent(string content, int? readingTimeMinutes)` - обновляет содержимое статьи

#### **QuizComponent** - Компонент квиза
```csharp
public class QuizComponent : ComponentBase
```

**Дополнительные свойства:**
- `Type: ComponentType.Quiz` - тип компонента (квиз)
- `QuestionText: string` - текст вопроса
- `Options: List<QuestionOption>` - варианты ответов (ровно 5)

**Методы:**
- `SetQuestion(string questionText, List<QuestionOption> options)` - устанавливает вопрос и варианты
- `AddOption(QuestionOption option)` - добавляет вариант ответа
- `GetTotalScore(): int` - подсчитывает общий балл

#### **QuestionOption** - Вариант ответа на вопрос
```csharp
public class QuestionOption
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор варианта
- `Text: string` - текст варианта ответа
- `IsCorrect: bool` - является ли вариант правильным
- `Order: string` - LexoRank позиция варианта
- `Message: string` - сообщение, показываемое при выборе
- `Points: int` - количество баллов за правильный ответ

#### **TaskComponent** - Компонент практического задания
```csharp
public class TaskComponent : ComponentBase
```

**Дополнительные свойства:**
- `Type: ComponentType.Task` - тип компонента (задание)
- `Instruction: string` - инструкция как найти кодовое слово
- `CodeWord: string` - кодовое слово для проверки ответа
- `Hint: string` - подсказка, доступная в любой момент

**Методы:**
- `UpdateInstruction(string instruction, string? hint)` - обновляет инструкцию
- `SetCodeWord(string codeWord)` - устанавливает кодовое слово
- `CheckAnswer(string answer): bool` - проверяет правильность ответа

### 1.4 Система версионирования

#### **FlowVersion** - Версия потока обучения
```csharp
public class FlowVersion : IVersionedEntity<FlowVersion>
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор версии
- `OriginalId: Guid` - идентификатор оригинального потока
- `Version: int` - номер версии
- `IsActive: bool` - является ли данная версия активной
- `Title: string` - название потока
- `Description: string` - описание потока
- `Tags: string` - теги потока (разделенные запятыми)
- `Status: FlowStatus` - статус потока
- `Priority: int` - приоритет потока
- `IsRequired: bool` - является ли поток обязательным
- `CreatedById: Guid` - идентификатор создателя
- `CreatedAt: DateTime` - дата создания версии
- `UpdatedAt: DateTime` - дата последнего обновления версии
- `PublishedAt: DateTime?` - дата публикации (если опубликован)
- `StepVersions: ICollection<FlowStepVersion>` - коллекция версий этапов
- `Assignments: ICollection<FlowAssignment>` - назначения, использующие эту версию

**Методы:**
- `CreateNewVersion(): FlowVersion` - создать новую версию на основе текущей
- `Activate()` - активировать эту версию
- `Deactivate()` - деактивировать эту версию
- `UpdateMetadata(...)` - обновить метаданные версии
- `ChangeStatus(FlowStatus newStatus)` - изменить статус версии
- `AddStepVersion(FlowStepVersion stepVersion)` - добавить версию этапа
- `RemoveStepVersion(FlowStepVersion stepVersion)` - удалить версию этапа

#### **FlowStepVersion** - Версия этапа потока
```csharp
public class FlowStepVersion : IVersionedEntity<FlowStepVersion>
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор версии этапа
- `OriginalId: Guid` - идентификатор оригинального этапа
- `Version: int` - номер версии
- `IsActive: bool` - является ли данная версия активной
- `FlowVersionId: Guid` - идентификатор версии потока
- `Title: string` - название этапа
- `Description: string` - описание этапа
- `Order: string` - порядок этапа (LexoRank)
- `IsRequired: bool` - является ли этап обязательным
- `EstimatedDurationMinutes: int` - оценочное время выполнения в минутах
- `Status: StepStatus` - статус этапа
- `Instructions: string` - инструкции для этапа
- `Notes: string` - заметки по этапу
- `CreatedAt: DateTime` - дата создания версии
- `UpdatedAt: DateTime` - дата последнего обновления версии
- `FlowVersion: FlowVersion` - версия потока, к которой принадлежит этот этап
- `ComponentVersions: ICollection<ComponentVersion>` - коллекция версий компонентов

**Методы:**
- `CreateNewVersion(): FlowStepVersion` - создать новую версию на основе текущей
- `Activate()` - активировать эту версию
- `Deactivate()` - деактивировать эту версию
- `UpdateMetadata(...)` - обновить метаданные версии этапа
- `ChangeStatus(StepStatus newStatus)` - изменить статус версии этапа
- `AddComponentVersion(ComponentVersion componentVersion)` - добавить версию компонента
- `RemoveComponentVersion(ComponentVersion componentVersion)` - удалить версию компонента

#### **ComponentVersion** - Версия компонента обучения
```csharp
public class ComponentVersion : IVersionedEntity<ComponentVersion>
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор версии компонента
- `OriginalId: Guid` - идентификатор оригинального компонента
- `Version: int` - номер версии
- `IsActive: bool` - является ли данная версия активной
- `StepVersionId: Guid` - идентификатор версии этапа
- `Title: string` - название компонента
- `Description: string` - описание компонента
- `ComponentType: ComponentType` - тип компонента
- `Status: ComponentStatus` - статус компонента
- `Order: string` - порядок компонента (LexoRank)
- `IsRequired: bool` - является ли компонент обязательным
- `EstimatedDurationMinutes: int` - оценочное время выполнения в минутах
- `MaxAttempts: int?` - максимальное количество попыток
- `MinPassingScore: int?` - минимальный проходной балл
- `Instructions: string` - инструкции для компонента
- `CreatedAt: DateTime` - дата создания версии
- `UpdatedAt: DateTime` - дата последнего обновления версии
- `StepVersion: FlowStepVersion` - версия этапа, к которой принадлежит этот компонент
- `ArticleVersion: ArticleComponentVersion?` - версия статьи (если тип = Article)
- `QuizVersion: QuizComponentVersion?` - версия квиза (если тип = Quiz)
- `TaskVersion: TaskComponentVersion?` - версия задания (если тип = Task)

**Методы:**
- `CreateNewVersion(): ComponentVersion` - создать новую версию на основе текущей
- `Activate()` - активировать эту версию
- `Deactivate()` - деактивировать эту версию
- `UpdateMetadata(...)` - обновить метаданные версии компонента
- `ChangeStatus(ComponentStatus newStatus)` - изменить статус версии компонента

### 1.5 Прогресс и отслеживание

#### **ComponentProgress** - Прогресс пользователя по конкретному компоненту
```csharp
public class ComponentProgress
```

**Свойства:**
- `Id: Guid` - идентификатор записи прогресса
- `StepProgressId: Guid` - идентификатор прогресса шага
- `StepProgress: StepProgress` - прогресс шага
- `ComponentVersionId: Guid` - идентификатор версии компонента
- `ComponentVersion: ComponentVersion` - версия компонента
- `Order: int` - порядковый номер компонента в шаге
- `IsRequired: bool` - является ли компонент обязательным
- `Status: ProgressStatus` - статус выполнения компонента
- `IsCompleted: bool` - завершен ли компонент
- `AttemptsCount: int` - количество попыток выполнения
- `BestScore: int?` - лучший результат (для квизов и заданий)
- `LastScore: int?` - последний результат
- `TimeSpentMinutes: int` - время, потраченное на компонент в минутах
- `ProgressData: ComponentProgressData` - данные прогресса компонента (JSON)
- `StartedAt: DateTime?` - дата начала выполнения
- `CompletedAt: DateTime?` - дата завершения
- `LastUpdatedAt: DateTime` - дата последнего обновления

**Методы:**
- `Start()` - начать выполнение компонента
- `Complete(int? score)` - завершить компонент
- `RegisterAttempt(int? score, ComponentProgressData? progressData)` - зарегистрировать попытку
- `AddTimeSpent(int minutes)` - добавить время выполнения
- `UpdateProgressData(ComponentProgressData progressData)` - обновить данные прогресса
- `Pause()` - приостановить выполнение
- `Resume()` - возобновить выполнение
- `Reset()` - сбросить прогресс
- `CanAttempt(int? maxAttempts): bool` - проверить возможность новой попытки
- `HasPassingScore(int? minimumScore): bool` - проверить достижение минимального балла
- `GetProgressData<T>(): T?` - получить типизированные данные прогресса

### 1.6 Уведомления

#### **Notification** - Уведомление для пользователя
```csharp
public class Notification
```

**Свойства:**
- `Id: Guid` - уникальный идентификатор уведомления
- `UserId: Guid` - идентификатор пользователя-получателя
- `Type: NotificationType` - тип уведомления
- `Channel: NotificationChannel` - канал доставки
- `Priority: NotificationPriority` - приоритет уведомления
- `Title: string` - заголовок уведомления
- `Content: string` - содержимое уведомления
- `Metadata: string?` - дополнительные данные в JSON формате
- `Status: NotificationStatus` - статус уведомления
- `CreatedAt: DateTime` - дата создания
- `ScheduledAt: DateTime` - дата запланированной отправки
- `SentAt: DateTime?` - дата фактической отправки
- `ReadAt: DateTime?` - дата прочтения пользователем
- `AttemptCount: int` - количество попыток отправки
- `MaxAttempts: int` - максимальное количество попыток
- `ErrorMessage: string?` - сообщение об ошибке при отправке
- `RelatedEntityId: Guid?` - связанная сущность (поток, назначение, достижение)
- `RelatedEntityType: string?` - тип связанной сущности
- `User: User` - пользователь

**Методы:**
- `MarkAsSent()` - отметить как отправленное
- `MarkAsRead()` - отметить как прочитанное
- `MarkAsFailed(string errorMessage)` - отметить как неудачную попытку
- `IsReadyToSend(): bool` - проверить готовность к отправке

**Статические методы:**
- `CreateFlowAssigned(...)` - создать уведомление о назначении потока
- `CreateDeadlineReminder(...)` - создать уведомление о приближении дедлайна
- `CreateAchievementUnlocked(...)` - создать уведомление о получении достижения

---

## 2. Value Objects

### **TelegramUserId** - Идентификатор пользователя Telegram
```csharp
public sealed class TelegramUserId : IEquatable<TelegramUserId>
```

**Свойства:**
- `Value: long` - числовое значение идентификатора Telegram

**Методы:**
- `FromString(string value): TelegramUserId` - создание из строки
- `FromLong(long value): TelegramUserId` - создание из числа
- `FromInt(int value): TelegramUserId` - создание из числа
- `IsLikelyBot(): bool` - проверка на бота
- `IsValidUserRange(): bool` - проверка допустимого диапазона

### **ProgressPercentage** - Процент прогресса (0-100)
```csharp
public class ProgressPercentage
```

**Свойства:**
- `Value: decimal` - значение процента (0-100)

**Методы:**
- `FromRatio(int completed, int total): ProgressPercentage` - создание из соотношения

### **ComponentProgressData** - Данные прогресса компонента
```csharp
public class ComponentProgressData
```

**Свойства:**
- `Data: string` - данные прогресса в JSON формате

**Методы:**
- `Empty: ComponentProgressData` - пустые данные прогресса
- `Create<T>(T data): ComponentProgressData` - создание из объекта
- `GetData<T>(): T?` - получение типизированных данных

### **DeadlineCalculation** - Расчет дедлайнов
```csharp
public class DeadlineCalculation
```

### **VersionNumber** - Номер версии
```csharp
public class VersionNumber
```

### **VersionedEntityId** - Идентификатор версионируемой сущности
```csharp
public class VersionedEntityId
```

---

## 3. Enums

### **FlowStatus** - Статус потока обучения
- `Draft = 0` - черновик (поток в разработке)
- `Published = 1` - опубликован и доступен для назначения
- `Archived = 2` - архивный (недоступен для новых назначений)

### **AssignmentStatus** - Статус назначения потока
- `Assigned = 0` - назначен, но не начат
- `InProgress = 1` - в процессе выполнения
- `Paused = 2` - приостановлен
- `Completed = 3` - завершен успешно
- `Cancelled = 4` - отменен
- `Overdue = 5` - просрочен

### **ComponentType** - Тип компонента контента
- `Article = 0` - статья для чтения
- `Quiz = 1` - квиз с вопросами и вариантами ответов
- `Task = 2` - практическое задание

### **ComponentStatus** - Статус компонента
- `Draft = 0` - черновик
- `Active = 1` - активный
- `Inactive = 2` - неактивный

### **StepStatus** - Статус шага
- `Draft = 0` - черновик
- `Active = 1` - активный
- `Inactive = 2` - неактивный

### **ProgressStatus** - Статус прогресса
- `NotStarted = 0` - не начат
- `InProgress = 1` - в процессе
- `Completed = 2` - завершен
- `Paused = 3` - приостановлен

### **NotificationType** - Тип уведомления
- `FlowAssigned` - назначен поток
- `DeadlineReminder` - напоминание о дедлайне
- `AchievementUnlocked` - получено достижение
- `StepCompleted` - завершен шаг
- `FlowCompleted` - завершен поток

### **NotificationChannel** - Канал доставки уведомлений
- `Telegram` - Telegram бот
- `Email` - электронная почта
- `InApp` - внутри приложения

### **NotificationPriority** - Приоритет уведомления
- `Low` - низкий
- `Medium` - средний
- `High` - высокий
- `Critical` - критический

### **NotificationStatus** - Статус уведомления
- `Pending` - ожидает отправки
- `Sent` - отправлено
- `Read` - прочитано
- `Failed` - не удалось отправить

### **AchievementRarity** - Редкость достижения
- `Common` - обычное
- `Rare` - редкое
- `Epic` - эпическое
- `Legendary` - легендарное

### **TaskSubmissionType** - Тип подачи задания
- `CodeWord` - кодовое слово
- `File` - файл
- `Text` - текст

---

## 4. Interfaces

### 4.1 Сервисы

#### **IVersioningService** - Сервис управления версионированием
Управляет жизненным циклом версий сущностей:
- Создание новых версий потоков, этапов и компонентов
- Активация/деактивация версий
- Получение активных и конкретных версий
- Очистка неиспользуемых версий

#### **IProgressCalculationService** - Сервис расчета прогресса
Рассчитывает прогресс пользователей по потокам и компонентам.

#### **IDomainEventDispatcher** - Диспетчер доменных событий
Обрабатывает и распространяет доменные события.

### 4.2 Репозитории

#### **IUserRepository** - Репозиторий пользователей
- Поиск пользователей по различным критериям
- Управление ролями пользователей
- Отслеживание активности

#### **IFlowRepository** - Репозиторий потоков
- CRUD операции с потоками
- Поиск и фильтрация потоков
- Управление публикацией

#### **IFlowVersionRepository** - Репозиторий версий потоков
- Управление версиями потоков
- Поиск активных версий
- Каскадные операции с версиями

#### **IFlowAssignmentRepository** - Репозиторий назначений
- Управление назначениями потоков
- Отслеживание прогресса
- Фильтрация по статусам и дедлайнам

#### **IComponentRepository** - Репозиторий компонентов
- CRUD операции с компонентами
- Управление порядком компонентов

#### **IComponentVersionRepository** - Репозиторий версий компонентов
- Управление версиями компонентов
- Связи между версиями

#### **INotificationRepository** - Репозиторий уведомлений
- Управление уведомлениями
- Планирование отправки
- Отслеживание статусов

### 4.3 Общие интерфейсы

#### **IVersionedEntity<T>** - Интерфейс версионируемой сущности
Определяет контракт для всех версионируемых сущностей:
- `Id: Guid` - уникальный идентификатор версии
- `OriginalId: Guid` - идентификатор оригинальной сущности
- `Version: int` - номер версии
- `IsActive: bool` - активность версии
- `CreatedAt: DateTime` - дата создания
- `UpdatedAt: DateTime` - дата обновления
- `CreateNewVersion(): T` - создание новой версии

#### **IUnitOfWork** - Единица работы
Обеспечивает транзакционность операций:
- `SaveChangesAsync(): Task<int>` - сохранение изменений
- `BeginTransactionAsync(): Task` - начало транзакции
- `CommitTransactionAsync(): Task` - подтверждение транзакции
- `RollbackTransactionAsync(): Task` - откат транзакции

---

## 5. Domain Events

### **IDomainEvent** - Интерфейс доменного события
```csharp
public interface IDomainEvent
```

**Свойства:**
- `EventId: Guid` - уникальный идентификатор события
- `OccurredAt: DateTime` - время возникновения события
- `Version: int` - версия события (для совместимости)

### Конкретные события:

#### **FlowAssigned** - Поток назначен пользователю
#### **FlowCompleted** - Поток завершен
#### **ComponentCompleted** - Компонент завершен
#### **StepUnlocked** - Шаг разблокирован

---

## 6. Exceptions

### **DomainException** - Базовое доменное исключение
### **FlowNotFoundException** - Поток не найден
### **FlowAlreadyAssignedException** - Поток уже назначен
### **UserNotFoundException** - Пользователь не найден

---

## 7. Services (Конкретные реализации)

### **ProgressCalculationService** - Сервис расчета прогресса
```csharp
public class ProgressCalculationService : IProgressCalculationService
```

Реализует логику расчета прогресса по потокам, шагам и компонентам.

---

## Рекомендации по оптимизации

### Потенциально избыточные свойства:

1. **FlowAssignment**:
   - `AttemptCount` - может быть избыточным, если не используется логика повторных попыток
   - `FinalScore` - зависит от бизнес-логики оценивания
   - `UserRating` - может быть вынесено в отдельную сущность Feedback

2. **User**:
   - `Position` - зависит от необходимости в HR-функциях
   - `Language` - может быть заменено на enum

3. **FlowSettings**:
   - Много флагов уведомлений - можно объединить в настройки уведомлений

4. **Notification**:
   - `AttemptCount` и `MaxAttempts` - зависит от требований к надежности доставки

### Ключевые свойства для сохранения:

1. Все свойства версионирования (критичны для архитектуры)
2. Временные метки (CreatedAt, UpdatedAt) - важны для аудита
3. Статусы и флаги активности - критичны для бизнес-логики
4. Связи между сущностями (Foreign Keys) - основа целостности данных
5. LexoRank поля (Order) - обеспечивают гибкую сортировку

Этот анализ показывает, что большинство свойств имеют четкое назначение в контексте системы онбординга. Оптимизация должна основываться на конкретных требованиях производительности и функциональности.