using Lauf.Application.Commands.FlowManagement;
using FluentValidation;

namespace Lauf.Application.Validators;

/// <summary>
/// Валидатор для команды создания потока
/// </summary>
public class CreateFlowCommandValidator : AbstractValidator<CreateFlowCommand>
{
    public CreateFlowCommandValidator()
    {
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

        RuleFor(x => x.Category)
            .MaximumLength(100)
            .WithMessage("Категория не должна превышать 100 символов");

        RuleFor(x => x.CreatedById)
            .NotEmpty()
            .WithMessage("Идентификатор создателя обязателен");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 10)
            .WithMessage("Приоритет должен быть от 0 до 10");

        RuleFor(x => x.Tags)
            .Must(BeValidJson)
            .WithMessage("Теги должны быть в формате JSON массива");

        When(x => x.Settings != null, () =>
        {
            RuleFor(x => x.Settings!.MaxAttempts)
                .GreaterThan(0)
                .When(x => x.Settings!.MaxAttempts.HasValue)
                .WithMessage("Максимальное количество попыток должно быть больше 0");

            RuleFor(x => x.Settings!.TimeToCompleteWorkingDays)
                .GreaterThan(0)
                .When(x => x.Settings!.TimeToCompleteWorkingDays.HasValue)
                .WithMessage("Время на выполнение должно быть больше 0 дней");

            RuleFor(x => x.Settings!.AdditionalSettings)
                .Must(BeValidJson)
                .WithMessage("Дополнительные настройки должны быть в формате JSON");
        });
    }

    private static bool BeValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return true;

        try
        {
            System.Text.Json.JsonDocument.Parse(jsonString);
            return true;
        }
        catch
        {
            return false;
        }
    }
}