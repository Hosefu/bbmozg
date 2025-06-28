using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;
using Lauf.Application.DTOs.Users;
using Lauf.Shared.Constants;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Обработчик команды создания пользователя
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<CreateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserDto> Handle(
        CreateUserCommand request, 
        CancellationToken cancellationToken)
    {
        // Проверяем, что пользователь с таким Telegram ID не существует
        var telegramUserId = request.TelegramUserId.HasValue ? new TelegramUserId(request.TelegramUserId.Value) : null!;
        var existingUser = await _unitOfWork.Users.GetByTelegramIdAsync(telegramUserId, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"Пользователь с Telegram ID {request.TelegramUserId} уже существует");
        }

        // Создаем нового пользователя
        var user = new User
        {
            Id = Guid.NewGuid(),
            TelegramUserId = telegramUserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            // Position и Language убраны в новой архитектуре
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Назначаем роль Employee по умолчанию при создании пользователя
        var employeeRole = await _unitOfWork.Roles.GetByNameAsync(Roles.Employee, cancellationToken);
        if (employeeRole != null)
        {
            user.Roles.Add(employeeRole);
            _logger.LogInformation("Пользователю {FirstName} {LastName} назначена роль Employee по умолчанию", 
                request.FirstName, request.LastName);
        }
        else
        {
            _logger.LogWarning("Роль Employee не найдена в системе. Пользователь создан без роли");
        }

        // Сохраняем пользователя
        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Пользователь {FirstName} {LastName} успешно создан с ID {UserId}", 
            request.FirstName, request.LastName, user.Id);

        // Создаем DTO для ответа
        return new UserDto
        {
            Id = user.Id,
            TelegramUserId = user.TelegramUserId.Value,
            FirstName = user.FirstName,
            LastName = user.LastName,
            // Position и Language убраны в новой архитектуре
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastActivityAt = user.LastActiveAt,
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }
}