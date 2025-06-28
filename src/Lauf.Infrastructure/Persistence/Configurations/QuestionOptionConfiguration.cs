using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для вариантов ответов - минимальная для работы
/// </summary>
public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.Text).IsRequired().HasMaxLength(500);
        builder.Property(x => x.IsCorrect).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.Order).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Score).IsRequired().HasDefaultValue(1);

        // Связь с вопросом
        builder.Property(x => x.QuizQuestionId).IsRequired();
        builder.HasOne<QuizQuestion>()
            .WithMany(q => q.Options)
            .HasForeignKey(x => x.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}