using FluentValidation;
using Lauf.Application.Commands.Components;

namespace Lauf.Application.Validators.Components;

/// <summary>
/// Валидатор команды создания компонента статьи
/// </summary>
public class CreateArticleComponentCommandValidator : AbstractValidator<CreateArticleComponentCommand>
{
    public CreateArticleComponentCommandValidator()
    {
        RuleFor(x => x.FlowStepId)
            .NotEmpty()
            .WithMessage("Идентификатор шага потока обязателен");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название статьи обязательно")
            .MaximumLength(200)
            .WithMessage("Название статьи не должно превышать 200 символов");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание статьи обязательно")
            .MaximumLength(1000)
            .WithMessage("Описание статьи не должно превышать 1000 символов");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Содержимое статьи обязательно")
            .MinimumLength(10)
            .WithMessage("Содержимое статьи должно содержать минимум 10 символов");

        RuleFor(x => x.ReadingTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Время чтения должно быть больше 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Время чтения не должно превышать 8 часов (480 минут)");


    }
}