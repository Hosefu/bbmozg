using System.Text;

namespace Lauf.Shared.Helpers;

/// <summary>
/// Helper для работы с LexoRank - алгоритм динамического упорядочивания элементов
/// Основан на алгоритме JIRA LexoRank для эффективного управления порядком элементов
/// </summary>
public static class LexoRankHelper
{
    /// <summary>
    /// Размер алфавита (a-z)
    /// </summary>
    private const int ALPHABET_SIZE = 26;
    
    /// <summary>
    /// Начальная позиция алфавита
    /// </summary>
    private const char ALPHABET_START = 'a';
    
    /// <summary>
    /// Конечная позиция алфавита
    /// </summary>
    private const char ALPHABET_END = 'z';
    
    /// <summary>
    /// Средняя позиция алфавита
    /// </summary>
    private const char ALPHABET_MIDDLE = 'n';

    /// <summary>
    /// Минимальный LexoRank
    /// </summary>
    public static string Min() => "aaa";

    /// <summary>
    /// Максимальный LexoRank
    /// </summary>
    public static string Max() => "zzz";

    /// <summary>
    /// Средний LexoRank
    /// </summary>
    public static string Middle() => "nnn";

    /// <summary>
    /// Генерирует следующий LexoRank после данного
    /// </summary>
    /// <param name="lexoRank">Текущий LexoRank</param>
    /// <returns>Следующий LexoRank</returns>
    public static string Next(string lexoRank)
    {
        if (string.IsNullOrEmpty(lexoRank))
            return Min();

        return Between(lexoRank, null);
    }

    /// <summary>
    /// Генерирует предыдущий LexoRank перед данным
    /// </summary>
    /// <param name="lexoRank">Текущий LexoRank</param>
    /// <returns>Предыдущий LexoRank</returns>
    public static string Previous(string lexoRank)
    {
        if (string.IsNullOrEmpty(lexoRank))
            return Max();

        return Between(null, lexoRank);
    }

    /// <summary>
    /// Генерирует LexoRank между двумя позициями
    /// </summary>
    /// <param name="firstRank">Первая позиция (может быть null)</param>
    /// <param name="secondRank">Вторая позиция (может быть null)</param>
    /// <returns>LexoRank между двумя позициями</returns>
    public static string Between(string? firstRank, string? secondRank)
    {
        // Если обе позиции null, возвращаем средний
        if (string.IsNullOrEmpty(firstRank) && string.IsNullOrEmpty(secondRank))
            return Middle();

        // Если первая позиция null, генерируем позицию перед второй
        if (string.IsNullOrEmpty(firstRank))
        {
            return GenerateBefore(secondRank!);
        }

        // Если вторая позиция null, генерируем позицию после первой
        if (string.IsNullOrEmpty(secondRank))
        {
            return GenerateAfter(firstRank);
        }

        // Проверяем правильность порядка
        if (string.Compare(firstRank, secondRank, StringComparison.Ordinal) >= 0)
        {
            throw new ArgumentException($"First rank '{firstRank}' must be less than second rank '{secondRank}'");
        }

        return GenerateBetween(firstRank, secondRank);
    }

    /// <summary>
    /// Генерирует позицию перед заданной
    /// </summary>
    private static string GenerateBefore(string rank)
    {
        var chars = rank.ToCharArray();
        
        // Пытаемся уменьшить последний символ
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            if (chars[i] > ALPHABET_START)
            {
                chars[i]--;
                return new string(chars);
            }
            
            // Если символ уже минимальный, ставим максимальный и идем к предыдущему
            chars[i] = ALPHABET_END;
        }
        
        // Если все символы минимальные, добавляем символ в начало
        return ALPHABET_END + rank;
    }

    /// <summary>
    /// Генерирует позицию после заданной
    /// </summary>
    private static string GenerateAfter(string rank)
    {
        var chars = rank.ToCharArray();
        
        // Пытаемся увеличить последний символ
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            if (chars[i] < ALPHABET_END)
            {
                chars[i]++;
                return new string(chars);
            }
            
            // Если символ уже максимальный, ставим минимальный и идем к предыдущему
            chars[i] = ALPHABET_START;
        }
        
        // Если все символы максимальные, добавляем средний символ в конец
        return rank + ALPHABET_MIDDLE;
    }

    /// <summary>
    /// Генерирует позицию между двумя заданными
    /// </summary>
    private static string GenerateBetween(string firstRank, string secondRank)
    {
        // Выравниваем длины строк
        var maxLength = Math.Max(firstRank.Length, secondRank.Length);
        firstRank = firstRank.PadRight(maxLength, ALPHABET_START);
        secondRank = secondRank.PadRight(maxLength, ALPHABET_START);

        var difference = CalculateDifference(firstRank, secondRank);
        
        // Если разность меньше или равна 1, добавляем средний символ к первой позиции
        if (difference <= 1)
        {
            return firstRank + ALPHABET_MIDDLE;
        }

        // Вычисляем средний ранг
        var halfDifference = difference / 2;
        return AddToRank(firstRank, halfDifference);
    }

    /// <summary>
    /// Вычисляет разность между двумя позициями
    /// </summary>
    private static long CalculateDifference(string firstRank, string secondRank)
    {
        long difference = 0;
        
        for (int i = 0; i < firstRank.Length; i++)
        {
            var firstCode = firstRank[i] - ALPHABET_START;
            var secondCode = secondRank[i] - ALPHABET_START;
            
            var powerValue = (long)Math.Pow(ALPHABET_SIZE, firstRank.Length - i - 1);
            difference += (secondCode - firstCode) * powerValue;
        }
        
        return difference;
    }

    /// <summary>
    /// Добавляет значение к позиции
    /// </summary>
    private static string AddToRank(string rank, long value)
    {
        var result = new StringBuilder();
        var carry = 0L;
        
        for (int i = rank.Length - 1; i >= 0; i--)
        {
            var currentValue = rank[i] - ALPHABET_START;
            var digitValue = value % ALPHABET_SIZE;
            value /= ALPHABET_SIZE;
            
            var newValue = currentValue + digitValue + carry;
            carry = newValue / ALPHABET_SIZE;
            newValue %= ALPHABET_SIZE;
            
            result.Insert(0, (char)(ALPHABET_START + newValue));
        }
        
        // Если есть перенос, добавляем его в начало
        while (carry > 0)
        {
            var digit = carry % ALPHABET_SIZE;
            carry /= ALPHABET_SIZE;
            result.Insert(0, (char)(ALPHABET_START + digit));
        }
        
        return result.ToString();
    }

    /// <summary>
    /// Генерирует список равномерно распределенных LexoRank позиций
    /// </summary>
    /// <param name="count">Количество позиций</param>
    /// <returns>Список LexoRank позиций</returns>
    public static List<string> GenerateDefaultRanks(int count)
    {
        if (count <= 0)
            return new List<string>();

        if (count == 1)
            return new List<string> { Middle() };

        var ranks = new List<string>();
        var startPos = Min();
        var endPos = Max();
        
        // Вычисляем общую разность
        var totalDiff = CalculateDifference(startPos, endPos);
        var stepSize = totalDiff / (count + 1);
        
        var currentRank = startPos;
        for (int i = 0; i < count; i++)
        {
            currentRank = AddToRank(startPos, stepSize * (i + 1));
            ranks.Add(currentRank);
        }
        
        return ranks;
    }

    /// <summary>
    /// Проверяет корректность LexoRank
    /// </summary>
    /// <param name="lexoRank">LexoRank для проверки</param>
    /// <returns>true, если LexoRank корректен</returns>
    public static bool IsValid(string lexoRank)
    {
        if (string.IsNullOrEmpty(lexoRank))
            return false;

        return lexoRank.All(c => c >= ALPHABET_START && c <= ALPHABET_END);
    }
} 