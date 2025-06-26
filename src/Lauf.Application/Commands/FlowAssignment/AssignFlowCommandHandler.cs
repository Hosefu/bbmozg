using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Events;
using Lauf.Domain.Exceptions;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Обработчик команды назначения потока пользователю
/// </summary>
public class AssignFlowCommandHandler : IRequestHandler<AssignFlowCommand, AssignFlowCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly FlowSnapshotService _snapshotService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignFlowCommandHandler> _logger;

    public AssignFlowCommandHandler(
        IFlowRepository flowRepository,
        IUserRepository userRepository,
        IFlowAssignmentRepository assignmentRepository,
        FlowSnapshotService snapshotService,
        IUnitOfWork unitOfWork,
        ILogger<AssignFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _userRepository = userRepository;
        _assignmentRepository = assignmentRepository;
        _snapshotService = snapshotService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AssignFlowCommandResult> Handle(AssignFlowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинается назначение потока {FlowId} пользователю {UserId}", 
            request.FlowId, request.UserId);

        try
        {
            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            // Проверяем существование потока
            var flow = await _flowRepository.GetWithDetailsAsync(request.FlowId, cancellationToken);
            if (flow == null)
            {
                throw new FlowNotFoundException(request.FlowId);
            }

            // Проверяем, что поток опубликован
            if (flow.Status != Lauf.Domain.Enums.FlowStatus.Published)
            {
                throw new InvalidOperationException($"Поток {flow.Title} не опубликован и не может быть назначен");
            }

            // Проверка существующих назначений будет добавлена в следующих итерациях
            // когда реализуем IFlowAssignmentRepository.GetActiveByUserAndFlowAsync

            // Проверяем buddy если указан
            if (request.BuddyId.HasValue)
            {
                var buddy = await _userRepository.GetByIdAsync(request.BuddyId.Value, cancellationToken);
                if (buddy == null)
                {
                    throw new UserNotFoundException(request.BuddyId.Value);
                }
            }

            // Создаем снапшот потока
            var snapshot = await _snapshotService.CreateFlowSnapshotAsync(flow, cancellationToken);

            // Рассчитываем дедлайн если не указан
            var deadline = request.Deadline ?? CalculateDefaultDeadline(flow);

            // Создаем назначение
            var assignment = new Domain.Entities.Flows.FlowAssignment(
                request.UserId,
                request.FlowId,
                snapshot.Id,
                deadline,
                request.BuddyId,
                request.CreatedById,
                request.Notes,
                request.Priority);

            await _assignmentRepository.AddAsync(assignment, cancellationToken);

            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Публикация событий будет реализована через доменный диспетчер
            // var domainEvent = new FlowAssigned(...);

            _logger.LogInformation("Поток {FlowId} успешно назначен пользователю {UserId}. ID назначения: {AssignmentId}", 
                request.FlowId, request.UserId, assignment.Id);

            return new AssignFlowCommandResult
            {
                AssignmentId = assignment.Id,
                SnapshotId = snapshot.Id,
                IsSuccess = true,
                Message = $"Поток \"{flow.Title}\" успешно назначен",
                EstimatedCompletionDate = deadline
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при назначении потока {FlowId} пользователю {UserId}", 
                request.FlowId, request.UserId);

            return new AssignFlowCommandResult
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    private static DateTime CalculateDefaultDeadline(Flow flow)
    {
        // Простой расчет: добавляем к текущей дате количество дней на основе количества шагов
        // Примерно 1 день на каждые 2 шага, минимум 7 дней
        var estimatedDays = Math.Max(7, Math.Ceiling(flow.TotalSteps / 2.0));
        
        return DateTime.UtcNow.AddDays(estimatedDays);
    }
}