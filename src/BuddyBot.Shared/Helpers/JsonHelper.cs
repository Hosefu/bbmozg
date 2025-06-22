using System.Text.Json;
using System.Text.Json.Serialization;

namespace BuddyBot.Shared.Helpers;

/// <summary>
/// Вспомогательный класс для работы с JSON
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Настройки JSON сериализации по умолчанию
    /// </summary>
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
    
    /// <summary>
    /// Настройки JSON для красивого форматирования
    /// </summary>
    public static readonly JsonSerializerOptions PrettyOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
    
    /// <summary>
    /// Сериализует объект в JSON строку
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="value">Объект для сериализации</param>
    /// <param name="options">Настройки сериализации</param>
    /// <returns>JSON строка</returns>
    public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Serialize(value, options ?? DefaultOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка сериализации объекта типа {typeof(T).Name}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Безопасная сериализация объекта в JSON строку
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="value">Объект для сериализации</param>
    /// <param name="defaultValue">Значение по умолчанию при ошибке</param>
    /// <param name="options">Настройки сериализации</param>
    /// <returns>JSON строка или значение по умолчанию</returns>
    public static string SerializeSafe<T>(T value, string defaultValue = "{}", JsonSerializerOptions? options = null)
    {
        try
        {
            return value == null ? "null" : JsonSerializer.Serialize(value, options ?? DefaultOptions);
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Десериализует JSON строку в объект
    /// </summary>
    /// <typeparam name="T">Тип целевого объекта</typeparam>
    /// <param name="json">JSON строка</param>
    /// <param name="options">Настройки десериализации</param>
    /// <returns>Десериализованный объект</returns>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;
            
        try
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Ошибка десериализации JSON в тип {typeof(T).Name}: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Безопасная десериализация JSON строки в объект
    /// </summary>
    /// <typeparam name="T">Тип целевого объекта</typeparam>
    /// <param name="json">JSON строка</param>
    /// <param name="defaultValue">Значение по умолчанию при ошибке</param>
    /// <param name="options">Настройки десериализации</param>
    /// <returns>Десериализованный объект или значение по умолчанию</returns>
    public static T DeserializeSafe<T>(string json, T defaultValue = default!, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
            return defaultValue;
            
        try
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions) ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Проверяет, является ли строка валидным JSON
    /// </summary>
    /// <param name="json">Проверяемая строка</param>
    /// <returns>true, если строка является валидным JSON</returns>
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;
            
        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Форматирует JSON строку с отступами
    /// </summary>
    /// <param name="json">Исходная JSON строка</param>
    /// <returns>Отформатированная JSON строка</returns>
    public static string PrettyPrint(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;
            
        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, PrettyOptions);
        }
        catch
        {
            return json;
        }
    }
    
    /// <summary>
    /// Минифицирует JSON строку (удаляет лишние пробелы)
    /// </summary>
    /// <param name="json">Исходная JSON строка</param>
    /// <returns>Минифицированная JSON строка</returns>
    public static string Minify(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return json;
            
        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, DefaultOptions);
        }
        catch
        {
            return json;
        }
    }
    
    /// <summary>
    /// Клонирует объект через JSON сериализацию/десериализацию
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="source">Исходный объект</param>
    /// <param name="options">Настройки JSON</param>
    /// <returns>Клон объекта</returns>
    public static T? Clone<T>(T source, JsonSerializerOptions? options = null)
    {
        if (source == null)
            return default;
            
        var json = Serialize(source, options);
        return Deserialize<T>(json, options);
    }
    
    /// <summary>
    /// Безопасное клонирование объекта
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="source">Исходный объект</param>
    /// <param name="defaultValue">Значение по умолчанию при ошибке</param>
    /// <param name="options">Настройки JSON</param>
    /// <returns>Клон объекта или значение по умолчанию</returns>
    public static T CloneSafe<T>(T source, T defaultValue = default!, JsonSerializerOptions? options = null)
    {
        try
        {
            return Clone(source, options) ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }
    
    /// <summary>
    /// Объединяет два JSON объекта (второй перезаписывает свойства первого)
    /// </summary>
    /// <param name="json1">Первый JSON объект</param>
    /// <param name="json2">Второй JSON объект</param>
    /// <returns>Объединенный JSON объект</returns>
    public static string MergeJson(string json1, string json2)
    {
        if (string.IsNullOrWhiteSpace(json1))
            return json2 ?? "{}";
        if (string.IsNullOrWhiteSpace(json2))
            return json1;
            
        try
        {
            using var doc1 = JsonDocument.Parse(json1);
            using var doc2 = JsonDocument.Parse(json2);
            
            var merged = new Dictionary<string, object?>();
            
            // Добавляем свойства из первого объекта
            foreach (var property in doc1.RootElement.EnumerateObject())
            {
                merged[property.Name] = GetJsonValue(property.Value);
            }
            
            // Перезаписываем свойствами из второго объекта
            foreach (var property in doc2.RootElement.EnumerateObject())
            {
                merged[property.Name] = GetJsonValue(property.Value);
            }
            
            return Serialize(merged);
        }
        catch
        {
            return json1;
        }
    }
    
    /// <summary>
    /// Получает значение из JsonElement
    /// </summary>
    /// <param name="element">JSON элемент</param>
    /// <returns>Значение объекта</returns>
    private static object? GetJsonValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.Deserialize<Dictionary<string, object?>>(),
            JsonValueKind.Array => element.Deserialize<object?[]>(),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var longValue) ? longValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => element.ToString()
        };
    }
    
    /// <summary>
    /// Извлекает значение по пути из JSON объекта
    /// </summary>
    /// <param name="json">JSON строка</param>
    /// <param name="path">Путь к свойству (например, "user.profile.name")</param>
    /// <returns>Значение или null если не найдено</returns>
    public static string? GetValueByPath(string json, string path)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(path))
            return null;
            
        try
        {
            using var document = JsonDocument.Parse(json);
            var current = document.RootElement;
            
            foreach (var segment in path.Split('.'))
            {
                if (!current.TryGetProperty(segment, out current))
                    return null;
            }
            
            return current.ValueKind == JsonValueKind.String 
                ? current.GetString() 
                : current.GetRawText();
        }
        catch
        {
            return null;
        }
    }
}