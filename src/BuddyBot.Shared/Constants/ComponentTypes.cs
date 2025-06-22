namespace BuddyBot.Shared.Constants;

/// <summary>
/// Константы типов компонентов в потоках обучения
/// </summary>
public static class ComponentTypes
{
    /// <summary>
    /// Статья - текстовый контент для чтения
    /// </summary>
    public const string Article = "Article";
    
    /// <summary>
    /// Задание - практическая задача для выполнения
    /// </summary>
    public const string Task = "Task";
    
    /// <summary>
    /// Тест - вопросы с вариантами ответов
    /// </summary>
    public const string Quiz = "Quiz";
    
    /// <summary>
    /// Видео - видеоконтент для просмотра
    /// </summary>
    public const string Video = "Video";
    
    /// <summary>
    /// Файл - документ для скачивания и изучения
    /// </summary>
    public const string File = "File";
    
    /// <summary>
    /// Ссылка - внешний ресурс для изучения
    /// </summary>
    public const string Link = "Link";
    
    /// <summary>
    /// Интерактивный модуль - сложный интерактивный компонент
    /// </summary>
    public const string Interactive = "Interactive";
    
    /// <summary>
    /// Все типы компонентов для проверок
    /// </summary>
    public static readonly string[] AllTypes = 
    {
        Article,
        Task,
        Quiz,
        Video,
        File,
        Link,
        Interactive
    };
    
    /// <summary>
    /// Типы компонентов, требующие активного взаимодействия
    /// </summary>
    public static readonly string[] InteractiveTypes = 
    {
        Task,
        Quiz,
        Interactive
    };
    
    /// <summary>
    /// Типы компонентов только для чтения/просмотра
    /// </summary>
    public static readonly string[] PassiveTypes = 
    {
        Article,
        Video,
        File,
        Link
    };
    
    /// <summary>
    /// Типы компонентов с оценкой (могут иметь баллы)
    /// </summary>
    public static readonly string[] GradableTypes = 
    {
        Task,
        Quiz
    };
    
    /// <summary>
    /// Типы компонентов с отслеживанием времени
    /// </summary>
    public static readonly string[] TimedTypes = 
    {
        Article,
        Video,
        Task,
        Quiz,
        Interactive
    };
}