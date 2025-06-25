using Microsoft.EntityFrameworkCore;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeder для создания предустановленных достижений в системе
/// </summary>
public static class AchievementSeeder
{
    /// <summary>
    /// Добавляет предустановленные достижения в базу данных
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Проверяем, есть ли уже достижения в базе
        if (await context.Achievements.AnyAsync())
        {
            return; // Данные уже есть
        }

        var achievements = new List<Achievement>
        {
            // Базовые достижения
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Первые шаги",
                Description = "Завершили свой первый поток обучения",
                Rarity = AchievementRarity.Common,
                IconUrl = "🎯",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Быстрый старт",
                Description = "Завершили 3 потока обучения за первую неделю",
                Rarity = AchievementRarity.Rare,
                IconUrl = "⚡",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Настойчивость",
                Description = "Обучались непрерывно в течение 30 дней",
                Rarity = AchievementRarity.Rare,
                IconUrl = "💪",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Марафонец",
                Description = "Потратили более 100 часов на обучение",
                Rarity = AchievementRarity.Epic,
                IconUrl = "🏃‍♂️",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Идеальный ученик",
                Description = "Достигли 95%+ прогресса по всем потокам",
                Rarity = AchievementRarity.Legendary,
                IconUrl = "🌟",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Социальные достижения
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Командный игрок",
                Description = "Активно взаимодействовали с коллегами в процессе обучения",
                Rarity = AchievementRarity.Rare,
                IconUrl = "🤝",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Наставник",
                Description = "Помогли 5 новичкам в их первых потоках обучения",
                Rarity = AchievementRarity.Rare,
                IconUrl = "👨‍🏫",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Продвинутые достижения
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Эксперт",
                Description = "Завершили 10 потоков обучения с отличными результатами",
                Rarity = AchievementRarity.Epic,
                IconUrl = "🎓",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Исследователь",
                Description = "Изучили 20 различных потоков обучения",
                Rarity = AchievementRarity.Rare,
                IconUrl = "🔍",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Новатор",
                Description = "Предложили улучшения, которые были внедрены в систему",
                Rarity = AchievementRarity.Legendary,
                IconUrl = "💡",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Временные достижения
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Ранняя пташка",
                Description = "Начинали обучение до 8:00 утра в течение недели",
                Rarity = AchievementRarity.Rare,
                IconUrl = "🌅",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Полуночник",
                Description = "Обучались после 22:00 в течение недели",
                Rarity = AchievementRarity.Rare,
                IconUrl = "🌙",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },

            // Специальные достижения
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Безупречность",
                Description = "Завершили поток без единой ошибки",
                Rarity = AchievementRarity.Epic,
                IconUrl = "💎",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Молния",
                Description = "Завершили поток в рекордно короткие сроки",
                Rarity = AchievementRarity.Rare,
                IconUrl = "⚡",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Achievement
            {
                Id = Guid.NewGuid(),
                Title = "Легенда",
                Description = "Получили все остальные достижения",
                Rarity = AchievementRarity.Legendary,
                IconUrl = "👑",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await context.Achievements.AddRangeAsync(achievements);
        await context.SaveChangesAsync();
    }
} 