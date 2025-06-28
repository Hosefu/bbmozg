using Lauf.Application.Commands.FlowManagement;
using FluentValidation;

namespace Lauf.Application.Validators;

/// <summary>
/// Валидатор для команды создания потока (новая архитектура)
/// </summary>
public class CreateFlowCommandValidator : AbstractValidator<CreateFlowCommand>
{
    public CreateFlowCommandValidator()
    {
        RuleFor(x => x.Name)
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

        RuleFor(x => x.Category)
            .MaximumLength(100)
            .WithMessage("Категория не должна превышать 100 символов");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 10)
            .WithMessage("Приоритет должен быть от 0 до 10");

        RuleFor(x => x.Tags)
            .Must(BeValidJson)
            .WithMessage("Теги должны быть в формате JSON массива");

        // Упрощенная валидация настроек в новой архитектуре
        When(x => x.Settings != null, () =>
        {
            RuleFor(x => x.Settings!.DaysPerStep)
                .GreaterThan(0)
                .WithMessage("Дней на шаг должно быть больше 0");
        });
    }

    private static bool BeValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return true;

        try
        {
            System.Text.Json.JsonSerializer.Deserialize<string[]>(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}