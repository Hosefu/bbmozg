using Lauf.Application.Commands.ComponentVersions;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Enums;
using FluentValidation;
using System;

namespace Lauf.Application.Validators.ComponentVersions;

/// <summary>
/// Валидатор для команды создания новой версии компонента
/// </summary>
public class CreateComponentVersionCommandValidator : AbstractValidator<CreateComponentVersionCommand>
{
    public CreateComponentVersionCommandValidator()
    {
        RuleFor(x => x.OriginalComponentId)
            .NotEmpty()
            .WithMessage("Идентификатор оригинального компонента обязателен");

        RuleFor(x => x.StepVersionId)
            .NotEmpty()
            .WithMessage("Идентификатор версии этапа обязателен");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название компонента обязательно")
            .MaximumLength(200)
            .WithMessage("Название компонента не должно превышать 200 символов")
            .MinimumLength(3)
            .WithMessage("Название компонента должно содержать минимум 3 символа");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание компонента обязательно")
            .MaximumLength(1000)
            .WithMessage("Описание компонента не должно превышать 1000 символов")
            .MinimumLength(10)
            .WithMessage("Описание компонента должно содержать минимум 10 символов");

        RuleFor(x => x.ComponentType)
            .IsInEnum()
            .WithMessage("Недопустимый тип компонента");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок компонента должен быть неотрицательным");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Оценочное время выполнения должно быть больше 0 минут");

        RuleFor(x => x.MaxAttempts)
            .GreaterThan(0)
            .WithMessage("Максимальное количество попыток должно быть больше 0");

        RuleFor(x => x.MinPassingScore)
            .InclusiveBetween(0, 100)
            .WithMessage("Минимальный проходной балл должен быть от 0 до 100");

        RuleFor(x => x.Instructions)
            .MaximumLength(2000)
            .WithMessage("Инструкции не должны превышать 2000 символов");

        // Валидация специализированных данных
        When(x => x.ComponentType == ComponentType.Article, () =>
        {
            RuleFor(x => x.ArticleData)
                .NotNull()
                .WithMessage("Данные статьи обязательны для компонента типа Article");

            When(x => x.ArticleData != null, () =>
            {
                RuleFor(x => x.ArticleData!.Content)
                    .NotEmpty()
                    .WithMessage("Содержимое статьи обязательно")
                    .MaximumLength(50000)
                    .WithMessage("Содержимое статьи не должно превышать 50000 символов");

                RuleFor(x => x.ArticleData!.ReadingTimeMinutes)
                    .GreaterThan(0)
                    .WithMessage("Время чтения должно быть больше 0 минут");
            });
        });

        When(x => x.ComponentType == ComponentType.Quiz, () =>
        {
            RuleFor(x => x.QuizData)
                .NotNull()
                .WithMessage("Данные теста обязательны для компонента типа Quiz");

            When(x => x.QuizData != null, () =>
            {
                RuleFor(x => x.QuizData!.PassingScore)
                    .InclusiveBetween(0, 100)
                    .WithMessage("Проходной балл должен быть от 0 до 100");

                RuleFor(x => x.QuizData!.TimeLimitMinutes)
                    .GreaterThan(0)
                    .When(x => x.QuizData!.TimeLimitMinutes.HasValue)
                    .WithMessage("Ограничение по времени должно быть больше 0 минут");

                RuleFor(x => x.QuizData!.Options)
                    .NotEmpty()
                    .WithMessage("Тест должен содержать хотя бы один вариант ответа");

                RuleForEach(x => x.QuizData!.Options)
                    .SetValidator(new CreateQuizOptionDataValidator());
            });
        });

        When(x => x.ComponentType == ComponentType.Task, () =>
        {
            RuleFor(x => x.TaskData)
                .NotNull()
                .WithMessage("Данные задания обязательны для компонента типа Task");

            When(x => x.TaskData != null, () =>
            {
                RuleFor(x => x.TaskData!.Instructions)
                    .NotEmpty()
                    .WithMessage("Инструкции задания обязательны")
                    .MaximumLength(5000)
                    .WithMessage("Инструкции задания не должны превышать 5000 символов");

                RuleFor(x => x.TaskData!.SubmissionType)
                    .IsInEnum()
                    .WithMessage("Недопустимый тип подачи решения");

                RuleFor(x => x.TaskData!.MaxFileSize)
                    .GreaterThan(0)
                    .When(x => x.TaskData!.MaxFileSize.HasValue)
                    .WithMessage("Максимальный размер файла должен быть больше 0");

                RuleFor(x => x.TaskData!.AllowedFileTypes)
                    .MaximumLength(500)
                    .WithMessage("Разрешенные типы файлов не должны превышать 500 символов");

                RuleFor(x => x.TaskData!.AutoApprovalKeywords)
                    .MaximumLength(1000)
                    .WithMessage("Ключевые слова для автоодобрения не должны превышать 1000 символов");
            });
        });
    }
}

/// <summary>
/// Валидатор для данных варианта ответа в тесте
/// </summary>
public class CreateQuizOptionDataValidator : AbstractValidator<CreateQuizOptionData>
{
    public CreateQuizOptionDataValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст варианта ответа обязателен")
            .MaximumLength(500)
            .WithMessage("Текст варианта ответа не должен превышать 500 символов");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Количество баллов не может быть отрицательным");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок варианта ответа должен быть неотрицательным");

        RuleFor(x => x.Explanation)
            .MaximumLength(1000)
            .WithMessage("Объяснение не должно превышать 1000 символов");
    }
}