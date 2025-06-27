using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.Exceptions;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Application.Commands.FlowManagement;

/// <summary>
/// Обработчик команды создания потока обучения
/// </summary>
public class CreateFlowCommandHandler : IRequestHandler<CreateFlowCommand, CreateFlowCommandResult>
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFlowCommandHandler> _logger;

    public CreateFlowCommandHandler(
        IFlowRepository flowRepository,
        IFlowVersionRepository flowVersionRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            // Создаем оригинальный поток (как ссылочную сущность)
            var flow = new Flow(request.Title, request.Description);
            
            // Заполняем дополнительные свойства
            flow.Tags = request.Tags;
            flow.Priority = request.Priority;
            flow.IsRequired = request.IsRequired;
            flow.CreatedById = request.CreatedById;

            // Сохраняем оригинальный поток
            await _flowRepository.AddAsync(flow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Создаем первую версию потока (версия 1, активная)
            var firstFlowVersion = new FlowVersion(
                flow.Id, // OriginalId
                1, // Version = 1 для первой версии
                request.Title,
                request.Description,
                request.Tags,
                FlowStatus.Draft, // Начинаем с черновика
                request.Priority,
                request.IsRequired,
                request.CreatedById,
                true // IsActive = true для первой версии
            );

            // Сохраняем первую версию
            await _flowVersionRepository.AddAsync(firstFlowVersion, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Поток \"{Title}\" успешно создан с ID {FlowId}, создана первая версия {FlowVersionId}", 
                request.Title, flow.Id, firstFlowVersion.Id);

            return new CreateFlowCommandResult
            {
                FlowId = flow.Id,
                FlowVersionId = firstFlowVersion.Id,
                Version = firstFlowVersion.Version,
                IsSuccess = true,
                Message = $"Поток \"{request.Title}\" успешно создан с версией {firstFlowVersion.Version}",
                Title = flow.Title,
                Status = firstFlowVersion.Status.ToString()
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