using MediatR;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Users;
using Lauf.Application.Services;

namespace Lauf.Application.Queries.Users;

/// <summary>
/// Запрос для получения достижений пользователя
/// </summary>
public record GetUserAchievementsQuery : IRequest<IEnumerable<UserAchievementDto>>
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Фильтр по редкости достижения
    /// </summary>
    public AchievementRarity? Rarity { get; init; }

    /// <summary>
    /// Включать только полученные достижения
    /// </summary>
    public bool OnlyEarned { get; init; } = false;

    public GetUserAchievementsQuery(Guid userId, AchievementRarity? rarity = null, bool onlyEarned = false)
    {
        UserId = userId;
        Rarity = rarity;
        OnlyEarned = onlyEarned;
    }
}

/// <summary>
/// DTO для достижения пользователя
/// </summary>
public class UserAchievementDto
{
    /// <summary>
    /// Идентификатор достижения
    /// </summary>
    public Guid AchievementId { get; set; }

    /// <summary>
    /// Название достижения
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание достижения
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Редкость достижения
    /// </summary>
    public AchievementRarity Rarity { get; set; }

    /// <summary>
    /// Иконка достижения
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Дата получения (если получено)
    /// </summary>
    public DateTime? EarnedAt { get; set; }

    /// <summary>
    /// Получено ли достижение
    /// </summary>
    public bool IsEarned => EarnedAt.HasValue;

    /// <summary>
    /// Прогресс к получению достижения (0-100)
    /// </summary>
    public decimal Progress { get; set; }
}

/// <summary>
/// Обработчик запроса получения достижений пользователя
/// </summary>
public class GetUserAchievementsQueryHandler : IRequestHandler<GetUserAchievementsQuery, IEnumerable<UserAchievementDto>>
{
    private readonly IAchievementRepository _achievementRepository;
    private readonly IUserAchievementRepository _userAchievementRepository;
    private readonly AchievementCalculationService _achievementCalculationService;

    public GetUserAchievementsQueryHandler(
        IAchievementRepository achievementRepository,
        IUserAchievementRepository userAchievementRepository,
        AchievementCalculationService achievementCalculationService)
    {
        _achievementRepository = achievementRepository ?? throw new ArgumentNullException(nameof(achievementRepository));
        _userAchievementRepository = userAchievementRepository ?? throw new ArgumentNullException(nameof(userAchievementRepository));
        _achievementCalculationService = achievementCalculationService ?? throw new ArgumentNullException(nameof(achievementCalculationService));
    }

    /// <summary>
    /// Обработка запроса получения достижений пользователя
    /// </summary>
    public async Task<IEnumerable<UserAchievementDto>> Handle(GetUserAchievementsQuery request, CancellationToken cancellationToken)
    {
        // Получаем все достижения
        var allAchievements = await _achievementRepository.GetAllAsync(cancellationToken);
        
        // Фильтруем по редкости если указана
        if (request.Rarity.HasValue)
        {
            allAchievements = allAchievements.Where(a => a.Rarity == request.Rarity.Value);
        }

        // Получаем достижения пользователя
        var userAchievements = await _userAchievementRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        var earnedAchievementIds = userAchievements.ToDictionary(ua => ua.AchievementId, ua => ua.EarnedAt);

        var result = new List<UserAchievementDto>();
        
        foreach (var achievement in allAchievements)
        {
            var progress = await _achievementCalculationService.CalculateProgressAsync(achievement, request.UserId, cancellationToken);
            
            result.Add(new UserAchievementDto
            {
                AchievementId = achievement.Id,
                Title = achievement.Title,
                Description = achievement.Description,
                Rarity = achievement.Rarity,
                IconUrl = achievement.IconUrl,
                EarnedAt = earnedAchievementIds.GetValueOrDefault(achievement.Id),
                Progress = progress
            });
        }

        // Фильтруем только полученные если указано
        if (request.OnlyEarned)
        {
            result = result.Where(a => a.IsEarned).ToList();
        }

        return result.OrderByDescending(a => a.EarnedAt).ThenBy(a => a.Title);
    }
}