using Lauf.Domain.Entities.Flows;
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
    private readonly IFlowContentRepository _flowContentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateFlowCommandHandler> _logger;

    public CreateFlowCommandHandler(
        IFlowRepository flowRepository,
        IFlowContentRepository flowContentRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateFlowCommandHandler> logger)
    {
        _flowRepository = flowRepository ?? throw new ArgumentNullException(nameof(flowRepository));
        _flowContentRepository = flowContentRepository ?? throw new ArgumentNullException(nameof(flowContentRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateFlowCommandResult> Handle(CreateFlowCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начинается создание потока \"{Name}\" пользователем {CreatedById}", 
            request.Name, request.CreatedById);

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

            // Создаем поток-координатор (новая архитектура)
            var flow = new Flow(request.Name, request.Description, request.CreatedById);
            
            // Сохраняем поток
            await _flowRepository.AddAsync(flow, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Создаем первое содержимое потока (версия 1)
            var firstContent = new FlowContent(flow.Id, 1, request.CreatedById);

            // Сохраняем содержимое
            await _flowContentRepository.AddAsync(firstContent, cancellationToken);
            
            // Устанавливаем активное содержимое в потоке
            flow.SetActiveContent(firstContent.Id);
            await _flowRepository.UpdateAsync(flow, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Поток \"{Name}\" успешно создан с ID {FlowId}, создано первое содержимое {ContentId}", 
                request.Name, flow.Id, firstContent.Id);

            return new CreateFlowCommandResult
            {
                FlowId = flow.Id,
                ContentId = firstContent.Id,
                Version = firstContent.Version,
                IsSuccess = true,
                Message = $"Поток \"{request.Name}\" успешно создан с версией {firstContent.Version}",
                Name = flow.Name,
                Status = FlowStatus.Draft.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании потока \"{Name}\"", request.Name);

            return new CreateFlowCommandResult
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}