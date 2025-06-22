using BuddyBot.Domain.Interfaces.Repositories;

namespace BuddyBot.Domain.Interfaces;

/// <summary>
/// Интерфейс Unit of Work для управления транзакциями
/// Обеспечивает атомарность операций с базой данных
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Репозиторий пользователей
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Репозиторий потоков
    /// </summary>
    IFlowRepository Flows { get; }

    /// <summary>
    /// Репозиторий назначений потоков
    /// </summary>
    IFlowAssignmentRepository FlowAssignments { get; }

    /// <summary>
    /// Начинает новую транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Транзакция</returns>
    Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет все изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменяет все несохраненные изменения
    /// </summary>
    void DiscardChanges();

    /// <summary>
    /// Проверяет, есть ли несохраненные изменения
    /// </summary>
    /// <returns>true, если есть несохраненные изменения</returns>
    bool HasChanges();
}

/// <summary>
/// Интерфейс транзакции базы данных
/// </summary>
public interface ITransaction : IAsyncDisposable
{
    /// <summary>
    /// Подтверждает транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Откатывает транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Уникальный идентификатор транзакции
    /// </summary>
    Guid TransactionId { get; }

    /// <summary>
    /// Проверяет, завершена ли транзакция
    /// </summary>
    bool IsCompleted { get; }
}