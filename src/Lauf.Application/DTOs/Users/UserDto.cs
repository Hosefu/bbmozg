namespace Lauf.Application.DTOs.Users;

/// <summary>
/// DTO для пользователя
/// </summary>
public class UserDto
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор в Telegram
    /// </summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Username в Telegram
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Должность
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Департамент
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Активен ли пользователь
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Язык интерфейса
    /// </summary>
    public string Language { get; set; } = "ru";

    /// <summary>
    /// Часовой пояс
    /// </summary>
    public string? Timezone { get; set; }

    /// <summary>
    /// Роли пользователя
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Дата регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// Дата последней активности
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Отображаемое имя пользователя
    /// </summary>
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName => DisplayName;
}