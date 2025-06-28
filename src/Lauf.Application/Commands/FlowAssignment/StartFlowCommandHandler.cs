using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Enums;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Обработчик команды начала прохождения потока
/// </summary>
public class StartFlowCommandHandler : IRequestHandler<StartFlowCommand, StartFlowCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StartFlowCommandHandler> _logger;

    public StartFlowCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<StartFlowCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Конвертирует AssignmentStatus в ProgressStatus для DTO
    /// </summary>
    private static ProgressStatus ConvertToProgressStatus(AssignmentStatus status)
    {
        return status switch
        {
            AssignmentStatus.Assigned => ProgressStatus.NotStarted,
            AssignmentStatus.InProgress => ProgressStatus.InProgress,
            AssignmentStatus.Completed => ProgressStatus.Completed,
            AssignmentStatus.Cancelled => ProgressStatus.Cancelled,
            AssignmentStatus.Paused => ProgressStatus.InProgress,
            AssignmentStatus.Overdue => ProgressStatus.InProgress,
            _ => ProgressStatus.NotStarted
        };
    }

    public async Task<StartFlowCommandResult> Handle(
        StartFlowCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Начинается обработка команды начала прохождения потока для назначения {AssignmentId}", 
                request.AssignmentId);

            // Получаем назначение потока
            var assignment = await _unitOfWork.FlowAssignments.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                var errorMessage = $"Назначение потока с ID {request.AssignmentId} не найдено";
                _logger.LogWarning(errorMessage);
                return new StartFlowCommandResult
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }

            // Проверяем, можно ли начать прохождение
            if (assignment.Status != AssignmentStatus.Assigned)
            {
                var errorMessage = $"Нельзя начать прохождение потока. Текущий статус: {assignment.Status}";
                _logger.LogWarning(errorMessage);
                return new StartFlowCommandResult
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }

            // Начинаем прохождение
            assignment.Start();
            
            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Прохождение потока для назначения {AssignmentId} успешно начато", 
                request.AssignmentId);

            // Создаем DTO для ответа (новая архитектура)
            var assignmentDto = new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = ConvertToProgressStatus(assignment.Status),
                AssignedAt = assignment.AssignedAt,
                Deadline = assignment.Deadline,
                CompletedAt = assignment.CompletedAt,
                AssignedBy = assignment.AssignedBy,
                Buddy = assignment.Buddy
            };

            return new StartFlowCommandResult
            {
                Assignment = assignmentDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при начале прохождения потока для назначения {AssignmentId}", 
                request.AssignmentId);

            return new StartFlowCommandResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}