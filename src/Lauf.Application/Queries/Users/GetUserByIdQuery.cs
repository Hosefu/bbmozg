using MediatR;
using AutoMapper;
using Lauf.Application.DTOs.Users;
using Lauf.Domain.Interfaces.Repositories;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос для получения пользователя по идентификатору
/// </summary>
public record GetUserByIdQuery : IRequest<UserDto?>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    public GetUserByIdQuery(Guid userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// Обработчик запроса получения пользователя по идентификатору
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// Обработка запроса получения пользователя
    /// </summary>
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }
}