using FluentValidation;
using Lauf.Application.Commands.Components;

namespace Lauf.Application.Validators.Components;

/// <summary>
/// Валидатор для команды создания компонента задания
/// </summary>
public class CreateTaskComponentCommandValidator : AbstractValidator<CreateTaskComponentCommand>
{
    public CreateTaskComponentCommandValidator()
    {
        RuleFor(x => x.FlowStepId)
            .NotEmpty()
            .WithMessage("Идентификатор шага потока обязателен");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название задания обязательно")
            .MaximumLength(200)
            .WithMessage("Название задания не должно превышать 200 символов");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание задания обязательно")
            .MaximumLength(1000)
            .WithMessage("Описание задания не должно превышать 1000 символов");

        RuleFor(x => x.Instruction)
            .NotEmpty()
            .WithMessage("Инструкция задания обязательна")
            .MaximumLength(2000)
            .WithMessage("Инструкция не должна превышать 2000 символов");

        RuleFor(x => x.CodeWord)
            .NotEmpty()
            .WithMessage("Кодовое слово обязательно")
            .MaximumLength(100)
            .WithMessage("Кодовое слово не должно превышать 100 символов");

        RuleFor(x => x.Hint)
            .NotEmpty()
            .WithMessage("Подсказка обязательна")
            .MaximumLength(1000)
            .WithMessage("Подсказка не должна превышать 1000 символов");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0)
            .WithMessage("Время выполнения должно быть больше 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Время выполнения не должно превышать 8 часов (480 минут)");

        RuleFor(x => x.Order)
            .GreaterThan(0)
            .WithMessage("Порядковый номер должен быть больше 0")
            .When(x => x.Order.HasValue);
    }
}