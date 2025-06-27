using Lauf.Application.Commands.FlowVersions;
using FluentValidation;
using System;

namespace Lauf.Application.Validators.FlowVersions;

/// <summary>
/// Валидатор для команды активации версии потока
/// </summary>
public class ActivateFlowVersionCommandValidator : AbstractValidator<ActivateFlowVersionCommand>
{
    public ActivateFlowVersionCommandValidator()
    {
        RuleFor(x => x.FlowVersionId)
            .NotEmpty()
            .WithMessage("Идентификатор версии потока обязателен");

        RuleFor(x => x.ActivatedById)
            .NotEmpty()
            .WithMessage("Идентификатор пользователя, выполняющего активацию, обязателен");
    }
}