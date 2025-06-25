using MediatR;
using Lauf.Application.DTOs.Users;
using Lauf.Application.DTOs.Flows;
using Lauf.Application.Queries.Users;
using Lauf.Application.Queries.Flows;
using Lauf.Application.Queries.FlowAssignments;
using Lauf.Api.GraphQL.Types;

namespace Lauf.Api.GraphQL.Resolvers;

/// <summary>
/// GraphQL Query resolver
/// </summary>
[QueryType]
public class Query
{
    /// <summary>
    /// Получить пользователей
    /// </summary>
    public async Task<IEnumerable<UserDto>> GetUsers(
        [Service] IMediator mediator,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersQuery { Skip = skip, Take = take };
        var result = await mediator.Send(query, cancellationToken);
        return result.Users;
    }

    /// <summary>
    /// Получить пользователя по ID
    /// </summary>
    public async Task<UserDto?> GetUser(
        [Service] IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);
        return result;
    }

    /// <summary>
    /// Получить потоки обучения
    /// </summary>
    public async Task<IEnumerable<FlowDto>> GetFlows(
        [Service] IMediator mediator,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFlowsQuery { Skip = skip, Take = take };
        var result = await mediator.Send(query, cancellationToken);
        return result.Flows;
    }

    /// <summary>
    /// Получить поток обучения по ID
    /// </summary>
    public async Task<FlowDetailsDto?> GetFlow(
        [Service] IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFlowDetailsQuery(id, null);
        var result = await mediator.Send(query, cancellationToken);
        return result;
    }

    /// <summary>
    /// Поиск потоков по названию
    /// </summary>
    public async Task<IEnumerable<FlowDto>> SearchFlows(
        [Service] IMediator mediator,
        string searchTerm,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchFlowsQuery 
        { 
            SearchTerm = searchTerm, 
            Skip = skip, 
            Take = take 
        };
        var result = await mediator.Send(query, cancellationToken);
        return result.Flows;
    }

    /// <summary>
    /// Получить назначения потоков
    /// </summary>
    public async Task<IEnumerable<FlowAssignmentDto>> GetFlowAssignments(
        [Service] IMediator mediator,
        Guid? userId = null,
        Guid? flowId = null,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        if (userId.HasValue)
        {
            var userQuery = new GetFlowAssignmentsByUserQuery 
            { 
                UserId = userId.Value, 
                Skip = skip, 
                Take = take 
            };
            var userResult = await mediator.Send(userQuery, cancellationToken);
            return userResult.Assignments;
        }

        if (flowId.HasValue)
        {
            var flowQuery = new GetFlowAssignmentsByFlowQuery 
            { 
                FlowId = flowId.Value, 
                Skip = skip, 
                Take = take 
            };
            var flowResult = await mediator.Send(flowQuery, cancellationToken);
            return flowResult.Assignments;
        }

        var query = new GetFlowAssignmentsQuery { Skip = skip, Take = take };
        var result = await mediator.Send(query, cancellationToken);
        return result.Assignments;
    }

    /// <summary>
    /// Получить назначение потока по ID
    /// </summary>
    public async Task<FlowAssignmentDto?> GetFlowAssignment(
        [Service] IMediator mediator,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetFlowAssignmentByIdQuery { AssignmentId = id };
        var result = await mediator.Send(query, cancellationToken);
        return result?.Assignment;
    }

    /// <summary>
    /// Получить активные назначения пользователя
    /// </summary>
    public async Task<IEnumerable<FlowAssignmentDto>> GetActiveAssignments(
        [Service] IMediator mediator,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetActiveAssignmentsQuery { UserId = userId };
        var result = await mediator.Send(query, cancellationToken);
        return result.Assignments;
    }

    /// <summary>
    /// Получить просроченные назначения
    /// </summary>
    public async Task<IEnumerable<FlowAssignmentDto>> GetOverdueAssignments(
        [Service] IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOverdueAssignmentsQuery();
        var result = await mediator.Send(query, cancellationToken);
        return result.Assignments;
    }
}