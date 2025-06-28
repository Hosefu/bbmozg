using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Shared.Helpers;

namespace Lauf.Application.Commands.FlowSteps;

/// <summary>
/// Обработчик команды изменения порядка шага в потоке
/// </summary>
public class ReorderFlowStepCommandHandler : IRequestHandler<ReorderFlowStepCommand, ReorderFlowStepCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly ILogger<ReorderFlowStepCommandHandler> _logger;

    public ReorderFlowStepCommandHandler(
        IFlowRepository flowRepository,
        ILogger<ReorderFlowStepCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _logger = logger;
    }

    public async Task<ReorderFlowStepCommandResult> Handle(ReorderFlowStepCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Изменение порядка шага {StepId} на позицию {NewPosition}", 
                request.StepId, request.NewPosition);

            // Находим поток по шагу
            var flow = await _flowRepository.GetFlowByStepIdAsync(request.StepId, cancellationToken);
            if (flow == null)
            {
                return ReorderFlowStepCommandResult.Failure("Поток с указанным шагом не найден");
            }

            // В новой архитектуре проверка Flow.IsActive вместо Status
            if (!flow.IsActive)
            {
                return ReorderFlowStepCommandResult.Failure("Нельзя изменять порядок шагов в неактивном потоке");
            }

            var step = flow.ActiveContent.Steps.FirstOrDefault(s => s.Id == request.StepId);
            if (step == null)
            {
                return ReorderFlowStepCommandResult.Failure("Шаг не найден");
            }

            // Получаем все шаги отсортированные по LexoRank (новая архитектура)
            var allSteps = flow.ActiveContent.Steps.OrderBy(s => s.Order).ToArray();
            
            // Проверяем корректность новой позиции
            if (request.NewPosition < 0 || request.NewPosition >= allSteps.Length)
            {
                return ReorderFlowStepCommandResult.Failure("Некорректная позиция");
            }

            // Находим текущую позицию
            var currentPosition = Array.IndexOf(allSteps, step);
            if (currentPosition == request.NewPosition)
            {
                return ReorderFlowStepCommandResult.Success("Шаг уже находится на указанной позиции");
            }

            // Рассчитываем новый LexoRank
            string newLexoRank;
            
            if (request.NewPosition == 0)
            {
                // Перемещаем в начало
                var firstStep = allSteps[0];
                newLexoRank = LexoRankHelper.Previous(firstStep.Order);
            }
            else if (request.NewPosition == allSteps.Length - 1)
            {
                // Перемещаем в конец
                var lastStep = allSteps[allSteps.Length - 1];
                newLexoRank = LexoRankHelper.Next(lastStep.Order);
            }
            else
            {
                // Вставляем между элементами
                var prevStep = allSteps[request.NewPosition - 1];
                var nextStep = allSteps[request.NewPosition];
                newLexoRank = LexoRankHelper.Between(prevStep.Order, nextStep.Order);
            }

            // Обновляем LexoRank шага
            step.Order = newLexoRank;

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow, cancellationToken);

            _logger.LogInformation("Порядок шага {StepId} успешно изменен", request.StepId);

            return ReorderFlowStepCommandResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при изменении порядка шага {StepId}", request.StepId);
            return ReorderFlowStepCommandResult.Failure("Произошла ошибка при изменении порядка шага");
        }
    }
} 