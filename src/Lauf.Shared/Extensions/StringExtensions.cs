using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Lauf.Shared.Extensions;

/// <summary>
/// Методы расширения для работы со строками
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Проверяет, является ли строка null или пустой
    /// </summary>
    /// <param name="value">Проверяемая строка</param>
    /// <returns>true, если строка null или пустая</returns>
    public static bool IsNullOrEmpty(this string? value)
    {
        return string.IsNullOrEmpty(value);
    }
    
    /// <summary>
    /// Проверяет, является ли строка null, пустой или состоит только из пробелов
    /// </summary>
    /// <param name="value">Проверяемая строка</param>
    /// <returns>true, если строка null, пустая или состоит только из пробелов</returns>
    public static bool IsNullOrWhiteSpace(this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
    
    /// <summary>
    /// Обрезает строку до указанной длины, добавляя многоточие если необходимо
    /// </summary>
    /// <param name="value">Исходная строка</param>
    /// <param name="maxLength">Максимальная длина</param>
    /// <param name="ellipsis">Символы для обозначения обрезки (по умолчанию "...")</param>
    /// <returns>Обрезанная строка</returns>
    public static string Truncate(this string value, int maxLength, string ellipsis = "...")
    {
        if (value.IsNullOrEmpty() || maxLength < 0)
            return value ?? string.Empty;
            
        if (value.Length <= maxLength)
            return value;
            
        if (maxLength < ellipsis.Length)
            return value.Substring(0, maxLength);
            
        return value.Substring(0, maxLength - ellipsis.Length) + ellipsis;
    }
    
    /// <summary>
    /// Преобразует строку в Title Case (каждое слово с заглавной буквы)
    /// </summary>
    /// <param name="value">Исходная строка</param>
    /// <returns>Строка в Title Case</returns>
    public static string ToTitleCase(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value ?? string.Empty;
            
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }
    
    /// <summary>
    /// Удаляет HTML теги из строки
    /// </summary>
    /// <param name="value">Строка с HTML</param>
    /// <returns>Строка без HTML тегов</returns>
    public static string StripHtml(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return value ?? string.Empty;
            
        return Regex.Replace(value, "<.*?>", string.Empty);
    }
    
    /// <summary>
    /// Преобразует строку в slug (URL-friendly формат)
    /// </summary>
    /// <param name="value">Исходная строка</param>
    /// <returns>Slug строка</returns>
    public static string ToSlug(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return string.Empty;
            
        // Приводим к нижнему регистру
        value = value.ToLowerInvariant();
        
        // Заменяем кириллицу на латиницу (базовая транслитерация)
        value = TransliterateRussian(value);
        
        // Удаляем все символы кроме букв, цифр, пробелов и дефисов
        value = Regex.Replace(value, @"[^a-z0-9\s-]", "");
        
        // Заменяем множественные пробелы и дефисы на один дефис
        value = Regex.Replace(value, @"[\s-]+", "-");
        
        // Убираем дефисы в начале и конце
        return value.Trim('-');
    }
    
    /// <summary>
    /// Считает количество слов в строке
    /// </summary>
    /// <param name="value">Исходная строка</param>
    /// <returns>Количество слов</returns>
    public static int WordCount(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return 0;
            
        return value.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
    
    /// <summary>
    /// Маскирует часть строки символами замены
    /// </summary>
    /// <param name="value">Исходная строка</param>
    /// <param name="visibleStart">Количество видимых символов в начале</param>
    /// <param name="visibleEnd">Количество видимых символов в конце</param>
    /// <param name="maskChar">Символ маскировки</param>
    /// <returns>Замаскированная строка</returns>
    public static string Mask(this string value, int visibleStart = 2, int visibleEnd = 2, char maskChar = '*')
    {
        if (value.IsNullOrEmpty() || value.Length <= visibleStart + visibleEnd)
            return value ?? string.Empty;
            
        var start = value.Substring(0, visibleStart);
        var end = value.Substring(value.Length - visibleEnd);
        var maskLength = value.Length - visibleStart - visibleEnd;
        
        return start + new string(maskChar, maskLength) + end;
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным email адресом
    /// </summary>
    /// <param name="value">Проверяемая строка</param>
    /// <returns>true, если строка является валидным email</returns>
    public static bool IsValidEmail(this string value)
    {
        if (value.IsNullOrWhiteSpace())
            return false;
            
        const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(value, emailPattern);
    }
    
    /// <summary>
    /// Безопасно преобразует строку в int
    /// </summary>
    /// <param name="value">Строка для преобразования</param>
    /// <param name="defaultValue">Значение по умолчанию</param>
    /// <returns>Числовое значение или значение по умолчанию</returns>
    public static int ToIntSafe(this string value, int defaultValue = 0)
    {
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Базовая транслитерация русских символов в латинские
    /// </summary>
    /// <param name="value">Строка с кириллицей</param>
    /// <returns>Строка с латиницей</returns>
    private static string TransliterateRussian(string value)
    {
        var transliterationMap = new Dictionary<char, string>
        {
            {'а', "a"}, {'б', "b"}, {'в', "v"}, {'г', "g"}, {'д', "d"},
            {'е', "e"}, {'ё', "yo"}, {'ж', "zh"}, {'з', "z"}, {'и', "i"},
            {'й', "y"}, {'к', "k"}, {'л', "l"}, {'м', "m"}, {'н', "n"},
            {'о', "o"}, {'п', "p"}, {'р', "r"}, {'с', "s"}, {'т', "t"},
            {'у', "u"}, {'ф', "f"}, {'х', "h"}, {'ц', "ts"}, {'ч', "ch"},
            {'ш', "sh"}, {'щ', "sch"}, {'ъ', ""}, {'ы', "y"}, {'ь', ""},
            {'э', "e"}, {'ю', "yu"}, {'я', "ya"}
        };
        
        var result = new StringBuilder();
        foreach (var c in value)
        {
            if (transliterationMap.TryGetValue(c, out var replacement))
                result.Append(replacement);
            else
                result.Append(c);
        }
        
        return result.ToString();
    }
}