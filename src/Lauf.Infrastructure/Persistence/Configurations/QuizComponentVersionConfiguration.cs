using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для QuizComponentVersion
/// </summary>
public class QuizComponentVersionConfiguration : IEntityTypeConfiguration<QuizComponentVersion>
{
    public void Configure(EntityTypeBuilder<QuizComponentVersion> builder)
    {
        builder.ToTable("QuizComponentVersions");

        // Первичный ключ
        builder.HasKey(qv => qv.ComponentVersionId);

        // Обязательные поля
        builder.Property(qv => qv.ComponentVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии компонента");

        builder.Property(qv => qv.PassingScore)
            .IsRequired()
            .HasDefaultValue(80)
            .HasComment("Проходной балл (в процентах)");

        builder.Property(qv => qv.TimeLimitMinutes)
            .HasComment("Ограничение по времени в минутах");

        builder.Property(qv => qv.AllowMultipleAttempts)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Разрешены ли множественные попытки");

        builder.Property(qv => qv.ShowCorrectAnswers)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Показывать ли правильные ответы после завершения");

        builder.Property(qv => qv.ShuffleQuestions)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Перемешивать ли вопросы");

        builder.Property(qv => qv.ShuffleAnswers)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Перемешивать ли варианты ответов");

        // Индексы для производительности
        builder.HasIndex(qv => qv.PassingScore)
            .HasDatabaseName("IX_QuizComponentVersions_PassingScore");

        builder.HasIndex(qv => qv.TimeLimitMinutes)
            .HasDatabaseName("IX_QuizComponentVersions_TimeLimit");

        builder.HasIndex(qv => qv.AllowMultipleAttempts)
            .HasDatabaseName("IX_QuizComponentVersions_MultipleAttempts");

        // Навигационные свойства
        builder.HasOne(qv => qv.ComponentVersion)
            .WithOne(cv => cv.QuizVersion)
            .HasForeignKey<QuizComponentVersion>(qv => qv.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(qv => qv.Options)
            .WithOne(ov => ov.QuizVersion)
            .HasForeignKey(ov => ov.QuizVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}