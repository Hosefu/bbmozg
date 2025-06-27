using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для QuizOptionVersion
/// </summary>
public class QuizOptionVersionConfiguration : IEntityTypeConfiguration<QuizOptionVersion>
{
    public void Configure(EntityTypeBuilder<QuizOptionVersion> builder)
    {
        builder.ToTable("QuizOptionVersions");

        // Первичный ключ
        builder.HasKey(ov => ov.Id);

        // Обязательные поля
        builder.Property(ov => ov.QuizVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии квиза");

        builder.Property(ov => ov.Text)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Текст варианта ответа");

        builder.Property(ov => ov.IsCorrect)
            .IsRequired()
            .HasComment("Является ли ответ правильным");

        builder.Property(ov => ov.Points)
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Количество баллов за этот ответ");

        builder.Property(ov => ov.Order)
            .IsRequired()
            .HasComment("Порядковый номер варианта ответа");

        builder.Property(ov => ov.Explanation)
            .HasMaxLength(1000)
            .HasComment("Объяснение ответа");

        // Индексы для производительности
        builder.HasIndex(ov => ov.QuizVersionId)
            .HasDatabaseName("IX_QuizOptionVersions_QuizVersionId");

        builder.HasIndex(ov => new { ov.QuizVersionId, ov.Order })
            .IsUnique()
            .HasDatabaseName("IX_QuizOptionVersions_QuizVersionId_Order");

        builder.HasIndex(ov => ov.IsCorrect)
            .HasDatabaseName("IX_QuizOptionVersions_IsCorrect");

        builder.HasIndex(ov => ov.Points)
            .HasDatabaseName("IX_QuizOptionVersions_Points");

        // Навигационные свойства
        builder.HasOne(ov => ov.QuizVersion)
            .WithMany(qv => qv.Options)
            .HasForeignKey(ov => ov.QuizVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}