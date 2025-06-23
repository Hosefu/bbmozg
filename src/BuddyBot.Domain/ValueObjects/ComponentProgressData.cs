using System.Text.Json;

namespace BuddyBot.Domain.ValueObjects;

/// <summary>
/// Value Object для хранения данных прогресса компонентов
/// </summary>
public record ComponentProgressData
{
    /// <summary>
    /// JSON-данные прогресса
    /// </summary>
    public string JsonData { get; }

    /// <summary>
    /// Конструктор с JSON-данными
    /// </summary>
    /// <param name="jsonData">JSON-строка с данными прогресса</param>
    public ComponentProgressData(string jsonData)
    {
        // Валидируем JSON
        if (!string.IsNullOrEmpty(jsonData))
        {
            try
            {
                JsonDocument.Parse(jsonData);
            }
            catch (JsonException ex)
            {
                throw new ArgumentException("Некорректный JSON", nameof(jsonData), ex);
            }
        }

        JsonData = jsonData ?? "{}";
    }

    /// <summary>
    /// Создать из объекта
    /// </summary>
    /// <param name="data">Объект данных</param>
    /// <returns>ComponentProgressData</returns>
    public static ComponentProgressData FromObject(object data)
    {
        var json = JsonSerializer.Serialize(data);
        return new ComponentProgressData(json);
    }

    /// <summary>
    /// Получить типизированные данные
    /// </summary>
    /// <typeparam name="T">Тип данных</typeparam>
    /// <returns>Десериализованные данные</returns>
    public T? GetData<T>() where T : class
    {
        if (string.IsNullOrEmpty(JsonData) || JsonData == "{}")
        {
            return null;
        }

        return JsonSerializer.Deserialize<T>(JsonData);
    }

    /// <summary>
    /// Получить значение поля
    /// </summary>
    /// <param name="fieldName">Имя поля</param>
    /// <returns>Значение поля или null</returns>
    public object? GetField(string fieldName)
    {
        if (string.IsNullOrEmpty(JsonData) || JsonData == "{}")
        {
            return null;
        }

        using var document = JsonDocument.Parse(JsonData);
        return document.RootElement.TryGetProperty(fieldName, out var property) 
            ? property.GetRawText() 
            : null;
    }

    /// <summary>
    /// Обновить данные прогресса
    /// </summary>
    /// <param name="updates">Обновления в виде словаря</param>
    /// <returns>Новый ComponentProgressData с обновленными данными</returns>
    public ComponentProgressData UpdateData(Dictionary<string, object> updates)
    {
        var currentData = new Dictionary<string, object>();

        // Парсим существующие данные
        if (!string.IsNullOrEmpty(JsonData) && JsonData != "{}")
        {
            using var document = JsonDocument.Parse(JsonData);
            foreach (var element in document.RootElement.EnumerateObject())
            {
                currentData[element.Name] = JsonSerializer.Deserialize<object>(element.Value.GetRawText());
            }
        }

        // Применяем обновления
        foreach (var update in updates)
        {
            currentData[update.Key] = update.Value;
        }

        var updatedJson = JsonSerializer.Serialize(currentData);
        return new ComponentProgressData(updatedJson);
    }

    /// <summary>
    /// Пустые данные прогресса
    /// </summary>
    public static ComponentProgressData Empty => new("{}");

    /// <summary>
    /// Неявное преобразование в строку
    /// </summary>
    public static implicit operator string(ComponentProgressData data) => data.JsonData;

    /// <summary>
    /// Явное преобразование из строки
    /// </summary>
    public static explicit operator ComponentProgressData(string json) => new(json);
}