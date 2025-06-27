using Lauf.Application.Commands.FlowVersions;
using FluentValidation;
using System;

namespace Lauf.Application.Validators.FlowVersions;

/// <summary>
/// Валидатор для команды обновления версии потока
/// </summary>
public class UpdateFlowVersionCommandValidator : AbstractValidator<UpdateFlowVersionCommand>
{
    public UpdateFlowVersionCommandValidator()
    {
        RuleFor(x => x.FlowVersionId)
            .NotEmpty()
            .WithMessage("Идентификатор версии потока обязателен");

        RuleFor(x => x.UpdatedById)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя, выполняющего обновление, обязателен");

        RuleFor(x => x.Title)
            .MaximumLength(200)
            .WithMessage("Название потока не должно превышать 200 символов")
            .MinimumLength(3)
            .WithMessage("Название потока должно содержать минимум 3 символа")
            .When(x => !string.IsNullOrWhiteSpace(x.Title));

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Описание потока не должно превышать 2000 символов")
            .MinimumLength(10)
            .WithMessage("Описание потока должно содержать минимум 10 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Tags)
            .MaximumLength(1000)
            .WithMessage("Теги не должны превышать 1000 символов")
            .When(x => x.Tags != null);

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 10)
            .WithMessage("Приоритет должен быть от 0 до 10")
            .When(x => x.Priority.HasValue);
    }
}