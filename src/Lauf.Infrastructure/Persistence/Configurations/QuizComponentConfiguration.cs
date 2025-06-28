using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для компонентов квизов - обновлена под новую архитектуру
/// </summary>
public class QuizComponentConfiguration : IEntityTypeConfiguration<QuizComponent>
{
    public void Configure(EntityTypeBuilder<QuizComponent> builder)
    {
        builder.ToTable("QuizComponents");

        // Настройки базового компонента уже заданы в ComponentBaseConfiguration
        
        // Связь с вопросами
        builder.HasMany(q => q.Questions)
            .WithOne()
            .HasForeignKey(qq => qq.QuizComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}