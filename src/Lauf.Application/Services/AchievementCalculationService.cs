using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;

namespace Lauf.Application.Services;

/// <summary>
/// Сервис для расчета прогресса достижений
/// </summary>
public class AchievementCalculationService
{
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    private readonly ProgressCalculationService _progressCalculationService;

    public AchievementCalculationService(
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository,
        ProgressCalculationService progressCalculationService)
    {
        _progressRepository = progressRepository;
        _assignmentRepository = assignmentRepository;
        _progressCalculationService = progressCalculationService;
    }

    /// <summary>
    /// Рассчитать прогресс к получению достижения
    /// </summary>
    /// <param name="achievement">Достижение</param>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Прогресс от 0 до 100</returns>
    public async Task<decimal> CalculateProgressAsync(Achievement achievement, Guid userId, CancellationToken cancellationToken = default)
    {
        return achievement.Title.ToLower() switch
        {
            "первые шаги" => await CalculateFirstStepsProgressAsync(userId, cancellationToken),
            "быстрый старт" => await CalculateFastStartProgressAsync(userId, cancellationToken),
            "настойчивость" => await CalculatePersistenceProgressAsync(userId, cancellationToken),
            "марафонец" => await CalculateMarathonProgressAsync(userId, cancellationToken),
            "идеальный ученик" => await CalculatePerfectStudentProgressAsync(userId, cancellationToken),
            "командный игрок" => await CalculateTeamPlayerProgressAsync(userId, cancellationToken),
            "эксперт" => await CalculateExpertProgressAsync(userId, cancellationToken),
            "лидер" => await CalculateLeaderProgressAsync(userId, cancellationToken),
            "исследователь" => await CalculateExplorerProgressAsync(userId, cancellationToken),
            "новатор" => await CalculateInnovatorProgressAsync(userId, cancellationToken),
            _ => 0
        };
    }

    /// <summary>
    /// Проверить все достижения пользователя и получить новые
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список новых достижений</returns>
    public async Task<List<Achievement>> CheckNewAchievementsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userStats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        var newAchievements = new List<Achievement>();

        // Проверяем различные критерии достижений
        if (userStats.CompletedFlowsCount >= 1)
            newAchievements.Add(CreateAchievement("Первые шаги", "Завершил первый поток обучения"));

        if (userStats.CompletedFlowsCount >= 3 && userStats.DaysActive <= 7)
            newAchievements.Add(CreateAchievement("Быстрый старт", "Завершил 3 потока за неделю"));

        if (userStats.DaysActive >= 30)
            newAchievements.Add(CreateAchievement("Настойчивость", "Учился 30 дней подряд"));

        if (userStats.TotalLearningHours >= 100)
            newAchievements.Add(CreateAchievement("Марафонец", "Потратил 100+ часов на обучение"));

        if (userStats.OverallProgress >= 95)
            newAchievements.Add(CreateAchievement("Идеальный ученик", "Достиг 95%+ прогресса"));

        return newAchievements;
    }

    #region Private Achievement Calculation Methods

    private async Task<decimal> CalculateFirstStepsProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        return Math.Min(100, stats.CompletedFlowsCount * 100);
    }

    private async Task<decimal> CalculateFastStartProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        if (stats.DaysActive > 7) return 0; // Время истекло
        
        var progress = (decimal)stats.CompletedFlowsCount / 3 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculatePersistenceProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        var progress = (decimal)stats.DaysActive / 30 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculateMarathonProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        var progress = (decimal)stats.TotalLearningHours / 100 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculatePerfectStudentProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        return (decimal)stats.OverallProgress;
    }

    private async Task<decimal> CalculateTeamPlayerProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Заглушка - требует данные о взаимодействии с коллегами
        await Task.CompletedTask;
        return 0;
    }

    private async Task<decimal> CalculateExpertProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        var progress = (decimal)stats.CompletedFlowsCount / 10 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculateLeaderProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Заглушка - требует данные о лидерских качествах
        await Task.CompletedTask;
        return 0;
    }

    private async Task<decimal> CalculateExplorerProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var stats = await _progressCalculationService.GetUserProgressStatisticsAsync(userId, cancellationToken);
        var progress = (decimal)stats.AssignedFlowsCount / 20 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculateInnovatorProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Заглушка - требует данные об инновационной деятельности
        await Task.CompletedTask;
        return 0;
    }

    #endregion

    private Achievement CreateAchievement(string title, string description)
    {
        return new Achievement
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Rarity = DetermineRarity(title),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private Domain.Enums.AchievementRarity DetermineRarity(string title)
    {
        return title.ToLower() switch
        {
            "первые шаги" => Domain.Enums.AchievementRarity.Common,
            "быстрый старт" => Domain.Enums.AchievementRarity.Rare,
            "настойчивость" => Domain.Enums.AchievementRarity.Rare,
            "марафонец" => Domain.Enums.AchievementRarity.Epic,
            "идеальный ученик" => Domain.Enums.AchievementRarity.Legendary,
            _ => Domain.Enums.AchievementRarity.Common
        };
    }
} 