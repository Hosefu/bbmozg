using Lauf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Users;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация сущности UserAchievement для Entity Framework
/// </summary>
public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("UserAchievements");

        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.AchievementId)
            .IsRequired();

        builder.Property(x => x.EarnedAt)
            .IsRequired();

        // Связи
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Achievement)
            .WithMany()
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.AchievementId);
        builder.HasIndex(x => x.EarnedAt);
        
        // Уникальное ограничение - пользователь может получить достижение только один раз
        builder.HasIndex(x => new { x.UserId, x.AchievementId })
            .IsUnique();
    }
}