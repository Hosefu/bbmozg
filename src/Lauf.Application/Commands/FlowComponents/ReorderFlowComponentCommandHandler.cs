using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Shared.Helpers;

namespace Lauf.Application.Commands.FlowComponents;

/// <summary>
/// Обработчик команды изменения порядка компонента в шаге
/// </summary>
public class ReorderFlowComponentCommandHandler : IRequestHandler<ReorderFlowComponentCommand, ReorderFlowComponentCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly ILogger<ReorderFlowComponentCommandHandler> _logger;

    public ReorderFlowComponentCommandHandler(
        IFlowRepository flowRepository,
        ILogger<ReorderFlowComponentCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _logger = logger;
    }

    public async Task<ReorderFlowComponentCommandResult> Handle(ReorderFlowComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Изменение порядка компонента {ComponentId} на позицию {NewPosition}", 
                request.ComponentId, request.NewPosition);

            // Находим шаг по компоненту
            var step = await _flowRepository.GetStepByComponentIdAsync(request.ComponentId, cancellationToken);
            if (step == null)
            {
                return ReorderFlowComponentCommandResult.Failure("Шаг с указанным компонентом не найден");
            }

            // Проверяем, что поток в статусе черновика
            var flow = await _flowRepository.GetFlowByStepIdAsync(step.Id, cancellationToken);
            if (flow?.Status != Domain.Enums.FlowStatus.Draft)
            {
                return ReorderFlowComponentCommandResult.Failure("Нельзя изменять порядок компонентов в опубликованном потоке");
            }

            var component = step.Components.FirstOrDefault(c => c.Id == request.ComponentId);
            if (component == null)
            {
                return ReorderFlowComponentCommandResult.Failure("Компонент не найден в шаге");
            }

            // Получаем все компоненты отсортированные по LexoRank
            var allComponents = step.Components.OrderBy(c => c.Order).ToArray();
            
            // Проверяем корректность новой позиции
            if (request.NewPosition < 0 || request.NewPosition >= allComponents.Length)
            {
                return ReorderFlowComponentCommandResult.Failure("Некорректная позиция");
            }

            // Находим текущую позицию
            var currentPosition = Array.IndexOf(allComponents, component);
            if (currentPosition == request.NewPosition)
            {
                return ReorderFlowComponentCommandResult.Success("Компонент уже находится на указанной позиции");
            }

            // Рассчитываем новый LexoRank
            string newLexoRank;
            
            if (request.NewPosition == 0)
            {
                // Перемещаем в начало
                var firstComponent = allComponents[0];
                newLexoRank = LexoRankHelper.Previous(firstComponent.Order);
            }
            else if (request.NewPosition == allComponents.Length - 1)
            {
                // Перемещаем в конец
                var lastComponent = allComponents[allComponents.Length - 1];
                newLexoRank = LexoRankHelper.Next(lastComponent.Order);
            }
            else
            {
                // Вставляем между элементами
                var prevComponent = allComponents[request.NewPosition - 1];
                var nextComponent = allComponents[request.NewPosition];
                newLexoRank = LexoRankHelper.Between(prevComponent.Order, nextComponent.Order);
            }

            // Order и UpdatedAt - protected setters, нужно обновлять через методы ComponentBase
            // Используем рефлексию для обновления Order или создаем новый компонент
            var componentType = component.GetType();
            var orderProperty = componentType.GetProperty("Order");
            var updatedAtProperty = componentType.GetProperty("UpdatedAt");
            
            if (orderProperty != null && updatedAtProperty != null)
            {
                orderProperty.SetValue(component, newLexoRank);
                updatedAtProperty.SetValue(component, DateTime.UtcNow);
            }

            // Сохраняем изменения
            await _flowRepository.UpdateAsync(flow!, cancellationToken);

            _logger.LogInformation("Порядок компонента {ComponentId} успешно изменен", request.ComponentId);

            return ReorderFlowComponentCommandResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при изменении порядка компонента {ComponentId}", request.ComponentId);
            return ReorderFlowComponentCommandResult.Failure("Произошла ошибка при изменении порядка компонента");
        }
    }
} 