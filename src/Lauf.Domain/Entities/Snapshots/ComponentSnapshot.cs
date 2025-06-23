using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Snapshots;

/// <summary>
/// Снапшот компонента - неизменяемая копия компонента на момент назначения потока
/// </summary>
public class ComponentSnapshot
{
    /// <summary>
    /// Идентификатор снапшота компонента
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; private set; }

    /// <summary>
    /// Идентификатор снапшота шага
    /// </summary>
    public Guid StepSnapshotId { get; private set; }

    /// <summary>
    /// Снапшот шага
    /// </summary>
    public FlowStepSnapshot StepSnapshot { get; private set; } = null!;

    /// <summary>
    /// Тип компонента
    /// </summary>
    public ComponentType Type { get; private set; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Является ли компонент обязательным для завершения шага
    /// </summary>
    public bool IsRequired { get; private set; }

    /// <summary>
    /// Расчетное время выполнения в минутах
    /// </summary>
    public int EstimatedMinutes { get; private set; }

    /// <summary>
    /// Максимальное количество попыток (для квизов и заданий)
    /// </summary>
    public int? MaxAttempts { get; private set; }

    /// <summary>
    /// Минимальный балл для прохождения (для квизов)
    /// </summary>
    public int? MinimumScore { get; private set; }

    /// <summary>
    /// Содержимое компонента в JSON формате
    /// </summary>
    public string Content { get; private set; } = "{}";

    /// <summary>
    /// Дополнительные настройки компонента в JSON формате
    /// </summary>
    public string Settings { get; private set; } = "{}";

    /// <summary>
    /// Дата создания снапшота
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    private ComponentSnapshot() { }

    /// <summary>
    /// Конструктор для создания снапшота компонента
    /// </summary>
    /// <param name="originalComponentId">ID оригинального компонента</param>
    /// <param name="stepSnapshotId">ID снапшота шага</param>
    /// <param name="type">Тип компонента</param>
    /// <param name="title">Название компонента</param>
    /// <param name="description">Описание компонента</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Является ли обязательным</param>
    /// <param name="estimatedMinutes">Расчетное время в минутах</param>
    /// <param name="content">Содержимое компонента</param>
    /// <param name="settings">Настройки компонента</param>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    /// <param name="minimumScore">Минимальный балл для прохождения</param>
    public ComponentSnapshot(
        Guid originalComponentId,
        Guid stepSnapshotId,
        ComponentType type,
        string title,
        string description,
        int order,
        bool isRequired,
        int estimatedMinutes,
        string content,
        string settings,
        int? maxAttempts = null,
        int? minimumScore = null)
    {
        Id = Guid.NewGuid();
        OriginalComponentId = originalComponentId;
        StepSnapshotId = stepSnapshotId;
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Order = order >= 0 ? order : throw new ArgumentException("Порядковый номер не может быть отрицательным");
        IsRequired = isRequired;
        EstimatedMinutes = estimatedMinutes >= 0 ? estimatedMinutes : throw new ArgumentException("Время не может быть отрицательным");
        Content = content ?? "{}";
        Settings = settings ?? "{}";
        MaxAttempts = maxAttempts > 0 ? maxAttempts : null;
        MinimumScore = minimumScore >= 0 ? minimumScore : null;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверить, имеет ли компонент ограничение на количество попыток
    /// </summary>
    public bool HasAttemptsLimit => MaxAttempts.HasValue && MaxAttempts.Value > 0;

    /// <summary>
    /// Проверить, требует ли компонент минимального балла
    /// </summary>
    public bool RequiresMinimumScore => MinimumScore.HasValue && MinimumScore.Value > 0;

    /// <summary>
    /// Получить типизированное содержимое компонента
    /// </summary>
    /// <typeparam name="T">Тип содержимого</typeparam>
    /// <returns>Десериализованное содержимое</returns>
    public T? GetTypedContent<T>() where T : class
    {
        if (string.IsNullOrEmpty(Content) || Content == "{}")
        {
            return null;
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Content);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получить типизированные настройки компонента
    /// </summary>
    /// <typeparam name="T">Тип настроек</typeparam>
    /// <returns>Десериализованные настройки</returns>
    public T? GetTypedSettings<T>() where T : class
    {
        if (string.IsNullOrEmpty(Settings) || Settings == "{}")
        {
            return null;
        }

        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Settings);
        }
        catch
        {
            return null;
        }
    }
}