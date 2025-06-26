namespace Lauf.Domain.ValueObjects;

/// <summary>
/// Value Object для процента прогресса (0-100)
/// </summary>
public class ProgressPercentage
{
    /// <summary>
    /// Значение процента (0-100)
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Безпараметрический конструктор для Entity Framework
    /// </summary>
    public ProgressPercentage()
    {
        Value = 0;
    }

    /// <summary>
    /// Конструктор для создания процента прогресса
    /// </summary>
    /// <param name="value">Значение процента</param>
    /// <exception cref="ArgumentException">Если значение не в диапазоне 0-100</exception>
    public ProgressPercentage(decimal value)
    {
        if (value < 0 || value > 100)
        {
            throw new ArgumentException("Процент должен быть в диапазоне от 0 до 100", nameof(value));
        }

        Value = Math.Round(value, 2);
    }

    /// <summary>
    /// Создать процент из завершенных и общих элементов
    /// </summary>
    /// <param name="completed">Количество завершенных элементов</param>
    /// <param name="total">Общее количество элементов</param>
    /// <returns>Процент прогресса</returns>
    public static ProgressPercentage FromRatio(int completed, int total)
    {
        if (total <= 0)
        {
            return new ProgressPercentage(0);
        }

        var percentage = (decimal)completed / total * 100;
        return new ProgressPercentage(percentage);
    }

    /// <summary>
    /// Неявное преобразование в decimal
    /// </summary>
    public static implicit operator decimal(ProgressPercentage percentage) => percentage.Value;

    /// <summary>
    /// Явное преобразование из decimal
    /// </summary>
    public static explicit operator ProgressPercentage(decimal value) => new(value);

    /// <summary>
    /// Строковое представление
    /// </summary>
    public override string ToString() => $"{Value:F1}%";

    /// <summary>
    /// Сравнение по значению
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is ProgressPercentage other && Value == other.Value;
    }

    /// <summary>
    /// Хэш-код по значению
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Оператор равенства
    /// </summary>
    public static bool operator ==(ProgressPercentage? left, ProgressPercentage? right)
    {
        return EqualityComparer<ProgressPercentage>.Default.Equals(left, right);
    }

    /// <summary>
    /// Оператор неравенства
    /// </summary>
    public static bool operator !=(ProgressPercentage? left, ProgressPercentage? right)
    {
        return !(left == right);
    }
}