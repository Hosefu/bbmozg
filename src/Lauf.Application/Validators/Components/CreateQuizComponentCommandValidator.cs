using FluentValidation;
using Lauf.Application.Commands.Components;

namespace Lauf.Application.Validators.Components;

/// <summary>
/// Валидатор для команды создания компонента квиза
/// </summary>
public class CreateQuizComponentCommandValidator : AbstractValidator<CreateQuizComponentCommand>
{
    public CreateQuizComponentCommandValidator()
    {
        RuleFor(x => x.FlowStepId)
            .NotEmpty()
            .WithMessage("Идентификатор шага потока обязателен");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название квиза обязательно")
            .MaximumLength(200)
            .WithMessage("Название квиза не должно превышать 200 символов");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание квиза обязательно")
            .MaximumLength(1000)
            .WithMessage("Описание квиза не должно превышать 1000 символов");

        RuleFor(x => x.QuestionText)
            .NotEmpty()
            .WithMessage("Текст вопроса обязателен")
            .MaximumLength(1000)
            .WithMessage("Текст вопроса не должен превышать 1000 символов");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Время выполнения должно быть больше 0")
            .LessThanOrEqualTo(60)
            .WithMessage("Время выполнения не должно превышать 60 минут");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Порядковый номер должен быть больше 0")
            .When(x => x.Order.HasValue);

        RuleFor(x => x.Options)
            .NotEmpty()
            .WithMessage("Квиз должен содержать варианты ответов")
            .Must(options => options.Count == 5)
            .WithMessage("Квиз должен содержать ровно 5 вариантов ответа");

        RuleFor(x => x.Options)
            .Must(HaveAtLeastOneCorrectAnswer)
            .WithMessage("Должен быть хотя бы один правильный ответ");

        RuleForEach(x => x.Options)
            .SetValidator(new CreateQuestionOptionValidator());
    }

    private static bool HaveAtLeastOneCorrectAnswer(List<CreateQuestionOptionDto> options)
    {
        return options.Any(o => o.IsCorrect);
    }
}

/// <summary>
/// Валидатор для варианта ответа
/// </summary>
public class CreateQuestionOptionValidator : AbstractValidator<CreateQuestionOptionDto>
{
    public CreateQuestionOptionValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Текст варианта ответа обязателен")
            .MaximumLength(500)
            .WithMessage("Текст варианта ответа не должен превышать 500 символов");

        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Сообщение при выборе варианта обязательно")
            .MaximumLength(1000)
            .WithMessage("Сообщение не должно превышать 1000 символов");

        RuleFor(x => x.Points)
            .GreaterThan(0)
            .WithMessage("Количество баллов должно быть больше 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Количество баллов не должно превышать 100");
    }
}