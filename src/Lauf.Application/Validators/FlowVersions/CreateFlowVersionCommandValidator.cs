using Lauf.Application.Commands.FlowVersions;
using FluentValidation;
using System;

namespace Lauf.Application.Validators.FlowVersions;

/// <summary>
/// Валидатор для команды создания новой версии потока
/// </summary>
public class CreateFlowVersionCommandValidator : AbstractValidator<CreateFlowVersionCommand>
{
    public CreateFlowVersionCommandValidator()
    {
        RuleFor(x => x.OriginalFlowId)
            .NotEmpty()
            .WithMessage("Идентификатор оригинального потока обязателен");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Название потока обязательно")
            .MaximumLength(200)
            .WithMessage("Название потока не должно превышать 200 символов")
            .MinimumLength(3)
            .WithMessage("Название потока должно содержать минимум 3 символа");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Описание потока обязательно")
            .MaximumLength(2000)
            .WithMessage("Описание потока не должно превышать 2000 символов")
            .MinimumLength(10)
            .WithMessage("Описание потока должно содержать минимум 10 символов");

        RuleFor(x => x.Tags)
            .MaximumLength(1000)
            .WithMessage("Теги не должны превышать 1000 символов");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 10)
            .WithMessage("Приоритет должен быть от 0 до 10");

        RuleFor(x => x.CreatedById)
            .NotEmpty()
            .WithMessage("Идентификатор создателя обязателен");
    }
}