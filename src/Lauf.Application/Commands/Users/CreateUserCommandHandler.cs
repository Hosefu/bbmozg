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
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResult>
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

    public async Task<CreateUserCommandResult> Handle(
        CreateUserCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Проверяем, что пользователь с таким Telegram ID не существует
            var telegramUserId = new TelegramUserId(request.TelegramId);
            var existingUser = await _unitOfWork.Users.GetByTelegramIdAsync(telegramUserId, cancellationToken);
            if (existingUser != null)
            {
                return new CreateUserCommandResult
                {
                    Success = false,
                    ErrorMessage = $"Пользователь с Telegram ID {request.TelegramId} уже существует"
                };
            }

            // Создаем нового пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                Email = request.Email,
                FirstName = request.FullName.Split(' ').FirstOrDefault() ?? "",
                LastName = string.Join(" ", request.FullName.Split(' ').Skip(1)),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Сохраняем пользователя
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Пользователь {FullName} успешно создан с ID {UserId}", 
                request.FullName, user.Id);

            // Создаем DTO для ответа
            var userDto = new UserDto
            {
                Id = user.Id,
                TelegramUserId = user.TelegramUserId.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Position = request.Position,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastActivityAt = user.LastActiveAt
            };

            return new CreateUserCommandResult
            {
                User = userDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании пользователя {FullName}", request.FullName);
            
            return new CreateUserCommandResult
            {
                Success = false,
                ErrorMessage = "Произошла ошибка при создании пользователя"
            };
        }
    }
}