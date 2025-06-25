using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Exceptions;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lauf.Application.Commands.FlowManagement;

/// <summary>
/// Обработчик команды создания потока обучения
/// </summary>
public class CreateFlowCommandHandler : IRequestHandler<CreateFlowCommand, CreateFlowCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFlowCommandHandler> _logger;

    public CreateFlowCommandHandler(
        IFlowRepository flowRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateFlowCommandResult> Handle(CreateFlowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинается создание потока \"{Title}\" пользователем {CreatedById}", 
            request.Title, request.CreatedById);

        try
        {
            // Проверяем существование создателя только если ID не пустой
            if (request.CreatedById != Guid.Empty)
            {
                var creator = await _userRepository.GetByIdAsync(request.CreatedById, cancellationToken);
                if (creator == null)
                {
                    throw new UserNotFoundException(request.CreatedById);
                }
            }

            // Создаем поток
            var flow = new Flow(request.Title, request.Description);
            
            // Заполняем дополнительные свойства
            flow.Category = request.Category;
            flow.Tags = request.Tags;
            flow.Priority = request.Priority;
            flow.IsRequired = request.IsRequired;
            flow.CreatedById = request.CreatedById;

            // Создание настроек потока будет добавлено в следующих итерациях
            // Пока что не создаем настройки - они будут добавлены позже

            // Сохраняем поток
            await _flowRepository.AddAsync(flow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Поток \"{Title}\" успешно создан с ID {FlowId}", 
                request.Title, flow.Id);

            return new CreateFlowCommandResult
            {
                FlowId = flow.Id,
                IsSuccess = true,
                Message = $"Поток \"{request.Title}\" успешно создан",
                Title = flow.Title,
                Status = flow.Status.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании потока \"{Title}\"", request.Title);

            return new CreateFlowCommandResult
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}