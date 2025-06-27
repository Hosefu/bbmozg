using System;

namespace Lauf.Domain.ValueObjects;

/// <summary>
/// Value Object для идентификатора версионной сущности
/// Комбинирует ID оригинальной сущности с номером версии
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public readonly struct VersionedEntityId<T> : IEquatable<VersionedEntityId<T>>
{
    /// <summary>
    /// Идентификатор оригинальной сущности
    /// </summary>
    public Guid OriginalId { get; }

    /// <summary>
    /// Номер версии
    /// </summary>
    public VersionNumber Version { get; }

    /// <summary>
    /// Конструктор для создания версионного идентификатора
    /// </summary>
    /// <param name="originalId">ID оригинальной сущности</param>
    /// <param name="version">Номер версии</param>
    public VersionedEntityId(Guid originalId, VersionNumber version)
    {
        OriginalId = originalId;
        Version = version;
    }

    /// <summary>
    /// Конструктор для создания версионного идентификатора
    /// </summary>
    /// <param name="originalId">ID оригинальной сущности</param>
    /// <param name="version">Номер версии</param>
    public VersionedEntityId(Guid originalId, int version) : this(originalId, new VersionNumber(version))
    {
    }

    /// <summary>
    /// Создать идентификатор для первой версии
    /// </summary>
    /// <param name="originalId">ID оригинальной сущности</param>
    public static VersionedEntityId<T> Initial(Guid originalId) => new(originalId, VersionNumber.Initial);

    /// <summary>
    /// Получить идентификатор следующей версии
    /// </summary>
    public VersionedEntityId<T> Next() => new(OriginalId, Version.Next());

    /// <summary>
    /// Получить идентификатор предыдущей версии
    /// </summary>
    public VersionedEntityId<T> Previous() => new(OriginalId, Version.Previous());

    /// <summary>
    /// Проверить, является ли это первой версией
    /// </summary>
    public bool IsInitial => Version.IsInitial;

    /// <summary>
    /// Оператор равенства
    /// </summary>
    public static bool operator ==(VersionedEntityId<T> left, VersionedEntityId<T> right) => left.Equals(right);

    /// <summary>
    /// Оператор неравенства
    /// </summary>
    public static bool operator !=(VersionedEntityId<T> left, VersionedEntityId<T> right) => !left.Equals(right);

    /// <summary>
    /// Проверка на равенство
    /// </summary>
    public bool Equals(VersionedEntityId<T> other) => 
        OriginalId.Equals(other.OriginalId) && Version.Equals(other.Version);

    /// <summary>
    /// Проверка на равенство с объектом
    /// </summary>
    public override bool Equals(object? obj) => 
        obj is VersionedEntityId<T> other && Equals(other);

    /// <summary>
    /// Получить хеш-код
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(OriginalId, Version);

    /// <summary>
    /// Строковое представление
    /// </summary>
    public override string ToString() => $"{typeof(T).Name}:{OriginalId:N}:{Version}";

    /// <summary>
    /// Парсинг строки в версионный идентификатор
    /// </summary>
    /// <param name="value">Строка в формате "TypeName:OriginalId:Version"</param>
    /// <returns>Версионный идентификатор</returns>
    /// <exception cref="ArgumentException">Если строка не может быть распознана</exception>
    public static VersionedEntityId<T> Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Строка идентификатора не может быть пустой", nameof(value));

        var parts = value.Split(':');
        if (parts.Length != 3)
            throw new ArgumentException($"Неверный формат идентификатора: {value}. Ожидается format: TypeName:OriginalId:Version", nameof(value));

        var typeName = parts[0];
        var expectedTypeName = typeof(T).Name;
        if (!string.Equals(typeName, expectedTypeName, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Неверный тип сущности: {typeName}. Ожидается: {expectedTypeName}", nameof(value));

        if (!Guid.TryParse(parts[1], out var originalId))
            throw new ArgumentException($"Неверный формат GUID: {parts[1]}", nameof(value));

        if (!VersionNumber.TryParse(parts[2], out var version))
            throw new ArgumentException($"Неверный формат версии: {parts[2]}", nameof(value));

        return new VersionedEntityId<T>(originalId, version);
    }

    /// <summary>
    /// Попытка парсинга строки в версионный идентификатор
    /// </summary>
    /// <param name="value">Строка для парсинга</param>
    /// <param name="versionedId">Результат парсинга</param>
    /// <returns>true, если парсинг успешен</returns>
    public static bool TryParse(string? value, out VersionedEntityId<T> versionedId)
    {
        versionedId = default;

        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            versionedId = Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Создать компактное строковое представление (без имени типа)
    /// </summary>
    public string ToCompactString() => $"{OriginalId:N}:{Version}";

    /// <summary>
    /// Парсинг компактной строки в версионный идентификатор
    /// </summary>
    /// <param name="value">Строка в формате "OriginalId:Version"</param>
    /// <returns>Версионный идентификатор</returns>
    /// <exception cref="ArgumentException">Если строка не может быть распознана</exception>
    public static VersionedEntityId<T> ParseCompact(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Строка идентификатора не может быть пустой", nameof(value));

        var parts = value.Split(':');
        if (parts.Length != 2)
            throw new ArgumentException($"Неверный формат идентификатора: {value}. Ожидается format: OriginalId:Version", nameof(value));

        if (!Guid.TryParse(parts[0], out var originalId))
            throw new ArgumentException($"Неверный формат GUID: {parts[0]}", nameof(value));

        if (!VersionNumber.TryParse(parts[1], out var version))
            throw new ArgumentException($"Неверный формат версии: {parts[1]}", nameof(value));

        return new VersionedEntityId<T>(originalId, version);
    }
}