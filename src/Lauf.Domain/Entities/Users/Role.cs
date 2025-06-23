namespace Lauf.Domain.Entities.Users;

/// <summary>
/// Роль пользователя в системе
/// </summary>
public class Role
{
    /// <summary>
    /// Уникальный идентификатор роли
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название роли
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание роли
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Активна ли роль
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Системная роль (не может быть удалена)
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Дата создания роли
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Дата последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Пользователи с данной ролью
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>
    /// Проверяет, может ли роль быть удалена
    /// </summary>
    /// <returns>true, если роль можно удалить</returns>
    public bool CanBeDeleted()
    {
        return !IsSystem && !Users.Any();
    }

    /// <summary>
    /// Активирует роль
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Деактивирует роль
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}