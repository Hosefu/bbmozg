using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;
using Lauf.Application.DTOs.Users;

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
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserDto> Handle(
        CreateUserCommand request, 
        CancellationToken cancellationToken)
    {
        // Проверяем, что пользователь с таким Telegram ID не существует
        var telegramUserId = new TelegramUserId(request.TelegramUserId);
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
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position,
            Language = request.Language,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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
            Email = user.Email,
            Position = user.Position,
            Language = user.Language,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastActivityAt = user.LastActiveAt
        };
    }
}