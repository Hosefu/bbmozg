using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Infrastructure.Persistence;

namespace Lauf.Infrastructure.Persistence.Repositories;

/// <summary>
/// Реализация паттерна Unit of Work (упрощенная версия для этапа 7)
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Репозитории (полная версия этап 8)
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;
    private IFlowRepository? _flowRepository;
    private IFlowAssignmentRepository? _flowAssignmentRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Репозиторий пользователей
    /// </summary>
    public IUserRepository Users => 
        _userRepository ??= new SimpleUserRepository(_context);

    /// <summary>
    /// Репозиторий ролей
    /// </summary>
    public IRoleRepository Roles => 
        _roleRepository ??= new RoleRepository(_context);

    /// <summary>
    /// Репозиторий потоков
    /// </summary>
    public IFlowRepository Flows => 
        _flowRepository ??= new FlowRepository(_context);

    /// <summary>
    /// Репозиторий назначений потоков
    /// </summary>
    public IFlowAssignmentRepository FlowAssignments => 
        _flowAssignmentRepository ??= new FlowAssignmentRepository(_context);

    /// <summary>
    /// Начать транзакцию
    /// </summary>
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Транзакция уже активна");
        }

        _transaction = await _context.BeginTransactionAsync(cancellationToken);
        return new TransactionWrapper(_transaction);
    }

    /// <summary>
    /// Сохранить изменения
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Проверить наличие изменений
    /// </summary>
    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }

    /// <summary>
    /// Отменить изменения
    /// </summary>
    public void DiscardChanges()
    {
        foreach (var entry in _context.ChangeTracker.Entries())
        {
            entry.State = EntityState.Detached;
        }
    }

    /// <summary>
    /// Освобождение ресурсов
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await _context.DisposeAsync();
    }

    /// <summary>
    /// Освобождение ресурсов (синхронная версия)
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Обертка для транзакции Entity Framework
/// </summary>
public class TransactionWrapper : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public TransactionWrapper(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Guid TransactionId => _transaction.TransactionId;
    public bool IsCompleted => _transaction.GetDbTransaction().Connection?.State != System.Data.ConnectionState.Open;

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}