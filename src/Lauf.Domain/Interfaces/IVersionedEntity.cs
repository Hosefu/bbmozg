using System;

namespace Lauf.Domain.Interfaces;

/// <summary>
/// Интерфейс для версионируемых сущностей
/// </summary>
/// <typeparam name="TOriginal">Тип оригинальной сущности</typeparam>
public interface IVersionedEntity<TOriginal>
{
    /// <summary>
    /// Уникальный идентификатор версии
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Идентификатор оригинальной сущности
    /// </summary>
    Guid OriginalId { get; }

    /// <summary>
    /// Номер версии (начинается с 1)
    /// </summary>
    int Version { get; }

    /// <summary>
    /// Является ли данная версия активной (текущей)
    /// </summary>
    bool IsActive { get; }

    /// <summary>
    /// Дата создания версии
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// Дата последнего обновления версии
    /// </summary>
    DateTime UpdatedAt { get; }

    /// <summary>
    /// Создать новую версию на основе текущей
    /// </summary>
    TOriginal CreateNewVersion();
}