using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Enums;
using Lauf.Application.DTOs.Flows;

namespace Lauf.Application.Commands.FlowAssignment;

/// <summary>
/// Обработчик команды завершения прохождения потока
/// </summary>
public class CompleteFlowCommandHandler : IRequestHandler<CompleteFlowCommand, CompleteFlowCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteFlowCommandHandler> _logger;

    public CompleteFlowCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CompleteFlowCommandHandler> logger)
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

    public async Task<CompleteFlowCommandResult> Handle(
        CompleteFlowCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Начинается обработка команды завершения прохождения потока для назначения {AssignmentId}", 
                request.AssignmentId);

            // Получаем назначение потока
            var assignment = await _unitOfWork.FlowAssignments.GetByIdAsync(request.AssignmentId, cancellationToken);
            if (assignment == null)
            {
                var errorMessage = $"Назначение потока с ID {request.AssignmentId} не найдено";
                _logger.LogWarning(errorMessage);
                return new CompleteFlowCommandResult
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }

            // Проверяем, можно ли завершить прохождение
            if (assignment.Status != AssignmentStatus.InProgress)
            {
                var errorMessage = $"Нельзя завершить прохождение потока. Текущий статус: {assignment.Status}";
                _logger.LogWarning(errorMessage);
                return new CompleteFlowCommandResult
                {
                    Success = false,
                    ErrorMessage = errorMessage
                };
            }

            // Завершаем прохождение
            assignment.Complete();
            
            // В новой архитектуре пропускаем заметки о завершении
            // AddUserFeedback метода больше нет
            if (!string.IsNullOrEmpty(request.CompletionNotes))
            {
                _logger.LogInformation("Получены заметки о завершении для назначения {AssignmentId}: {Notes}", 
                    request.AssignmentId, request.CompletionNotes);
            }
            
            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Прохождение потока для назначения {AssignmentId} успешно завершено", 
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

            return new CompleteFlowCommandResult
            {
                Assignment = assignmentDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при завершении прохождения потока для назначения {AssignmentId}", 
                request.AssignmentId);

            return new CompleteFlowCommandResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}