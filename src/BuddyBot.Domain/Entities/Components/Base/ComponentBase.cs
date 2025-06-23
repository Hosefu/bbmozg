using BuddyBot.Domain.Enums;

namespace BuddyBot.Domain.Entities.Components.Base;

/// <summary>
/// Базовый класс для всех типов компонентов контента
/// </summary>
public abstract class ComponentBase
{
    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid Id { get; protected set; }

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
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Версия компонента
    /// </summary>
    public int Version { get; protected set; }

    /// <summary>
    /// Приватный конструктор для EF Core
    /// </summary>
    protected ComponentBase() { }

    /// <summary>
    /// Конструктор для создания компонента
    /// </summary>
    /// <param name="title">Название компонента</param>
    /// <param name="description">Описание компонента</param>
    /// <param name="order">Порядковый номер</param>
    /// <param name="isRequired">Является ли обязательным</param>
    /// <param name="estimatedMinutes">Расчетное время в минутах</param>
    /// <param name="settings">Настройки компонента</param>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    /// <param name="minimumScore">Минимальный балл для прохождения</param>
    protected ComponentBase(
        string title,
        string description,
        int order,
        bool isRequired,
        int estimatedMinutes,
        string settings,
        int? maxAttempts = null,
        int? minimumScore = null)
    {
        Id = Guid.NewGuid();
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Order = order >= 0 ? order : throw new ArgumentException("Порядковый номер не может быть отрицательным");
        IsRequired = isRequired;
        EstimatedMinutes = estimatedMinutes >= 0 ? estimatedMinutes : throw new ArgumentException("Время не может быть отрицательным");
        Settings = settings ?? "{}";
        MaxAttempts = maxAttempts > 0 ? maxAttempts : null;
        MinimumScore = minimumScore >= 0 ? minimumScore : null;
        Version = 1;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновить базовую информацию компонента
    /// </summary>
    /// <param name="title">Новое название</param>
    /// <param name="description">Новое описание</param>
    /// <param name="estimatedMinutes">Новое расчетное время</param>
    /// <param name="settings">Новые настройки</param>
    public virtual void UpdateBasicInfo(
        string? title = null,
        string? description = null,
        int? estimatedMinutes = null,
        string? settings = null)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
        }

        if (description != null)
        {
            Description = description;
        }

        if (estimatedMinutes.HasValue && estimatedMinutes.Value >= 0)
        {
            EstimatedMinutes = estimatedMinutes.Value;
        }

        if (settings != null)
        {
            Settings = settings;
        }

        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Обновить настройки попыток и оценивания
    /// </summary>
    /// <param name="maxAttempts">Максимальное количество попыток</param>
    /// <param name="minimumScore">Минимальный балл</param>
    public void UpdateAttemptSettings(int? maxAttempts = null, int? minimumScore = null)
    {
        MaxAttempts = maxAttempts > 0 ? maxAttempts : null;
        MinimumScore = minimumScore >= 0 ? minimumScore : null;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Изменить порядок компонента
    /// </summary>
    /// <param name="newOrder">Новый порядковый номер</param>
    public void ChangeOrder(int newOrder)
    {
        if (newOrder >= 0)
        {
            Order = newOrder;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Переключить обязательность компонента
    /// </summary>
    public void ToggleRequired()
    {
        IsRequired = !IsRequired;
        UpdatedAt = DateTime.UtcNow;
        Version++;
    }

    /// <summary>
    /// Получить серилизованное содержимое компонента для создания снапшота
    /// </summary>
    /// <returns>JSON-строка с содержимым</returns>
    public abstract string SerializeContent();

    /// <summary>
    /// Валидировать содержимое компонента
    /// </summary>
    /// <returns>Список ошибок валидации</returns>
    public abstract List<string> ValidateContent();

    /// <summary>
    /// Создать копию компонента для снапшота
    /// </summary>
    /// <returns>Копия компонента</returns>
    public abstract ComponentBase CreateSnapshot();

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
}