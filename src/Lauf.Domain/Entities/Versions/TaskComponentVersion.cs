using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Entities.Versions;

/// <summary>
/// Версия компонента задания
/// </summary>
public class TaskComponentVersion
{
    /// <summary>
    /// Идентификатор версии компонента (Foreign Key)
    /// </summary>
    public Guid ComponentVersionId { get; private set; }

    /// <summary>
    /// Подробные инструкции по выполнению задания
    /// </summary>
    [Required]
    public string Instructions { get; private set; } = string.Empty;

    /// <summary>
    /// Тип отправки результата
    /// </summary>
    public TaskSubmissionType SubmissionType { get; private set; } = TaskSubmissionType.Text;

    /// <summary>
    /// Максимальный размер файла в байтах (только для файловых заданий)
    /// </summary>
    public int? MaxFileSize { get; private set; }

    /// <summary>
    /// Разрешенные типы файлов (разделенные запятыми, например: "pdf,doc,docx")
    /// </summary>
    [StringLength(200)]
    public string? AllowedFileTypes { get; private set; }

    /// <summary>
    /// Требуется ли одобрение наставника
    /// </summary>
    public bool RequiresMentorApproval { get; private set; } = true;

    /// <summary>
    /// Ключевые слова для автоматического одобрения (разделенные запятыми)
    /// </summary>
    public string? AutoApprovalKeywords { get; private set; }

    /// <summary>
    /// Версия компонента, к которой принадлежит это задание
    /// </summary>
    public virtual ComponentVersion ComponentVersion { get; set; } = null!;

    /// <summary>
    /// Конструктор для EF Core
    /// </summary>
    protected TaskComponentVersion() { }

    /// <summary>
    /// Конструктор для создания версии задания
    /// </summary>
    public TaskComponentVersion(
        Guid componentVersionId,
        string instructions,
        TaskSubmissionType submissionType = TaskSubmissionType.Text,
        int? maxFileSize = null,
        string? allowedFileTypes = null,
        bool requiresMentorApproval = true,
        string? autoApprovalKeywords = null)
    {
        ComponentVersionId = componentVersionId;
        Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        SubmissionType = submissionType;
        MaxFileSize = maxFileSize;
        AllowedFileTypes = allowedFileTypes;
        RequiresMentorApproval = requiresMentorApproval;
        AutoApprovalKeywords = autoApprovalKeywords;

        ValidateTask();
    }

    /// <summary>
    /// Обновить настройки задания
    /// </summary>
    public void UpdateSettings(
        string instructions,
        TaskSubmissionType submissionType,
        int? maxFileSize,
        string? allowedFileTypes,
        bool requiresMentorApproval,
        string? autoApprovalKeywords)
    {
        Instructions = instructions ?? throw new ArgumentNullException(nameof(instructions));
        SubmissionType = submissionType;
        MaxFileSize = maxFileSize;
        AllowedFileTypes = allowedFileTypes;
        RequiresMentorApproval = requiresMentorApproval;
        AutoApprovalKeywords = autoApprovalKeywords;

        ValidateTask();
    }

    /// <summary>
    /// Валидация настроек задания
    /// </summary>
    private void ValidateTask()
    {
        if (string.IsNullOrWhiteSpace(Instructions))
        {
            throw new ArgumentException("Инструкции по заданию не могут быть пустыми", nameof(Instructions));
        }

        // Валидация для файловых заданий
        if (SubmissionType == TaskSubmissionType.File)
        {
            if (MaxFileSize.HasValue)
            {
                if (MaxFileSize.Value <= 0)
                {
                    throw new ArgumentException("Максимальный размер файла должен быть больше 0", nameof(MaxFileSize));
                }

                if (MaxFileSize.Value > 100 * 1024 * 1024) // 100 МБ максимум
                {
                    throw new ArgumentException("Максимальный размер файла не может превышать 100 МБ", nameof(MaxFileSize));
                }
            }

            if (!string.IsNullOrEmpty(AllowedFileTypes))
            {
                ValidateFileTypes();
            }
        }
        else
        {
            // Для не-файловых заданий файловые настройки должны быть пустыми
            if (MaxFileSize.HasValue || !string.IsNullOrEmpty(AllowedFileTypes))
            {
                throw new ArgumentException("Настройки файлов применимы только к файловым заданиям");
            }
        }

        // Валидация ключевых слов автоодобрения
        if (!string.IsNullOrEmpty(AutoApprovalKeywords))
        {
            ValidateAutoApprovalKeywords();
        }
    }

    /// <summary>
    /// Валидация разрешенных типов файлов
    /// </summary>
    private void ValidateFileTypes()
    {
        if (string.IsNullOrEmpty(AllowedFileTypes))
            return;

        var types = AllowedFileTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(t => t.Trim().ToLowerInvariant())
                                   .ToArray();

        if (types.Length == 0)
        {
            throw new ArgumentException("Список разрешенных типов файлов не может быть пустым", nameof(AllowedFileTypes));
        }

        // Проверяем, что все типы файлов корректны
        var validExtensions = new[] { "pdf", "doc", "docx", "txt", "rtf", "odt", "jpg", "jpeg", "png", "gif", "zip", "rar" };
        var invalidTypes = types.Where(t => !validExtensions.Contains(t)).ToArray();

        if (invalidTypes.Any())
        {
            throw new ArgumentException($"Недопустимые типы файлов: {string.Join(", ", invalidTypes)}", nameof(AllowedFileTypes));
        }

        if (types.Length > 10)
        {
            throw new ArgumentException("Нельзя указать более 10 типов файлов", nameof(AllowedFileTypes));
        }
    }

    /// <summary>
    /// Валидация ключевых слов для автоодобрения
    /// </summary>
    private void ValidateAutoApprovalKeywords()
    {
        if (string.IsNullOrEmpty(AutoApprovalKeywords))
            return;

        var keywords = AutoApprovalKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(k => k.Trim())
                                          .ToArray();

        if (keywords.Length > 20)
        {
            throw new ArgumentException("Нельзя указать более 20 ключевых слов для автоодобрения", nameof(AutoApprovalKeywords));
        }

        if (keywords.Any(k => k.Length < 3))
        {
            throw new ArgumentException("Ключевые слова для автоодобрения должны содержать не менее 3 символов", nameof(AutoApprovalKeywords));
        }
    }

    /// <summary>
    /// Получить разрешенные типы файлов как массив
    /// </summary>
    public string[] GetAllowedFileTypesArray()
    {
        if (string.IsNullOrEmpty(AllowedFileTypes))
            return Array.Empty<string>();

        return AllowedFileTypes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim().ToLowerInvariant())
                              .ToArray();
    }

    /// <summary>
    /// Получить ключевые слова автоодобрения как массив
    /// </summary>
    public string[] GetAutoApprovalKeywordsArray()
    {
        if (string.IsNullOrEmpty(AutoApprovalKeywords))
            return Array.Empty<string>();

        return AutoApprovalKeywords.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(k => k.Trim())
                                  .ToArray();
    }

    /// <summary>
    /// Проверить, разрешен ли данный тип файла
    /// </summary>
    public bool IsFileTypeAllowed(string fileExtension)
    {
        if (SubmissionType != TaskSubmissionType.File)
            return false;

        if (string.IsNullOrEmpty(AllowedFileTypes))
            return true; // Если типы не указаны, разрешены все

        var extension = fileExtension.TrimStart('.').ToLowerInvariant();
        var allowedTypes = GetAllowedFileTypesArray();

        return allowedTypes.Contains(extension);
    }

    /// <summary>
    /// Проверить, подходит ли размер файла
    /// </summary>
    public bool IsFileSizeValid(long fileSizeBytes)
    {
        if (SubmissionType != TaskSubmissionType.File)
            return true;

        if (!MaxFileSize.HasValue)
            return true; // Если лимит не установлен, размер не ограничен

        return fileSizeBytes <= MaxFileSize.Value;
    }

    /// <summary>
    /// Проверить, может ли задание быть автоматически одобрено на основе текста ответа
    /// </summary>
    public bool CanBeAutoApproved(string submissionText)
    {
        if (RequiresMentorApproval)
            return false;

        if (string.IsNullOrEmpty(AutoApprovalKeywords) || string.IsNullOrEmpty(submissionText))
            return false;

        var keywords = GetAutoApprovalKeywordsArray();
        var lowerSubmissionText = submissionText.ToLowerInvariant();

        // Проверяем, содержит ли текст хотя бы одно из ключевых слов
        return keywords.Any(keyword => lowerSubmissionText.Contains(keyword.ToLowerInvariant()));
    }

    /// <summary>
    /// Получить человекочитаемое описание типа отправки
    /// </summary>
    public string GetSubmissionTypeDescription()
    {
        return SubmissionType switch
        {
            TaskSubmissionType.Text => "Текстовый ответ",
            TaskSubmissionType.File => "Загрузка файла",
            TaskSubmissionType.Link => "Ссылка на ресурс",
            _ => "Неизвестный тип"
        };
    }

    /// <summary>
    /// Получить подробную информацию о требованиях к файлу
    /// </summary>
    public string GetFileRequirementsDescription()
    {
        if (SubmissionType != TaskSubmissionType.File)
            return string.Empty;

        var requirements = new System.Text.StringBuilder();

        if (MaxFileSize.HasValue)
        {
            var sizeMB = MaxFileSize.Value / (1024.0 * 1024.0);
            requirements.AppendLine($"Максимальный размер: {sizeMB:F1} МБ");
        }

        if (!string.IsNullOrEmpty(AllowedFileTypes))
        {
            var types = string.Join(", ", GetAllowedFileTypesArray());
            requirements.AppendLine($"Разрешенные типы: {types}");
        }

        return requirements.ToString().Trim();
    }
}