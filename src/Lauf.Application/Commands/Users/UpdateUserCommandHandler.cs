using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Application.DTOs.Users;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Обработчик команды обновления пользователя
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateUserCommandResult> Handle(
        UpdateUserCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Получаем пользователя
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return new UpdateUserCommandResult
                {
                    Success = false,
                    ErrorMessage = $"Пользователь с ID {request.UserId} не найден"
                };
            }

            // Обновляем поля пользователя, если они переданы
            if (!string.IsNullOrEmpty(request.Email))
            {
                user.Email = request.Email;
            }

            if (!string.IsNullOrEmpty(request.FullName))
            {
                var nameParts = request.FullName.Split(' ');
                user.FirstName = nameParts.FirstOrDefault() ?? "";
                user.LastName = string.Join(" ", nameParts.Skip(1));
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    user.Activate();
                }
                else
                {
                    user.Deactivate();
                }
            }

            user.UpdatedAt = DateTime.UtcNow;

            // Сохраняем изменения
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Пользователь {UserId} успешно обновлен", request.UserId);

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

            return new UpdateUserCommandResult
            {
                User = userDto,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", request.UserId);
            
            return new UpdateUserCommandResult
            {
                Success = false,
                ErrorMessage = "Произошла ошибка при обновлении пользователя"
            };
        }
    }
}