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
    /// Email пользователя
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Телефон пользователя
    /// </summary>
    public string? Phone { get; set; }

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
    /// Дата последней активности
    /// </summary>
    public DateTime? LastActivityAt { get; set; }

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Отображаемое имя (FullName или Username)
    /// </summary>
    public string DisplayName => !string.IsNullOrEmpty(FullName) ? FullName : Username ?? $"User{TelegramUserId}";
}