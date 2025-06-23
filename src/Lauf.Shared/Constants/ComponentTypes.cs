namespace Lauf.Shared.Constants;

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
    /// Все типы компонентов для проверок
    /// </summary>
    public static readonly string[] AllTypes = 
    {
        Article,
        Task,
        Quiz
    };
    
    /// <summary>
    /// Типы компонентов, требующие активного взаимодействия
    /// </summary>
    public static readonly string[] InteractiveTypes = 
    {
        Task,
        Quiz,
    };
    
    /// <summary>
    /// Типы компонентов только для чтения/просмотра
    /// </summary>
    public static readonly string[] PassiveTypes = 
    {
        Article
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
        Task,
        Quiz
    };
}