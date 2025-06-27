using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;
using Lauf.Domain.Events;
using Lauf.Domain.Exceptions;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Обработчик команды назначения потока пользователю
/// </summary>
public class AssignFlowCommandHandler : IRequestHandler<AssignFlowCommand, AssignFlowCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IVersioningService _versioningService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignFlowCommandHandler> _logger;

    public AssignFlowCommandHandler(
        IFlowRepository flowRepository,
        IFlowVersionRepository flowVersionRepository,
        IUserRepository userRepository,
        IFlowAssignmentRepository assignmentRepository,
        IVersioningService versioningService,
        IUnitOfWork unitOfWork,
        ILogger<AssignFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
        _versioningService = versioningService ?? throw new ArgumentNullException(nameof(versioningService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            // Получаем активную версию потока
            var activeFlowVersion = await _versioningService.GetActiveFlowVersionAsync(request.FlowId, cancellationToken);
            if (activeFlowVersion == null)
            {
                throw new FlowNotFoundException(request.FlowId);
            }

            // Загружаем полную версию потока с деталями
            var flowVersionWithDetails = await _flowVersionRepository.GetByIdWithDetailsAsync(activeFlowVersion.Id, cancellationToken);
            if (flowVersionWithDetails == null)
            {
                throw new InvalidOperationException($"Не удалось загрузить детали активной версии потока {request.FlowId}");
            }

            // Проверяем, что поток опубликован
            if (flowVersionWithDetails.Status != FlowStatus.Published)
            {
                throw new InvalidOperationException($"Поток {flowVersionWithDetails.Title} не опубликован и не может быть назначен");
            }

            // Проверяем buddy если указан
            if (request.BuddyId.HasValue)
            {
                var buddy = await _userRepository.GetByIdAsync(request.BuddyId.Value, cancellationToken);
                if (buddy == null)
                {
                    throw new UserNotFoundException(request.BuddyId.Value);
                }
            }

            // Проверяем существующие активные назначения
            var hasActiveAssignment = await _assignmentRepository.HasActiveAssignmentAsync(request.UserId, request.FlowId, cancellationToken);
            if (hasActiveAssignment)
            {
                throw new InvalidOperationException($"У пользователя уже есть активное назначение для этого потока");
            }

            // Рассчитываем дедлайн если не указан
            var deadline = request.Deadline ?? CalculateDefaultDeadline(flowVersionWithDetails);

            // Создаем назначение, используя FlowVersionId вместо SnapshotId
            var assignment = new Domain.Entities.Flows.FlowAssignment(
                request.UserId,
                flowVersionWithDetails.OriginalId, // Сохраняем ссылку на оригинальный поток
                flowVersionWithDetails.OriginalId, // OriginalFlowId
                flowVersionWithDetails.Id, // Ссылаемся на конкретную версию
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
                FlowVersionId = flowVersionWithDetails.Id,
                IsSuccess = true,
                Message = $"Поток \"{flowVersionWithDetails.Title}\" (версия {flowVersionWithDetails.Version}) успешно назначен",
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

    private static DateTime CalculateDefaultDeadline(Domain.Entities.Versions.FlowVersion flowVersion)
    {
        // Простой расчет: добавляем к текущей дате количество дней на основе количества этапов
        // Примерно 1 день на каждые 2 этапа, минимум 7 дней
        var estimatedDays = Math.Max(7, Math.Ceiling(flowVersion.StepVersions.Count / 2.0));
        
        return DateTime.UtcNow.AddDays(estimatedDays);
    }
}