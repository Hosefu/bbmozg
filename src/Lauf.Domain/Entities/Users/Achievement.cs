using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Users;

/// <summary>
/// Достижение
/// </summary>
public class Achievement
{
    /// <summary>
    /// Идентификатор достижения
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название достижения
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание достижения
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }

    /// <summary>
    /// URL иконки достижения
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Активно ли достижение
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}