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
            
            // Добавляем заметки о завершении, если есть
            if (!string.IsNullOrEmpty(request.CompletionNotes))
            {
                assignment.AddUserFeedback(request.CompletionNotes);
            }
            
            // Сохраняем изменения
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Прохождение потока для назначения {AssignmentId} успешно завершено", 
                request.AssignmentId);

            // Создаем DTO для ответа
            var assignmentDto = new FlowAssignmentDto
            {
                Id = assignment.Id,
                UserId = assignment.UserId,
                FlowId = assignment.FlowId,
                Status = assignment.Status,
                CreatedAt = assignment.CreatedAt,
                DueDate = assignment.DueDate,
                StartedAt = assignment.StartedAt,
                CompletedAt = assignment.CompletedAt,
                ProgressPercentage = assignment.ProgressPercent
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