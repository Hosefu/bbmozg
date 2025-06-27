Из сущности User удалить language и position

Изменить концепт Flow и FlowVersion (касается всех компонентов где информация дублируется в версии). Нужно избежать дублирования сущностей и данных. Лучше чёткто разделить ответственность сущностей. Сеейчас каша.

Лучше сделать для всех сущностей с версиями по такому принципу:
 Разделение метаданных и контента: Flow как координатор версий
Идея заключается в четком разделении ответственности между двумя типами сущностей:
- **Flow** - "координатор", отвечает за метаданные, административные функции и группировку версий
- **FlowVersion** - "исполнитель", содержит весь контент и бизнес-логику
## Архитектурное разделение

### Flow - Координатор версий

```csharp
public class Flow
{
    // Основная идентификация
    public Guid Id { get; set; }
    public string Name { get; set; }             // Имя потока (неизменное)
    public string Description { get; set; }     // Описание потока (может изменяться)
    
    // Административные метаданные
    public Guid CreatedBy { get; set; }         // Кто создал поток
    public DateTime CreatedAt { get; set; }     // Когда создан
    public DateTime UpdatedAt { get; set; }     // Последнее обновление
    public bool IsActive { get; set; }          // Можно ли создавать назначения
    public bool IsArchived { get; set; }        // Архивирован ли поток
    
    // Управление версиями
    public Guid ActiveVersionId { get; set; }   // Ссылка на активную версию

    // Навигационные свойства
    public FlowVersion ActiveVersion { get; set; }
    public ICollection<FlowVersion> Versions { get; set; }
    public ICollection<FlowAssignment> Assignments { get; set; }
}
```

### FlowVersion - Контент (версионируется и привязывается к назначению)

```csharp
public class FlowVersion
{
    // Версионирование
    public Guid Id { get; set; }
    public Guid FlowId { get; set; }            // Ссылка на Flow-координатор
    public int Version { get; set; }            // Номер версии
    public bool IsActive { get; set; }          // Активная версия (только одна)
    public DateTime CreatedAt { get; set; }     // Когда создана версия
    public Guid CreatedBy { get; set; }         // Кто создал версию
    public string ChangeLog { get; set; }       // Описание изменений
    
    // Весь контент и настройки
    public string Content { get; set; }         // Основное содержимое
    public TimeSpan EstimatedDuration { get; set; }
    public int MaxAttempts { get; set; }
    public bool RequireSequentialCompletion { get; set; }
    public TimeSpan? TimeLimit { get; set; }
    
    // Настройки уведомлений (для этой версии)
    public bool SendStartNotification { get; set; }
    public bool SendProgressReminders { get; set; }
    public bool SendCompletionNotification { get; set; }
    public TimeSpan ReminderInterval { get; set; }
    
    // Контент (этапы и компоненты)
    public ICollection<FlowStepVersion> StepVersions { get; set; }
    
    // Навигационные свойства
    public Flow Flow { get; set; }
    public ICollection<FlowAssignment> Assignments { get; set; }
}
```


Ото всюду удаляем свойство EstimatedDuration. Считать дедлайн нужно согласно настройкам флоу-сеттингс. А не по хардовым данным.

У Flow вообще не нужны свойства: EstimatedDuration, Tags, Priority, IsRequired

FlowSettings: 
убрать DaysToComplete, - Вместо этого лучше добавить свойство "Сколько дней на один шаг потока дедлайн"
ExcludeHolidays,  - убрать это свойство пока оверинжиниринг, по умолчанию не считать праздники и выходные в дедлайне, пусть люди отдыхают
ExcludeWeekends, - убрать это свойство пока оверинжиниринг, по умолчанию не считать праздники и выходные в дедлайне, пусть люди отдыхают
RequiresBuddy - всегда треуебтся бадди, убрать
AutoAssignBuddy - убрать, нет такой функции пока
AllowSelfPaced - убрать вообще пока
AllowPause - нужно переименовать чтобы было понятно "разрешить ставить паузу самостоятельно"
CustomSettings - удалить свойство. мы не храним ничего в JSON

Теперь поговорим о связях
Давай будем делать так:
Flow (Сущность группирующая поток, название, ссылки на все подсущности)
-FlowSettings (Настройки флоу, не версионируются)
-FlowContent (Контент флоу (его шаги), которые версионируются) (Бывший FlowVersion)
-FlowAssignment (Назначения флоу. Ссылка внутри на версию, которая назначена)

-FlowStep (шаг потока, привязывается к конкретной версии, содержит компоненты) - больше не нужно версионировать. Он привязывается к конкретной версии контента. Если создаётся новая версия то все шаги просто дублируются и привязываются к нужной версии.

-Component ... (Компоненты шагов и так далее ведут себя аналогично с FlowStep. Они больше не версионируются, а привязываются к конкретному FlowStep, который в свою очередь привязан к конкретному FlowContent, а он в свою очередь версионируется)

Стратегия версий:
если на текущей версии 0 assignment то и создавать версию незачем.
Смысл версию это сохранения контента у юзеров. Но если версия которую мы пыетаеся редактировать не имеет начатых или завершенных назначений, то и создавать новую версию не нужно. Можно редактировать старую

убрать везде дублирующие связи сущностьID. Если сущность привязана какая-то, то не нужно делать дублирующее поле сущностьID. типа UserId и User

FlowAssignment:
Buddy - он может быть не один, их может быть много
AssignedBy - это не администратор а админ или бадди который создал назначение
Priority - удалить бред
DueDate - бред. вычисляется динамически хранить смысла нет
AttemptCount - удалить. нет у нас попыток вообще нигде

Давай многое вынесем в отдельную новую сущность FlowAssignmentProgress или типа того:
    public Guid Id { get; set; }
    public Guid FlowAssignmentId { get; set; }
    public FlowAssignment FlowAssignment { get; set; }

    public int ProgressPercent { get; set; }
    public int CompletedSteps { get; set; }
    public int TotalSteps { get; set; }
    public int AttemptCount { get; set; }
    public int? FinalScore { get; set; }

    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastActivityAt { get; set; }
    public DateTime? PausedAt { get; set; }
    public string? PauseReason { get; set; }

    public int? UserRating { get; set; }
    public string? UserFeedback { get; set; }

не хочется чтобы все в кашу было

#### **ComponentBase**
EstimatedDurationMinutes - бред убрать
MaxAttempts - у нас нет попыток
MinPassingScore -  у нас нет проходного балла

ArticleComponent: 
ReadingTimeMinutes - свойство должно вычисляться просто на основе content. не надо заставлять пользователя просить вводить это

GetTotalScore - это интересно. мне кажется это должно быть как-то более системно. вроде у нас есть компоненты за которые мы получаем score и за которые не получаем. Иногда фиксированный иногда в зависимости от ответа. Я думаю эта логика должна быть хорошо определена в ComponentBase. А GetTotalScore просто будет его будет переопределять с логикой подсчёта score

QuestionOption:
Points заменить на Score тогда уж чтобы консистентно было

TaskComponent:
Instruction дублирует Instruction из ComponentBase
не нравится. реши как-то этот вопрос

FlowVersion упразднить как и все version

ComponentProgress:
удалить. зачем мы считаем прогресс по каждому компоненту, фигня какая-то
Это неуместно и излишне

Notifucation:
Metadata - удалить, ничего не храним в JSON
ReadAt: DateTime? - удалить, не нужно
ErrorMessage - убрать

**ComponentProgressData** - Данные прогресса компонента
бред убрать

**ComponentStatus** и StepStatus - Статус компонента заменить на is_enabled 

ProgressStatus добавить 4 отменён

NotificationChannel -  Email убрать, вообще не храним мэил и не будем

NotificationStatus - Статус уведомления
Read - удалить


"TaskSubmissionType - Тип подачи задания
CodeWord - кодовое слово
File - файл
Text - текст" - вот это всё чё за хуйня вообще пиздец. у нас компонент Task всегда принимает только кодовое слово. Это буквально его смысл. Пиздец

