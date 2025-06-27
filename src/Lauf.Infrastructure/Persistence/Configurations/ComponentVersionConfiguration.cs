using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для ComponentVersion
/// </summary>
public class ComponentVersionConfiguration : IEntityTypeConfiguration<ComponentVersion>
{
    public void Configure(EntityTypeBuilder<ComponentVersion> builder)
    {
        builder.ToTable("ComponentVersions");

        // Первичный ключ
        builder.HasKey(cv => cv.Id);

        // Обязательные поля
        builder.Property(cv => cv.OriginalId)
            .IsRequired()
            .HasComment("Идентификатор оригинального компонента");

        builder.Property(cv => cv.Version)
            .IsRequired()
            .HasComment("Номер версии");

        builder.Property(cv => cv.IsActive)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Является ли версия активной");

        builder.Property(cv => cv.StepVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии этапа");

        builder.Property(cv => cv.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Название компонента");

        builder.Property(cv => cv.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .HasComment("Описание компонента");

        builder.Property(cv => cv.ComponentType)
            .IsRequired()
            .HasConversion<string>()
            .HasComment("Тип компонента");

        builder.Property(cv => cv.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ComponentStatus.Draft)
            .HasComment("Статус компонента");

        builder.Property(cv => cv.Order)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Порядок компонента (LexoRank)");

        builder.Property(cv => cv.IsRequired)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Является ли компонент обязательным");

        builder.Property(cv => cv.EstimatedDurationMinutes)
            .IsRequired()
            .HasDefaultValue(15)
            .HasComment("Оценочное время выполнения в минутах");

        builder.Property(cv => cv.MaxAttempts)
            .HasComment("Максимальное количество попыток");

        builder.Property(cv => cv.MinPassingScore)
            .HasComment("Минимальный проходной балл");

        builder.Property(cv => cv.Instructions)
            .IsRequired()
            .HasMaxLength(2000)
            .HasDefaultValue("")
            .HasComment("Инструкции для компонента");

        // Временные метки
        builder.Property(cv => cv.CreatedAt)
            .IsRequired()
            .HasComment("Дата создания версии");

        builder.Property(cv => cv.UpdatedAt)
            .IsRequired()
            .HasComment("Дата последнего обновления версии");

        // Уникальные ограничения
        builder.HasIndex(cv => new { cv.OriginalId, cv.Version })
            .IsUnique()
            .HasDatabaseName("IX_ComponentVersions_OriginalId_Version");

        // Уникальное ограничение на активную версию
        builder.HasIndex(cv => new { cv.OriginalId, cv.IsActive })
            .IsUnique()
            .HasFilter($"{nameof(ComponentVersion.IsActive)} = 1")
            .HasDatabaseName("IX_ComponentVersions_OriginalId_Active");

        // Индексы для производительности
        builder.HasIndex(cv => cv.StepVersionId)
            .HasDatabaseName("IX_ComponentVersions_StepVersionId");

        builder.HasIndex(cv => cv.OriginalId)
            .HasDatabaseName("IX_ComponentVersions_OriginalId");

        builder.HasIndex(cv => cv.Version)
            .HasDatabaseName("IX_ComponentVersions_Version");

        builder.HasIndex(cv => cv.IsActive)
            .HasFilter($"{nameof(ComponentVersion.IsActive)} = 1")
            .HasDatabaseName("IX_ComponentVersions_Active");

        builder.HasIndex(cv => cv.ComponentType)
            .HasDatabaseName("IX_ComponentVersions_ComponentType");

        builder.HasIndex(cv => cv.Status)
            .HasDatabaseName("IX_ComponentVersions_Status");

        builder.HasIndex(cv => cv.Order)
            .HasDatabaseName("IX_ComponentVersions_Order");

        builder.HasIndex(cv => cv.CreatedAt)
            .HasDatabaseName("IX_ComponentVersions_CreatedAt");

        // Навигационные свойства
        builder.HasOne(cv => cv.StepVersion)
            .WithMany(sv => sv.ComponentVersions)
            .HasForeignKey(cv => cv.StepVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связи с специализированными версиями компонентов
        builder.HasOne(cv => cv.ArticleVersion)
            .WithOne(av => av.ComponentVersion)
            .HasForeignKey<ArticleComponentVersion>(av => av.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cv => cv.QuizVersion)
            .WithOne(qv => qv.ComponentVersion)
            .HasForeignKey<QuizComponentVersion>(qv => qv.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cv => cv.TaskVersion)
            .WithOne(tv => tv.ComponentVersion)
            .HasForeignKey<TaskComponentVersion>(tv => tv.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}