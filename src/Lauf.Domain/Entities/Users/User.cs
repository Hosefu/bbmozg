using Lauf.Domain.ValueObjects;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Domain.Entities.Users;

/// <summary>
/// Пользователь системы
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Username в Telegram
    /// </summary>
    public string? TelegramUsername { get; set; }

    /// <summary>
    /// Идентификатор пользователя в Telegram
    /// </summary>
    public TelegramUserId TelegramUserId { get; set; } = null!;

    /// <summary>
    /// Электронная почта
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Должность пользователя
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Язык интерфейса
    /// </summary>
    public string Language { get; set; } = "ru";

    /// <summary>
    /// Активен ли пользователь
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата создания аккаунта
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последней активности
    /// </summary>
    public DateTime? LastActiveAt { get; set; }

    /// <summary>
    /// Роли пользователя
    /// </summary>
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    /// <summary>
    /// Назначения потоков пользователю
    /// </summary>
    public virtual ICollection<FlowAssignment> FlowAssignments { get; set; } = new List<FlowAssignment>();

    /// <summary>
    /// Проверяет, имеет ли пользователь указанную роль
    /// </summary>
    /// <param name="roleName">Название роли</param>
    /// <returns>true, если роль присутствует</returns>
    public bool HasRole(string roleName)
    {
        return Roles.Any(r => r.Name == roleName);
    }

    /// <summary>
    /// Обновляет время последней активности
    /// </summary>
    public void UpdateLastActivity()
    {
        LastActiveAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Активирует пользователя
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивирует пользователя
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}