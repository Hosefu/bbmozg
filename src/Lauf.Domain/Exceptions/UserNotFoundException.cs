namespace Lauf.Domain.Exceptions;

/// <summary>
/// Исключение, выбрасываемое при отсутствии пользователя
/// </summary>
public class UserNotFoundException : DomainException
{
    /// <summary>
    /// Код ошибки
    /// </summary>
    public override string ErrorCode => "USER_NOT_FOUND";

    /// <summary>
    /// Создает новое исключение с идентификатором пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    public UserNotFoundException(Guid userId) 
        : base($"Пользователь с ID {userId} не найден", "USER_NOT_FOUND")
    {
        WithEntityId(userId).WithEntityType("User");
    }

    /// <summary>
    /// Создает новое исключение с Telegram ID пользователя
    /// </summary>
    /// <param name="telegramUserId">Telegram ID пользователя</param>
    public UserNotFoundException(long telegramUserId) 
        : base($"Пользователь с Telegram ID {telegramUserId} не найден", "USER_NOT_FOUND")
    {
        WithDetail("TelegramUserId", telegramUserId).WithEntityType("User");
    }

    /// <summary>
    /// Создает новое исключение с email пользователя
    /// </summary>
    /// <param name="email">Email пользователя</param>
    public UserNotFoundException(string email) 
        : base($"Пользователь с email {email} не найден", "USER_NOT_FOUND")
    {
        WithDetail("Email", email).WithEntityType("User");
    }

    /// <summary>
    /// Создает общее исключение о ненайденном пользователе
    /// </summary>
    public UserNotFoundException() 
        : base("Пользователь не найден", "USER_NOT_FOUND")
    {
        WithEntityType("User");
    }
}