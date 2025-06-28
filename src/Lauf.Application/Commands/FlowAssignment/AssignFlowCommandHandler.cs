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
    private readonly IFlowContentRepository _flowContentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignFlowCommandHandler> _logger;

    public AssignFlowCommandHandler(
        IFlowRepository flowRepository,
        IFlowContentRepository flowContentRepository,
        IUserRepository userRepository,
        IFlowAssignmentRepository assignmentRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _flowContentRepository = flowContentRepository ?? throw new ArgumentNullException(nameof(flowContentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _assignmentRepository = assignmentRepository ?? throw new ArgumentNullException(nameof(assignmentRepository));
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

            // Получаем поток с активным содержимым (новая архитектура)
            var flow = await _flowRepository.GetByIdAsync(request.FlowId, cancellationToken);
            if (flow == null)
            {
                throw new FlowNotFoundException(request.FlowId);
            }

            // Проверяем, что поток активен
            if (!flow.IsActive)
            {
                throw new InvalidOperationException($"Поток {flow.Name} неактивен и не может быть назначен");
            }

            // Получаем активное содержимое потока
            var activeContent = await _flowContentRepository.GetActiveByFlowIdAsync(request.FlowId, cancellationToken);
            if (activeContent == null)
            {
                throw new InvalidOperationException($"У потока {flow.Name} нет активного содержимого");
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

            // Создаем назначение (новая архитектура - упрощенный конструктор)
            var assignment = new Domain.Entities.Flows.FlowAssignment(
                request.UserId,
                request.FlowId,
                activeContent.Id,
                request.CreatedById);

            // Добавляем бадди если указан
            if (request.BuddyId.HasValue)
            {
                var buddy = await _userRepository.GetByIdAsync(request.BuddyId.Value, cancellationToken);
                if (buddy != null)
                {
                    assignment.AddBuddy(buddy);
                }
            }

            // Создаем начальный прогресс
            var progress = new Domain.Entities.Flows.FlowAssignmentProgress(
                assignment.Id,
                activeContent.Steps.Count);

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
                FlowContentId = activeContent.Id,
                IsSuccess = true,
                Message = $"Поток \"{flow.Name}\" (версия {activeContent.Version}) успешно назначен",
                EstimatedCompletionDate = assignment.Deadline
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

    private static DateTime CalculateDefaultDeadline(Flow flow, FlowContent content)
    {
        // Простой расчет в новой архитектуре: используем настройки потока или дефолт
        var daysPerStep = flow.Settings?.DaysPerStep ?? 7;
        var totalDays = content.Steps.Count * daysPerStep;
        
        return DateTime.UtcNow.AddDays(Math.Max(7, totalDays));
    }
}