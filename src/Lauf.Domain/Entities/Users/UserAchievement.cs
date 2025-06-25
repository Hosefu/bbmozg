namespace Lauf.Domain.Entities.Users;

/// <summary>
/// Достижение пользователя
/// </summary>
public class UserAchievement
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Идентификатор достижения
    /// </summary>
    public Guid AchievementId { get; set; }

    /// <summary>
    /// Дата получения достижения
    /// </summary>
    public DateTime EarnedAt { get; set; }

    /// <summary>
    /// Навигационное свойство к пользователю
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Навигационное свойство к достижению
    /// </summary>
    public Achievement? Achievement { get; set; }
}