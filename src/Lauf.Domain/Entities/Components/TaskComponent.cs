using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Компонент задания (только кодовое слово)
/// </summary>
public class TaskComponent : ComponentBase
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public override ComponentType Type => ComponentType.Task;

    /// <summary>
    /// Правильный ответ (кодовое слово)
    /// </summary>
    public string CodeWord { get; private set; } = string.Empty;

    /// <summary>
    /// Учитывать ли регистр
    /// </summary>
    public bool IsCaseSensitive { get; private set; } = false;

    /// <summary>
    /// Конструктор для создания нового задания
    /// </summary>
    /// <param name="flowStepId">Идентификатор шага потока</param>
    /// <param name="title">Название задания</param>
    /// <param name="description">Описание задания (используется вместо Instruction)</param>
    /// <param name="content">Содержимое задания</param>
    /// <param name="codeWord">Кодовое слово</param>
    /// <param name="order">Порядковый номер компонента</param>
    /// <param name="isRequired">Обязательный ли компонент</param>
    /// <param name="isCaseSensitive">Учитывать ли регистр</param>
    public TaskComponent(Guid flowStepId, string title, string description, string content, string codeWord, 
        string order, bool isRequired = true, bool isCaseSensitive = false)
        : base(flowStepId, title, description, content, order, isRequired)
    {
        CodeWord = codeWord ?? throw new ArgumentNullException(nameof(codeWord));
        IsCaseSensitive = isCaseSensitive;
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected TaskComponent() { }

    /// <summary>
    /// Проверяет правильность ответа
    /// </summary>
    /// <param name="answer">Ответ пользователя</param>
    /// <returns>true, если ответ правильный</returns>
    public bool CheckAnswer(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer)) return false;
        
        var comparison = IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        return string.Equals(CodeWord.Trim(), answer.Trim(), comparison);
    }
}