using Lauf.Domain.Entities.Users;
using Lauf.Domain.Interfaces.Repositories;

namespace Lauf.Application.Services;

/// <summary>
/// Сервис для расчета прогресса достижений
/// </summary>
public class AchievementCalculationService
{
    private readonly IUserProgressRepository _progressRepository;
    private readonly IFlowAssignmentRepository _assignmentRepository;
    public AchievementCalculationService(
        IUserProgressRepository progressRepository,
        IFlowAssignmentRepository assignmentRepository)
    {
        _progressRepository = progressRepository;
        _assignmentRepository = assignmentRepository;
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
        // Новая архитектура - упрощенные критерии достижений
        var completedAssignments = await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken);
        var newAchievements = new List<Achievement>();

        // Проверяем базовые критерии достижений
        if (completedAssignments.Count >= 1)
            newAchievements.Add(CreateAchievement("Первые шаги", "Завершил первый поток обучения"));

        if (completedAssignments.Count >= 3)
            newAchievements.Add(CreateAchievement("Быстрый старт", "Завершил 3 потока"));

        if (completedAssignments.Count >= 10)
            newAchievements.Add(CreateAchievement("Эксперт", "Завершил 10 потоков"));

        return newAchievements;
    }

    #region Private Achievement Calculation Methods

    private async Task<decimal> CalculateFirstStepsProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var completedCount = (await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken)).Count;
        return Math.Min(100, completedCount * 100);
    }

    private async Task<decimal> CalculateFastStartProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var completedCount = (await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken)).Count;
        var progress = (decimal)completedCount / 3 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculatePersistenceProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Упрощенная логика - базируется на количестве назначений
        var totalCount = (await _assignmentRepository.GetByUserIdAsync(userId, cancellationToken)).Count;
        var progress = (decimal)totalCount / 10 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculateMarathonProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var completedCount = (await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken)).Count;
        var progress = (decimal)completedCount / 20 * 100;
        return Math.Min(100, progress);
    }

    private async Task<decimal> CalculatePerfectStudentProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Упрощенная логика - базируется на процентном соотношении завершенных потоков
        var allAssignments = await _assignmentRepository.GetByUserIdAsync(userId, cancellationToken);
        var completedAssignments = await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken);
        
        if (allAssignments.Count == 0) return 0;
        return (decimal)completedAssignments.Count / allAssignments.Count * 100;
    }

    private async Task<decimal> CalculateTeamPlayerProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Заглушка - требует данные о взаимодействии с коллегами
        await Task.CompletedTask;
        return 0;
    }

    private async Task<decimal> CalculateExpertProgressAsync(Guid userId, CancellationToken cancellationToken)
    {
        var completedCount = (await _assignmentRepository.GetCompletedByUserIdAsync(userId, cancellationToken)).Count;
        var progress = (decimal)completedCount / 10 * 100;
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
        var assignedCount = (await _assignmentRepository.GetByUserIdAsync(userId, cancellationToken)).Count;
        var progress = (decimal)assignedCount / 20 * 100;
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