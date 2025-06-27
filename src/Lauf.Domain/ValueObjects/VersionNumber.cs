using System;

namespace Lauf.Domain.ValueObjects;

/// <summary>
/// Value Object для номера версии
/// </summary>
public readonly struct VersionNumber : IEquatable<VersionNumber>, IComparable<VersionNumber>
{
    /// <summary>
    /// Номер версии (начинается с 1)
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Конструктор для создания номера версии
    /// </summary>
    /// <param name="value">Номер версии</param>
    /// <exception cref="ArgumentException">Если номер версии меньше 1</exception>
    public VersionNumber(int value)
    {
        if (value < 1)
            throw new ArgumentException("Номер версии должен быть больше 0", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Создать первую версию
    /// </summary>
    public static VersionNumber Initial => new(1);

    /// <summary>
    /// Получить следующую версию
    /// </summary>
    public VersionNumber Next() => new(Value + 1);

    /// <summary>
    /// Получить предыдущую версию
    /// </summary>
    /// <exception cref="InvalidOperationException">Если это уже первая версия</exception>
    public VersionNumber Previous()
    {
        if (Value <= 1)
            throw new InvalidOperationException("Нет предыдущей версии для версии 1");

        return new VersionNumber(Value - 1);
    }

    /// <summary>
    /// Проверить, является ли это первой версией
    /// </summary>
    public bool IsInitial => Value == 1;

    /// <summary>
    /// Неявное преобразование из int
    /// </summary>
    public static implicit operator VersionNumber(int value) => new(value);

    /// <summary>
    /// Неявное преобразование в int
    /// </summary>
    public static implicit operator int(VersionNumber version) => version.Value;

    /// <summary>
    /// Оператор равенства
    /// </summary>
    public static bool operator ==(VersionNumber left, VersionNumber right) => left.Equals(right);

    /// <summary>
    /// Оператор неравенства
    /// </summary>
    public static bool operator !=(VersionNumber left, VersionNumber right) => !left.Equals(right);

    /// <summary>
    /// Оператор больше
    /// </summary>
    public static bool operator >(VersionNumber left, VersionNumber right) => left.Value > right.Value;

    /// <summary>
    /// Оператор меньше
    /// </summary>
    public static bool operator <(VersionNumber left, VersionNumber right) => left.Value < right.Value;

    /// <summary>
    /// Оператор больше или равно
    /// </summary>
    public static bool operator >=(VersionNumber left, VersionNumber right) => left.Value >= right.Value;

    /// <summary>
    /// Оператор меньше или равно
    /// </summary>
    public static bool operator <=(VersionNumber left, VersionNumber right) => left.Value <= right.Value;

    /// <summary>
    /// Оператор инкремента
    /// </summary>
    public static VersionNumber operator ++(VersionNumber version) => version.Next();

    /// <summary>
    /// Оператор декремента
    /// </summary>
    public static VersionNumber operator --(VersionNumber version) => version.Previous();

    /// <summary>
    /// Проверка на равенство
    /// </summary>
    public bool Equals(VersionNumber other) => Value == other.Value;

    /// <summary>
    /// Проверка на равенство с объектом
    /// </summary>
    public override bool Equals(object? obj) => obj is VersionNumber other && Equals(other);

    /// <summary>
    /// Получить хеш-код
    /// </summary>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Сравнение с другой версией
    /// </summary>
    public int CompareTo(VersionNumber other) => Value.CompareTo(other.Value);

    /// <summary>
    /// Строковое представление
    /// </summary>
    public override string ToString() => $"v{Value}";

    /// <summary>
    /// Парсинг строки в номер версии
    /// </summary>
    /// <param name="value">Строка для парсинга (например, "v1", "1", "version 2")</param>
    /// <returns>Номер версии</returns>
    /// <exception cref="ArgumentException">Если строка не может быть распознана как номер версии</exception>
    public static VersionNumber Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Строка версии не может быть пустой", nameof(value));

        // Убираем префиксы типа "v", "version ", и т.д.
        var cleanValue = value.ToLowerInvariant()
            .Replace("v", "")
            .Replace("version", "")
            .Trim();

        if (int.TryParse(cleanValue, out var intValue))
        {
            return new VersionNumber(intValue);
        }

        throw new ArgumentException($"Не удалось распознать номер версии из строки: {value}", nameof(value));
    }

    /// <summary>
    /// Попытка парсинга строки в номер версии
    /// </summary>
    /// <param name="value">Строка для парсинга</param>
    /// <param name="version">Результат парсинга</param>
    /// <returns>true, если парсинг успешен</returns>
    public static bool TryParse(string? value, out VersionNumber version)
    {
        version = default;

        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            version = Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }
}