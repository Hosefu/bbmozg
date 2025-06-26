using MediatR;
using Microsoft.AspNetCore.Authorization;
using Lauf.Application.DTOs.Users;
using Lauf.Application.DTOs.Flows;
using Lauf.Application.Commands.Users;
using Lauf.Application.Commands.Flows;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Application.Commands.FlowSteps;
using Lauf.Application.Commands.FlowComponents;
using Lauf.Application.Services.Interfaces;
using Lauf.Api.GraphQL.Types;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Enums;

namespace Lauf.Api.GraphQL.Resolvers;

/// <summary>
/// GraphQL Mutation resolver
/// </summary>
public class Mutation
{
    private readonly IMediator _mediator;
    private readonly ILogger<Mutation> _logger;

    public Mutation(IMediator mediator, ILogger<Mutation> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Создать пользователя
    /// </summary>
    public async Task<UserDto> CreateUser(
        [Service] IMediator mediator,
        CreateUserInput input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateUserCommand
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                TelegramUserId = input.TelegramUserId
            };

            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании пользователя");
            throw new GraphQLException("Не удалось создать пользователя");
        }
    }

    /// <summary>
    /// Обновить пользователя
    /// </summary>
    public async Task<UserDto> UpdateUser(
        [Service] IMediator mediator,
        UpdateUserInput input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new UpdateUserCommand
            {
                UserId = input.UserId,
                FirstName = input.FirstName,
                LastName = input.LastName,
                TelegramUserId = input.TelegramUserId,
                RoleIds = input.RoleIds
            };

            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении пользователя с ID {UserId}", input.UserId);
            throw new GraphQLException("Не удалось обновить пользователя");
        }
    }

    /// <summary>
    /// Создать поток обучения
    /// </summary>
    [Authorize]
    public async Task<FlowDto> CreateFlow(
        [Service] IMediator mediator,
        [Service] ICurrentUserService currentUserService,
        CreateFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated");
        
        var command = new CreateFlowCommand
        {
            Title = input.Title,
            Description = input.Description,
            CreatedById = userId,
            Settings = new CreateFlowSettingsCommand
            {
                RequireSequentialCompletion = input.IsSequential,
                AllowRetry = input.AllowRetry,
                TimeToCompleteWorkingDays = input.TimeLimit,
                MaxAttempts = input.PassingScore
            }
        };

        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(result.Message);
        }

        // Конвертируем результат в FlowDto
        return new FlowDto
        {
            Id = result.FlowId,
            Title = result.Title,
            Status = Lauf.Domain.Enums.FlowStatus.Draft
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
    [Authorize]
    public async Task<AssignFlowCommandResult> AssignFlow(
        [Service] IMediator mediator,
        [Service] ICurrentUserService currentUserService,
        AssignFlowInput input,
        CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated");
        
        var command = new AssignFlowCommand
        {
            UserId = input.UserId,
            FlowId = input.FlowId,
            Deadline = input.DueDate,
            CreatedById = input.AssignedBy ?? userId
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

    /// <summary>
    /// Создать шаг потока
    /// </summary>
    [Authorize]
    public async Task<CreateFlowStepCommandResult> CreateFlowStep(
        [Service] IMediator mediator,
        CreateFlowStepInput input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateFlowStepCommand
            {
                FlowId = input.FlowId,
                Title = input.Title,
                Description = input.Description,
                Order = input.Order,
                IsRequired = input.IsRequired,
                Instructions = input.Instructions,
                Notes = input.Notes
            };

            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании шага потока {FlowId}", input.FlowId);
            throw new GraphQLException("Не удалось создать шаг потока");
        }
    }

    /// <summary>
    /// Создать компонент шага
    /// </summary>
    [Authorize]
    public async Task<CreateFlowComponentCommandResult> CreateFlowComponent(
        [Service] IMediator mediator,
        CreateFlowComponentInput input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var command = new CreateFlowComponentCommand
            {
                FlowStepId = input.FlowStepId,
                Title = input.Title,
                Description = input.Description,
                Type = input.Type,
                Content = input.Content,
                Order = input.Order,
                IsRequired = input.IsRequired,
            };

            var result = await mediator.Send(command, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании компонента шага {StepId}", input.FlowStepId);
            throw new GraphQLException("Не удалось создать компонент шага");
        }
    }
}

/// <summary>
/// Input типы для мутаций
/// </summary>
public record CreateUserInput(
    string FirstName,
    string LastName,
    long? TelegramUserId);

public record UpdateUserInput(
    Guid UserId,
    string FirstName,
    string LastName,
    long? TelegramUserId,
    List<Guid>? RoleIds);

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

public record CreateFlowStepInput(
    Guid FlowId,
    string Title,
    string Description,
    int? Order = null,
    bool IsRequired = true,
    string Instructions = "",
    string Notes = "");

public record CreateFlowComponentInput(
    Guid FlowStepId,
    string Title,
    string Description,
    ComponentType Type,
    string Content = "{}",
    int? Order = null,
    bool IsRequired = true);