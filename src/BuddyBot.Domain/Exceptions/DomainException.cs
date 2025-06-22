namespace BuddyBot.Domain.Exceptions;

/// <summary>
/// Базовый класс для всех доменных исключений
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>
    /// Код ошибки для идентификации
    /// </summary>
    public virtual string ErrorCode { get; }

    /// <summary>
    /// Дополнительные данные об ошибке
    /// </summary>
    public Dictionary<string, object> Details { get; }

    /// <summary>
    /// Создает новое доменное исключение
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="errorCode">Код ошибки</param>
    protected DomainException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
        Details = new Dictionary<string, object>();
    }

    /// <summary>
    /// Создает новое доменное исключение с вложенным исключением
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="errorCode">Код ошибки</param>
    /// <param name="innerException">Вложенное исключение</param>
    protected DomainException(string message, string errorCode, Exception innerException) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = new Dictionary<string, object>();
    }

    /// <summary>
    /// Добавляет дополнительную информацию об ошибке
    /// </summary>
    /// <param name="key">Ключ</param>
    /// <param name="value">Значение</param>
    /// <returns>Текущий экземпляр исключения</returns>
    public DomainException WithDetail(string key, object value)
    {
        Details[key] = value;
        return this;
    }

    /// <summary>
    /// Добавляет идентификатор сущности
    /// </summary>
    /// <param name="entityId">Идентификатор сущности</param>
    /// <returns>Текущий экземпляр исключения</returns>
    public DomainException WithEntityId(Guid entityId)
    {
        return WithDetail("EntityId", entityId);
    }

    /// <summary>
    /// Добавляет тип сущности
    /// </summary>
    /// <param name="entityType">Тип сущности</param>
    /// <returns>Текущий экземпляр исключения</returns>
    public DomainException WithEntityType(string entityType)
    {
        return WithDetail("EntityType", entityType);
    }
}