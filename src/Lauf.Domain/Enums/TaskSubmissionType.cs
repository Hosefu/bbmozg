namespace Lauf.Domain.Enums;

/// <summary>
/// Тип отправки результата задания
/// </summary>
public enum TaskSubmissionType
{
    /// <summary>
    /// Текстовый ответ
    /// </summary>
    Text = 0,
    
    /// <summary>
    /// Загрузка файла
    /// </summary>
    File = 1,
    
    /// <summary>
    /// Ссылка на внешний ресурс
    /// </summary>
    Link = 2
}