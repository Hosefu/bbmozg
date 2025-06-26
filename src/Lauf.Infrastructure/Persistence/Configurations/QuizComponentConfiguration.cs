using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для QuizComponent
/// </summary>
public class QuizComponentConfiguration : IEntityTypeConfiguration<QuizComponent>
{
    public void Configure(EntityTypeBuilder<QuizComponent> builder)
    {
        builder.ToTable("QuizComponents");

        // Наследование от ComponentBase
        builder.HasBaseType<ComponentBase>();

        // Основные свойства
        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(1000);

        // Навигационные свойства
        builder.HasMany(q => q.Options)
            .WithOne()
            .HasForeignKey("QuizComponentId")
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы для производительности
        builder.HasIndex(q => q.QuestionText)
            .HasDatabaseName("IX_QuizComponents_QuestionText");
    }
}