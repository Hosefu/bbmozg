using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Components.Base;

/// <summary>
/// Базовый класс для снапшотов всех типов компонентов
/// </summary>
public abstract class ComponentSnapshotBase
{
    /// <summary>
    /// Идентификатор снапшота компонента
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Идентификатор оригинального компонента
    /// </summary>
    public Guid OriginalComponentId { get; protected set; }

    /// <summary>
    /// Тип компонента
    /// </summary>
    public abstract ComponentType Type { get; }

    /// <summary>
    /// Название компонента
    /// </summary>
    public string Title { get; protected set; } = string.Empty;

    /// <summary>
    /// Описание компонента
    /// </summary>
    public string Description { get; protected set; } = string.Empty;

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int Order { get; protected set; }

    /// <summary>
    /// Является ли компонент обязательным для завершения шага
    /// </summary>
    public bool IsRequired { get; protected set; }

    /// <summary>
    /// Расчетное время выполнения в минутах
    /// </summary>
    public int EstimatedMinutes { get; protected set; }

    /// <summary>
    /// Максимальное количество попыток (для квизов и заданий)
    /// </summary>
    public int? MaxAttempts { get; protected set; }

    /// <summary>
    /// Минимальный балл для прохождения (для квизов)
    /// </summary>
    public int? MinimumScore { get; protected set; }

    /// <summary>
    /// Дополнительные настройки компонента в JSON формате
    /// </summary>
    public string Settings { get; protected set; } = "{}";

    /// <summary>
    /// Дата создания снапшота
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Версия компонента на момент создания снапшота
    /// </summary>
    public int Version { get; protected set; }

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    protected ComponentSnapshotBase() { }

    /// <summary>
    /// Конструктор для создания снапшота компонента
    /// </summary>
    /// <param name="originalComponentId">ID оригинального компонента</param>
    /// <param name="title">Название компонента</param>
    /// <param name="description">Описание компонента</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Является ли обязательным</param>
    /// <param name="estimatedMinutes">Расчетное время в минутах</param>
    /// <param name="settings">Настройки компонента</param>
    /// <param name="version">Версия компонента</param>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    /// <param name="minimumScore">Минимальный балл для прохождения</param>
    protected ComponentSnapshotBase(
        Guid originalComponentId,
        string title,
        string description,
        int order,
        bool isRequired,
        int estimatedMinutes,
        string settings,
        int version,
        int? maxAttempts = null,
        int? minimumScore = null)
    {
        Id = Guid.NewGuid();
        OriginalComponentId = originalComponentId;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Order = order >= 0 ? order : throw new ArgumentException("Порядковый номер не может быть отрицательным");
        IsRequired = isRequired;
        EstimatedMinutes = estimatedMinutes >= 0 ? estimatedMinutes : throw new ArgumentException("Время не может быть отрицательным");
        Settings = settings ?? "{}";
        Version = version;
        MaxAttempts = maxAttempts > 0 ? maxAttempts : null;
        MinimumScore = minimumScore >= 0 ? minimumScore : null;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Получить серилизованное содержимое снапшота
    /// </summary>
    /// <returns>JSON-строка с содержимым</returns>
    public abstract string SerializeContent();

    /// <summary>
    /// Получить типизированное содержимое снапшота
    /// </summary>
    /// <typeparam name="T">Тип содержимого</typeparam>
    /// <returns>Десериализованное содержимое</returns>
    public abstract T? GetTypedContent<T>() where T : class;

    /// <summary>
    /// Проверить, имеет ли компонент ограничение на количество попыток
    /// </summary>
    public bool HasAttemptsLimit => MaxAttempts.HasValue && MaxAttempts.Value > 0;

    /// <summary>
    /// Проверить, требует ли компонент минимального балла
    /// </summary>
    public bool RequiresMinimumScore => MinimumScore.HasValue && MinimumScore.Value > 0;

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

    /// <summary>
    /// Проверить, можно ли выполнить еще одну попытку
    /// </summary>
    /// <param name="currentAttempts">Текущее количество попыток</param>
    public bool CanAttempt(int currentAttempts)
    {
        if (!HasAttemptsLimit)
        {
            return true; // Неограниченное количество попыток
        }

        return currentAttempts < MaxAttempts!.Value;
    }

    /// <summary>
    /// Проверить, достигнут ли минимальный проходной балл
    /// </summary>
    /// <param name="score">Текущий балл</param>
    public bool HasPassingScore(int? score)
    {
        if (!RequiresMinimumScore)
        {
            return true; // Нет требований к минимальному баллу
        }

        return score.HasValue && score.Value >= MinimumScore!.Value;
    }
}