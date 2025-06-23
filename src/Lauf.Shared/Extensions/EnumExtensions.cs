using System.ComponentModel;
using System.Reflection;

namespace Lauf.Shared.Extensions;

/// <summary>
/// Методы расширения для работы с перечислениями
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Получает описание перечисления из атрибута Description
    /// </summary>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Описание или название если описания нет</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
            return value.ToString();
            
        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
    
    /// <summary>
    /// Получает все значения перечисления
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Массив всех значений</returns>
    public static T[] GetAllValues<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>();
    }
    
    /// <summary>
    /// Получает все значения перечисления с их описаниями
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Словарь значение-описание</returns>
    public static Dictionary<T, string> GetAllValuesWithDescriptions<T>() where T : struct, Enum
    {
        return GetAllValues<T>().ToDictionary(value => value, value => value.GetDescription());
    }
    
    /// <summary>
    /// Проверяет, является ли значение валидным для данного перечисления
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="value">Проверяемое значение</param>
    /// <returns>true, если значение валидно</returns>
    public static bool IsValidEnumValue<T>(object value) where T : struct, Enum
    {
        return Enum.IsDefined(typeof(T), value);
    }
    
    /// <summary>
    /// Безопасное преобразование строки в перечисление
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="value">Строковое значение</param>
    /// <param name="defaultValue">Значение по умолчанию</param>
    /// <param name="ignoreCase">Игнорировать регистр</param>
    /// <returns>Значение перечисления или значение по умолчанию</returns>
    public static T ParseSafe<T>(string? value, T defaultValue, bool ignoreCase = true) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return defaultValue;
            
        return Enum.TryParse<T>(value, ignoreCase, out var result) ? result : defaultValue;
    }
    
    /// <summary>
    /// Безопасное преобразование числа в перечисление
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="value">Числовое значение</param>
    /// <param name="defaultValue">Значение по умолчанию</param>
    /// <returns>Значение перечисления или значение по умолчанию</returns>
    public static T ParseSafe<T>(int value, T defaultValue) where T : struct, Enum
    {
        return IsValidEnumValue<T>(value) ? (T)(object)value : defaultValue;
    }
    
    /// <summary>
    /// Получает следующее значение в перечислении (циклично)
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="value">Текущее значение</param>
    /// <returns>Следующее значение</returns>
    public static T GetNext<T>(this T value) where T : struct, Enum
    {
        var values = GetAllValues<T>();
        var currentIndex = Array.IndexOf(values, value);
        var nextIndex = (currentIndex + 1) % values.Length;
        return values[nextIndex];
    }
    
    /// <summary>
    /// Получает предыдущее значение в перечислении (циклично)
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <param name="value">Текущее значение</param>
    /// <returns>Предыдущее значение</returns>
    public static T GetPrevious<T>(this T value) where T : struct, Enum
    {
        var values = GetAllValues<T>();
        var currentIndex = Array.IndexOf(values, value);
        var previousIndex = (currentIndex - 1 + values.Length) % values.Length;
        return values[previousIndex];
    }
    
    /// <summary>
    /// Проверяет, содержит ли флаговое перечисление указанный флаг
    /// </summary>
    /// <typeparam name="T">Тип флагового перечисления</typeparam>
    /// <param name="value">Значение перечисления</param>
    /// <param name="flag">Проверяемый флаг</param>
    /// <returns>true, если флаг установлен</returns>
    public static bool HasFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var valueInt = Convert.ToInt64(value);
        var flagInt = Convert.ToInt64(flag);
        return (valueInt & flagInt) == flagInt;
    }
    
    /// <summary>
    /// Добавляет флаг к флаговому перечислению
    /// </summary>
    /// <typeparam name="T">Тип флагового перечисления</typeparam>
    /// <param name="value">Значение перечисления</param>
    /// <param name="flag">Добавляемый флаг</param>
    /// <returns>Новое значение с добавленным флагом</returns>
    public static T AddFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var valueInt = Convert.ToInt32(value);
        var flagInt = Convert.ToInt32(flag);
        return (T)(object)(valueInt | flagInt);
    }
    
    /// <summary>
    /// Удаляет флаг из флагового перечисления
    /// </summary>
    /// <typeparam name="T">Тип флагового перечисления</typeparam>
    /// <param name="value">Значение перечисления</param>
    /// <param name="flag">Удаляемый флаг</param>
    /// <returns>Новое значение без указанного флага</returns>
    public static T RemoveFlag<T>(this T value, T flag) where T : struct, Enum
    {
        var valueInt = Convert.ToInt32(value);
        var flagInt = Convert.ToInt32(flag);
        return (T)(object)(valueInt & ~flagInt);
    }
    
    /// <summary>
    /// Переключает флаг в флаговом перечислении
    /// </summary>
    /// <typeparam name="T">Тип флагового перечисления</typeparam>
    /// <param name="value">Значение перечисления</param>
    /// <param name="flag">Переключаемый флаг</param>
    /// <returns>Новое значение с переключенным флагом</returns>
    public static T ToggleFlag<T>(this T value, T flag) where T : struct, Enum
    {
        return value.HasFlag(flag) ? value.RemoveFlag(flag) : value.AddFlag(flag);
    }
    
    /// <summary>
    /// Получает все установленные флаги в флаговом перечислении
    /// </summary>
    /// <typeparam name="T">Тип флагового перечисления</typeparam>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Список установленных флагов</returns>
    public static IEnumerable<T> GetFlags<T>(this T value) where T : struct, Enum
    {
        return GetAllValues<T>().Where(flag => value.HasFlag(flag));
    }
    
    /// <summary>
    /// Получает количество значений в перечислении
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Количество значений</returns>
    public static int GetCount<T>() where T : struct, Enum
    {
        return GetAllValues<T>().Length;
    }
    
    /// <summary>
    /// Получает случайное значение из перечисления
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Случайное значение перечисления</returns>
    public static T GetRandom<T>() where T : struct, Enum
    {
        var values = GetAllValues<T>();
        var random = new Random();
        return values[random.Next(values.Length)];
    }
    
    /// <summary>
    /// Преобразует перечисление в список для выпадающих списков
    /// </summary>
    /// <typeparam name="T">Тип перечисления</typeparam>
    /// <returns>Список элементов для выпадающего списка</returns>
    public static List<SelectListItem<T>> ToSelectList<T>() where T : struct, Enum
    {
        return GetAllValues<T>()
            .Select(value => new SelectListItem<T>
            {
                Value = value,
                Text = value.GetDescription(),
                Selected = false
            })
            .ToList();
    }
    
    /// <summary>
    /// Элемент списка для выбора
    /// </summary>
    /// <typeparam name="T">Тип значения</typeparam>
    public class SelectListItem<T>
    {
        /// <summary>
        /// Значение элемента
        /// </summary>
        public T Value { get; set; } = default!;
        
        /// <summary>
        /// Отображаемый текст
        /// </summary>
        public string Text { get; set; } = string.Empty;
        
        /// <summary>
        /// Выбран ли элемент
        /// </summary>
        public bool Selected { get; set; }
    }
}