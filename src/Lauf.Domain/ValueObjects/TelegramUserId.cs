namespace Lauf.Domain.ValueObjects;

/// <summary>
/// Value Object для идентификатора пользователя Telegram
/// Обеспечивает типобезопасность и инкапсуляцию логики работы с Telegram ID
/// </summary>
public sealed class TelegramUserId : IEquatable<TelegramUserId>
{
    /// <summary>
    /// Числовое значение идентификатора Telegram
    /// </summary>
    public long Value { get; }

    /// <summary>
    /// Создает новый экземпляр TelegramUserId
    /// </summary>
    /// <param name="value">Числовое значение ID</param>
    /// <exception cref="ArgumentException">Выбрасывается при недопустимом значении</exception>
    public TelegramUserId(long value)
    {
        if (value <= 0)
            throw new ArgumentException("Telegram User ID должен быть положительным числом", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Создает TelegramUserId из строки
    /// </summary>
    /// <param name="value">Строковое представление ID</param>
    /// <returns>Экземпляр TelegramUserId</returns>
    /// <exception cref="ArgumentException">Выбрасывается при недопустимом значении</exception>
    public static TelegramUserId FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Значение не может быть пустым", nameof(value));

        if (!long.TryParse(value, out var numericValue))
            throw new ArgumentException("Значение должно быть числом", nameof(value));

        return new TelegramUserId(numericValue);
    }

    /// <summary>
    /// Создает TelegramUserId из целого числа
    /// </summary>
    /// <param name="value">Числовое значение</param>
    /// <returns>Экземпляр TelegramUserId</returns>
    public static TelegramUserId FromLong(long value) => new(value);

    /// <summary>
    /// Создает TelegramUserId из целого числа
    /// </summary>
    /// <param name="value">Числовое значение</param>
    /// <returns>Экземпляр TelegramUserId</returns>
    public static TelegramUserId FromInt(int value) => new(value);

    /// <summary>
    /// Проверяет, является ли ID ботом (по соглашению Telegram)
    /// Боты обычно имеют ID, заканчивающиеся на определенные цифры
    /// </summary>
    /// <returns>true, если это может быть бот</returns>
    public bool IsLikelyBot()
    {
        // Это упрощенная логика. В реальности определение ботов более сложное
        var valueString = Value.ToString();
        return valueString.EndsWith("bot", StringComparison.OrdinalIgnoreCase) ||
               valueString.Length > 10; // Боты часто имеют длинные ID
    }

    /// <summary>
    /// Проверяет, является ли ID допустимым для пользователя
    /// </summary>
    /// <returns>true, если ID допустим</returns>
    public bool IsValidUserRange()
    {
        // Telegram User ID находятся в определенном диапазоне
        return Value > 0 && Value < 10_000_000_000L; // 10 миллиардов
    }

    /// <summary>
    /// Возвращает строковое представление
    /// </summary>
    /// <returns>Строковое представление ID</returns>
    public override string ToString() => Value.ToString();

    /// <summary>
    /// Проверяет равенство объектов
    /// </summary>
    /// <param name="obj">Объект для сравнения</param>
    /// <returns>true, если объекты равны</returns>
    public override bool Equals(object? obj) => Equals(obj as TelegramUserId);

    /// <summary>
    /// Проверяет равенство с другим TelegramUserId
    /// </summary>
    /// <param name="other">Другой TelegramUserId</param>
    /// <returns>true, если объекты равны</returns>
    public bool Equals(TelegramUserId? other) => other is not null && Value == other.Value;

    /// <summary>
    /// Возвращает хеш-код объекта
    /// </summary>
    /// <returns>Хеш-код</returns>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Оператор равенства
    /// </summary>
    public static bool operator ==(TelegramUserId? left, TelegramUserId? right) =>
        EqualityComparer<TelegramUserId>.Default.Equals(left, right);

    /// <summary>
    /// Оператор неравенства
    /// </summary>
    public static bool operator !=(TelegramUserId? left, TelegramUserId? right) => !(left == right);

    /// <summary>
    /// Неявное преобразование в long
    /// </summary>
    public static implicit operator long(TelegramUserId telegramUserId) => telegramUserId.Value;

    /// <summary>
    /// Явное преобразование из long
    /// </summary>
    public static explicit operator TelegramUserId(long value) => new(value);

    /// <summary>
    /// Явное преобразование из int
    /// </summary>
    public static explicit operator TelegramUserId(int value) => new(value);

    /// <summary>
    /// Преобразование в строку (неявное)
    /// </summary>
    public static implicit operator string(TelegramUserId telegramUserId) => telegramUserId.ToString();
}