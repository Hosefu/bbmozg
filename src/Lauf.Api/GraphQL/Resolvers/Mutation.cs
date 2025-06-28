using MediatR;
using Microsoft.AspNetCore.Authorization;
using Lauf.Application.DTOs.Users;
using Lauf.Application.DTOs.Flows;
using Lauf.Application.Commands.Users;
using Lauf.Application.Commands.Flows;
using Lauf.Application.Commands.FlowManagement;
using Lauf.Application.Commands.FlowAssignment;
using Lauf.Application.Commands.FlowSteps;
using FluentValidation;

using Lauf.Application.Commands.Components;
using Lauf.Application.Commands.FlowComponents;
using Lauf.Application.Queries.Flows;
using Lauf.Application.Services.Interfaces;
using Lauf.Api.GraphQL.Types;
using Lauf.Api.GraphQL.Types.Components;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Enums;
using AutoMapper;

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
        try
        {
            _logger.LogInformation("Начинается создание флоу через GraphQL: {Title}", input.Title);
            
            var userId = currentUserService.GetCurrentUserId() ?? throw new UnauthorizedAccessException("User not authenticated");
            
            _logger.LogInformation("Пользователь аутентифицирован: {UserId}", userId);
            
            var command = new CreateFlowCommand
            {
                Name = input.Title,
                Description = input.Description,
                CreatedById = userId,
                Settings = new CreateFlowSettingsCommand
                {
                    RequireSequentialCompletionComponents = input.IsSequential,
                    AllowSelfRestart = input.AllowRetry,
                    DaysPerStep = input.TimeLimit ?? 7
                }
            };

            _logger.LogInformation("Отправляем команду создания флоу");
            var result = await mediator.Send(command, cancellationToken);
            
            if (!result.IsSuccess)
            {
                _logger.LogError("Ошибка создания флоу: {Message}", result.Message);
                throw new InvalidOperationException(result.Message);
            }

            _logger.LogInformation("Флоу создан успешно с ID: {FlowId}", result.FlowId);

            // Получаем полную информацию о созданном флоу
            var getFlowQuery = new Lauf.Application.Queries.Flows.GetFlowByIdQuery(result.FlowId, false);
            _logger.LogInformation("Запрашиваем данные созданного флоу");
            var flowDto = await mediator.Send(getFlowQuery, cancellationToken);
            
            if (flowDto == null)
            {
                _logger.LogError("Созданный флоу не найден по ID: {FlowId}", result.FlowId);
                throw new InvalidOperationException("Созданный флоу не найден");
            }

            _logger.LogInformation("Флоу успешно получен, возвращаем результат");
            return flowDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании флоу через GraphQL: {Title}", input.Title);
            throw new GraphQLException($"Не удалось создать флоу: {ex.Message}");
        }
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
    /// Создать типизированный компонент (новый метод)
    /// </summary>
    [Authorize]
    public async Task<CreateComponentResult> CreateComponent(
        [Service] IMediator mediator,
        [Service] IMapper mapper,
        CreateComponentInput input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Определяем тип компонента и создаем соответствующую команду
            if (input.Article != null)
            {
                var command = mapper.Map<CreateArticleComponentCommand>(input.Article);
                var result = await mediator.Send(command, cancellationToken);
                
                return new CreateComponentResult
                {
                    Article = new ArticleComponentResult
                    {
                        IsSuccess = result.IsSuccess,
                        Message = result.Message,
                        ComponentId = result.ComponentId,
                        Component = result.Component
                    }
                };
            }
            
            if (input.Quiz != null)
            {
                try
                {
                    var command = mapper.Map<CreateQuizComponentCommand>(input.Quiz);
                    var result = await mediator.Send(command, cancellationToken);
                    
                    return new CreateComponentResult
                    {
                        Quiz = new QuizComponentResult
                        {
                            IsSuccess = result.IsSuccess,
                            Message = result.Message,
                            ComponentId = result.ComponentId,
                            Component = result.Component
                        }
                    };
                }
                catch (FluentValidation.ValidationException ex)
                {
                    var validationErrors = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
                    return new CreateComponentResult
                    {
                        Quiz = new QuizComponentResult
                        {
                            IsSuccess = false,
                            Message = $"Ошибка валидации: {validationErrors}",
                            ComponentId = null,
                            Component = null
                        }
                    };
                }
            }
            
            if (input.Task != null)
            {
                var command = mapper.Map<CreateTaskComponentCommand>(input.Task);
                var result = await mediator.Send(command, cancellationToken);
                
                return new CreateComponentResult
                {
                    Task = new TaskComponentResult
                    {
                        IsSuccess = result.IsSuccess,
                        Message = result.Message,
                        ComponentId = result.ComponentId,
                        Component = result.Component
                    }
                };
            }

            throw new GraphQLException("Необходимо указать один из типов компонента: Article, Quiz или Task");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании типизированного компонента");
            throw new GraphQLException("Не удалось создать компонент");
        }
    }

    /// <summary>
    /// Изменить порядок шага в потоке
    /// </summary>
    [Authorize]
    public async Task<ReorderFlowStepCommandResult> ReorderFlowStep(
        [Service] IMediator mediator,
        ReorderFlowStepInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new ReorderFlowStepCommand
        {
            StepId = input.StepId,
            NewPosition = input.NewPosition
        };

        var result = await mediator.Send(command, cancellationToken);
        return result;
    }

    /// <summary>
    /// Изменить порядок компонента в шаге
    /// </summary>
    [Authorize]
    public async Task<ReorderFlowComponentCommandResult> ReorderFlowComponent(
        [Service] IMediator mediator,
        ReorderFlowComponentInput input,
        CancellationToken cancellationToken = default)
    {
        var command = new ReorderFlowComponentCommand
        {
            ComponentId = input.ComponentId,
            NewPosition = input.NewPosition
        };

        var result = await mediator.Send(command, cancellationToken);
        return result;
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

public record ReorderFlowStepInput(
    Guid StepId,
    int NewPosition);

public record ReorderFlowComponentInput(
    Guid ComponentId,
    int NewPosition);

