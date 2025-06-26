using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Components;

/// <summary>
/// Компонент практического задания - поиск кодового слова
/// </summary>
public class TaskComponent : ComponentBase
{
    /// <summary>
    /// Тип компонента
    /// </summary>
    public override ComponentType Type => ComponentType.Task;

    /// <summary>
    /// Инструкция - как найти кодовое слово
    /// </summary>
    public string Instruction { get; private set; } = string.Empty;

    /// <summary>
    /// Кодовое слово для проверки ответа
    /// </summary>
    public string CodeWord { get; private set; } = string.Empty;

    /// <summary>
    /// Подсказка, доступная в любой момент
    /// </summary>
    public string Hint { get; private set; } = string.Empty;

    /// <summary>
    /// Конструктор для создания нового задания
    /// </summary>
    /// <param name="title">Название задания</param>
    /// <param name="description">Описание задания</param>
    /// <param name="instruction">Инструкция как найти кодовое слово</param>
    /// <param name="codeWord">Кодовое слово</param>
    /// <param name="hint">Подсказка</param>
    /// <param name="estimatedDurationMinutes">Приблизительное время выполнения</param>
    public TaskComponent(string title, string description, string instruction, 
        string codeWord, string hint, int estimatedDurationMinutes = 30)
        : base(title, description, estimatedDurationMinutes)
    {
        Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
        CodeWord = codeWord ?? throw new ArgumentNullException(nameof(codeWord));
        Hint = hint ?? throw new ArgumentNullException(nameof(hint));
    }

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected TaskComponent() { }

    /// <summary>
    /// Обновляет инструкцию задания
    /// </summary>
    /// <param name="instruction">Новая инструкция</param>
    /// <param name="hint">Новая подсказка</param>
    public void UpdateInstruction(string instruction, string? hint = null)
    {
        Instruction = instruction ?? throw new ArgumentNullException(nameof(instruction));
        if (hint != null)
            Hint = hint;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Устанавливает кодовое слово
    /// </summary>
    /// <param name="codeWord">Новое кодовое слово</param>
    public void SetCodeWord(string codeWord)
    {
        CodeWord = codeWord ?? throw new ArgumentNullException(nameof(codeWord));
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет правильность ответа
    /// </summary>
    /// <param name="answer">Ответ пользователя</param>
    /// <returns>true, если ответ правильный</returns>
    public bool CheckAnswer(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer)) return false;
        return string.Equals(CodeWord.Trim(), answer.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Проверяет, может ли компонент быть активирован
    /// </summary>
    public override bool CanBeActivated()
    {
        return !string.IsNullOrWhiteSpace(Instruction) && 
               !string.IsNullOrWhiteSpace(CodeWord) &&
               !string.IsNullOrWhiteSpace(Hint);
    }
}