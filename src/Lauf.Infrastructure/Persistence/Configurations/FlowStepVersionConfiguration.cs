using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для FlowStepVersion
/// </summary>
public class FlowStepVersionConfiguration : IEntityTypeConfiguration<FlowStepVersion>
{
    public void Configure(EntityTypeBuilder<FlowStepVersion> builder)
    {
        builder.ToTable("FlowStepVersions");

        // Первичный ключ
        builder.HasKey(sv => sv.Id);

        // Обязательные поля
        builder.Property(sv => sv.OriginalId)
            .IsRequired()
            .HasComment("Идентификатор оригинального этапа");

        builder.Property(sv => sv.Version)
            .IsRequired()
            .HasComment("Номер версии");

        builder.Property(sv => sv.IsActive)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Является ли версия активной");

        builder.Property(sv => sv.FlowVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии потока");

        builder.Property(sv => sv.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Название этапа");

        builder.Property(sv => sv.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .HasComment("Описание этапа");

        builder.Property(sv => sv.Order)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Порядок этапа (LexoRank)");

        builder.Property(sv => sv.IsRequired)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Является ли этап обязательным");

        builder.Property(sv => sv.EstimatedDurationMinutes)
            .IsRequired()
            .HasComment("Оценочное время выполнения в минутах");

        builder.Property(sv => sv.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(StepStatus.Draft)
            .HasComment("Статус этапа");

        builder.Property(sv => sv.Instructions)
            .IsRequired()
            .HasMaxLength(2000)
            .HasDefaultValue("")
            .HasComment("Инструкции для этапа");

        builder.Property(sv => sv.Notes)
            .IsRequired()
            .HasDefaultValue("")
            .HasComment("Заметки по этапу");

        // Временные метки
        builder.Property(sv => sv.CreatedAt)
            .IsRequired()
            .HasComment("Дата создания версии");

        builder.Property(sv => sv.UpdatedAt)
            .IsRequired()
            .HasComment("Дата последнего обновления версии");

        // Уникальные ограничения
        builder.HasIndex(sv => new { sv.OriginalId, sv.Version })
            .IsUnique()
            .HasDatabaseName("IX_FlowStepVersions_OriginalId_Version");

        // Уникальное ограничение на активную версию
        builder.HasIndex(sv => new { sv.OriginalId, sv.IsActive })
            .IsUnique()
            .HasFilter($"{nameof(FlowStepVersion.IsActive)} = 1")
            .HasDatabaseName("IX_FlowStepVersions_OriginalId_Active");

        // Индексы для производительности
        builder.HasIndex(sv => sv.FlowVersionId)
            .HasDatabaseName("IX_FlowStepVersions_FlowVersionId");

        builder.HasIndex(sv => sv.OriginalId)
            .HasDatabaseName("IX_FlowStepVersions_OriginalId");

        builder.HasIndex(sv => sv.Version)
            .HasDatabaseName("IX_FlowStepVersions_Version");

        builder.HasIndex(sv => sv.IsActive)
            .HasFilter($"{nameof(FlowStepVersion.IsActive)} = 1")
            .HasDatabaseName("IX_FlowStepVersions_Active");

        builder.HasIndex(sv => sv.Order)
            .HasDatabaseName("IX_FlowStepVersions_Order");

        builder.HasIndex(sv => sv.Status)
            .HasDatabaseName("IX_FlowStepVersions_Status");

        builder.HasIndex(sv => sv.CreatedAt)
            .HasDatabaseName("IX_FlowStepVersions_CreatedAt");

        // Навигационные свойства
        builder.HasOne(sv => sv.FlowVersion)
            .WithMany(fv => fv.StepVersions)
            .HasForeignKey(sv => sv.FlowVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sv => sv.ComponentVersions)
            .WithOne(cv => cv.StepVersion)
            .HasForeignKey(cv => cv.StepVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}