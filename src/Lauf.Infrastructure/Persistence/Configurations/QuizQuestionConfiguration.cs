using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация EF для QuizQuestion
/// </summary>
public class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.ToTable("QuizQuestions");

        // Первичный ключ
        builder.HasKey(qq => qq.Id);

        // Свойства
        builder.Property(qq => qq.QuizComponentId)
            .IsRequired();

        builder.Property(qq => qq.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(qq => qq.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(qq => qq.Order)
            .IsRequired()
            .HasMaxLength(50);

        // Индексы
        builder.HasIndex(qq => qq.QuizComponentId);
        builder.HasIndex(qq => qq.Order);

        // Связи
        builder.HasOne(qq => qq.QuizComponent)
            .WithMany(qc => qc.Questions)
            .HasForeignKey(qq => qq.QuizComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(qq => qq.Options)
            .WithOne(qo => qo.QuizQuestion)
            .HasForeignKey(qo => qo.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}