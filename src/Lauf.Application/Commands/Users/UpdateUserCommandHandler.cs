using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces;
using Lauf.Application.DTOs.Users;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Exceptions;
using Lauf.Domain.ValueObjects;

namespace Lauf.Application.Commands.Users;

/// <summary>
/// Обработчик команды обновления пользователя
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
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

    public async Task<UserDto> Handle(
        UpdateUserCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            // Получаем пользователя
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new ArgumentException($"Пользователь с ID {request.UserId} не найден");
            }

            // Обновляем поля пользователя
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            if (request.TelegramUserId.HasValue)
            {
                user.TelegramUserId = new TelegramUserId(request.TelegramUserId.Value);
            }

            if (request.RoleIds != null && request.RoleIds.Any())
            {
                // Получаем роли по ID
                var roles = await _unitOfWork.Roles.GetByIdsAsync(request.RoleIds, cancellationToken);
                if (roles.Count != request.RoleIds.Count)
                {
                    throw new ArgumentException("Одна или несколько указанных ролей не найдены");
                }

                // Очищаем текущие роли и устанавливаем новые
                user.Roles.Clear();
                foreach (var role in roles)
                {
                    user.Roles.Add(role);
                }
            }

            user.UpdatedAt = DateTime.UtcNow;

            // Сохраняем изменения
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Пользователь {UserId} успешно обновлен", request.UserId);

            // Возвращаем обновленного пользователя
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TelegramUserId = user.TelegramUserId.Value,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.Roles.Select(r => r.Name).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", request.UserId);
            throw;
        }
    }
}