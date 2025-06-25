using MediatR;
using Lauf.Application.DTOs.Users;
using Lauf.Application.DTOs.Flows;
using Lauf.Application.Commands.Users;
using Lauf.Application.Commands.Flows;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Api.GraphQL.Types;

namespace Lauf.Api.GraphQL.Resolvers;

/// <summary>
/// GraphQL Mutation resolver
/// </summary>
public class Mutation
{
    /// <summary>
    /// Создать пользователя
    /// </summary>
    public async Task<UserDto> CreateUser(
        [Service] IMediator mediator,
        CreateUserInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateUserCommand
        {
            TelegramUserId = input.TelegramId,
            Email = input.Email,
            FirstName = input.FullName.Split(' ').FirstOrDefault() ?? "",
            LastName = string.Join(" ", input.FullName.Split(' ').Skip(1)),
            Position = input.Position,
            Language = "ru"
        };

        var result = await mediator.Send(command, cancellationToken);
        return result;
    }

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    public async Task<UserDto> UpdateUser(
        [Service] IMediator mediator,
        UpdateUserInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserCommand
        {
            UserId = input.Id,
            Email = input.Email,
            FullName = input.FullName,
            Position = input.Position,
            IsActive = input.IsActive
        };

        var result = await mediator.Send(command, cancellationToken);
        return result;
    }

    /// <summary>
    /// Создать поток обучения
    /// </summary>
    public async Task<FlowDto> CreateFlow(
        [Service] IMediator mediator,
        CreateFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateFlowCommand
        {
            Title = input.Title,
            Description = input.Description,
            Settings = new CreateFlowSettingsCommand
            {
                RequireSequentialCompletion = input.IsSequential,
                AllowRetry = input.AllowRetry,
                TimeToCompleteWorkingDays = input.TimeLimit,
                MaxAttempts = input.PassingScore
            }
        };

        var result = await mediator.Send(command, cancellationToken);
        
        // Конвертируем результат в FlowDto
        return new FlowDto
        {
            Id = result.FlowId,
            Title = result.Title,
            Status = Lauf.Domain.Enums.FlowStatus.Draft // По умолчанию
        };
    }

    /// <summary>
    /// Обновить поток обучения
    /// </summary>
    public async Task<FlowDto> UpdateFlow(
        [Service] IMediator mediator,
        UpdateFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateFlowCommand
        {
            FlowId = input.Id,
            Title = input.Title,
            Description = input.Description,
            Status = input.Status
        };

        var result = await mediator.Send(command, cancellationToken);
        return result.Flow;
    }

    /// <summary>
    /// Назначить поток пользователю
    /// </summary>
    public async Task<AssignFlowCommandResult> AssignFlow(
        [Service] IMediator mediator,
        AssignFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new AssignFlowCommand
        {
            UserId = input.UserId,
            FlowId = input.FlowId,
            Deadline = input.DueDate,
            CreatedById = input.AssignedBy ?? Guid.Empty
        };

        var result = await mediator.Send(command, cancellationToken);
        return result;
    }

    /// <summary>
    /// Начать прохождение потока
    /// </summary>
    public async Task<FlowAssignmentDto> StartFlow(
        [Service] IMediator mediator,
        StartFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new StartFlowCommand
        {
            AssignmentId = input.AssignmentId
        };

        var result = await mediator.Send(command, cancellationToken);
        return result.Assignment;
    }

    /// <summary>
    /// Завершить прохождение потока
    /// </summary>
    public async Task<FlowAssignmentDto> CompleteFlow(
        [Service] IMediator mediator,
        CompleteFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new CompleteFlowCommand
        {
            AssignmentId = input.AssignmentId,
            CompletionNotes = input.CompletionNotes
        };

        var result = await mediator.Send(command, cancellationToken);
        return result.Assignment;
    }
}

/// <summary>
/// Input типы для мутаций
/// </summary>
public record CreateUserInput(
    long TelegramId,
    string Email,
    string FullName,
    string? Position);

public record UpdateUserInput(
    Guid Id,
    string? Email,
    string? FullName,
    string? Position,
    bool? IsActive);

public record CreateFlowInput(
    string Title,
    string Description,
    bool IsSequential = true,
    bool AllowRetry = true,
    int? TimeLimit = null,
    int? PassingScore = null);

public record UpdateFlowInput(
    Guid Id,
    string? Title,
    string? Description,
    Domain.Enums.FlowStatus? Status);

public record AssignFlowInput(
    Guid UserId,
    Guid FlowId,
    DateTime? DueDate,
    Guid? AssignedBy);

public record StartFlowInput(
    Guid AssignmentId);

public record CompleteFlowInput(
    Guid AssignmentId,
    string? CompletionNotes);