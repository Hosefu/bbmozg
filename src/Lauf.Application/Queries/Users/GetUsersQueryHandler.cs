using MediatR;
using Microsoft.Extensions.Logging;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Application.DTOs.Users;
using AutoMapper;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Обработчик запроса получения списка пользователей
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersQueryResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetUsersQueryResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Получение списка пользователей: Skip={Skip}, Take={Take}", 
                request.Skip, request.Take);

            // Получаем пользователей из репозитория
            var users = await _userRepository.GetAllAsync(cancellationToken);
            
            // Применяем фильтры
            var filteredUsers = users.AsEnumerable();
            
            if (request.IsActive.HasValue)
            {
                filteredUsers = filteredUsers.Where(u => u.IsActive == request.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLowerInvariant();
                filteredUsers = filteredUsers.Where(u => 
                    u.FirstName.ToLowerInvariant().Contains(searchTerm) ||
                    u.LastName.ToLowerInvariant().Contains(searchTerm));
            }

            var totalCount = filteredUsers.Count();
            var pagedUsers = filteredUsers
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            // Конвертируем в DTO
            var userDtos = pagedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                TelegramUserId = user.TelegramUserId.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,

                Position = user.Position,
                Language = user.Language,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastActivityAt = user.LastActiveAt
            }).ToList();

            _logger.LogInformation("Найдено {TotalCount} пользователей, возвращено {Count}", 
                totalCount, userDtos.Count);

            return new GetUsersQueryResult
            {
                Users = userDtos,
                TotalCount = totalCount,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка пользователей");
            
            return new GetUsersQueryResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}