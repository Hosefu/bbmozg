using Lauf.Application.Commands.FlowAssignment;
using FluentValidation;

namespace Lauf.Application.Validators;

/// <summary>
/// Валидатор для команды назначения потока
/// </summary>
public class AssignFlowCommandValidator : AbstractValidator<AssignFlowCommand>
{
    public AssignFlowCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя обязателен");

        RuleFor(x => x.FlowId)
            .NotEmpty()
            .WithMessage("Идентификатор потока обязателен");

        RuleFor(x => x.CreatedById)
            .NotEmpty()
            .WithMessage("Идентификатор создателя назначения обязателен");

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.Deadline.HasValue)
            .WithMessage("Дедлайн должен быть в будущем");

        // Priority убран в новой архитектуре

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Заметки не должны превышать 1000 символов");

        RuleFor(x => x.BuddyId)
            .NotEqual(x => x.UserId)
            .When(x => x.BuddyId.HasValue)
            .WithMessage("Buddy не может быть тем же пользователем, что и исполнитель");
    }
}