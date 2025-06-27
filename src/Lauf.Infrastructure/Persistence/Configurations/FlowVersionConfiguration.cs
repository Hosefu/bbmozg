using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Lauf.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для FlowVersion
/// </summary>
public class FlowVersionConfiguration : IEntityTypeConfiguration<FlowVersion>
{
    public void Configure(EntityTypeBuilder<FlowVersion> builder)
    {
        builder.ToTable("FlowVersions");

        // Первичный ключ
        builder.HasKey(fv => fv.Id);

        // Обязательные поля
        builder.Property(fv => fv.OriginalId)
            .IsRequired()
            .HasComment("Идентификатор оригинального потока");

        builder.Property(fv => fv.Version)
            .IsRequired()
            .HasComment("Номер версии");

        builder.Property(fv => fv.IsActive)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Является ли версия активной");

        builder.Property(fv => fv.Title)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Название потока");

        builder.Property(fv => fv.Description)
            .IsRequired()
            .HasMaxLength(1000)
            .HasComment("Описание потока");

        builder.Property(fv => fv.Tags)
            .IsRequired()
            .HasDefaultValue("")
            .HasComment("Теги потока (разделенные запятыми)");

        builder.Property(fv => fv.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(FlowStatus.Draft)
            .HasComment("Статус потока");

        builder.Property(fv => fv.Priority)
            .IsRequired()
            .HasComment("Приоритет потока");

        builder.Property(fv => fv.IsRequired)
            .IsRequired()
            .HasComment("Является ли поток обязательным");

        builder.Property(fv => fv.CreatedById)
            .IsRequired()
            .HasComment("Идентификатор создателя");

        // Временные метки
        builder.Property(fv => fv.CreatedAt)
            .IsRequired()
            .HasComment("Дата создания версии");

        builder.Property(fv => fv.UpdatedAt)
            .IsRequired()
            .HasComment("Дата последнего обновления версии");

        builder.Property(fv => fv.PublishedAt)
            .HasComment("Дата публикации (если опубликован)");

        // Уникальные ограничения
        builder.HasIndex(fv => new { fv.OriginalId, fv.Version })
            .IsUnique()
            .HasDatabaseName("IX_FlowVersions_OriginalId_Version");

        // Уникальное ограничение на активную версию
        builder.HasIndex(fv => new { fv.OriginalId, fv.IsActive })
            .IsUnique()
            .HasFilter($"{nameof(FlowVersion.IsActive)} = 1")
            .HasDatabaseName("IX_FlowVersions_OriginalId_Active");

        // Индексы для производительности
        builder.HasIndex(fv => fv.OriginalId)
            .HasDatabaseName("IX_FlowVersions_OriginalId");

        builder.HasIndex(fv => fv.Version)
            .HasDatabaseName("IX_FlowVersions_Version");

        builder.HasIndex(fv => fv.IsActive)
            .HasFilter($"{nameof(FlowVersion.IsActive)} = 1")
            .HasDatabaseName("IX_FlowVersions_Active");

        builder.HasIndex(fv => fv.Status)
            .HasDatabaseName("IX_FlowVersions_Status");

        builder.HasIndex(fv => fv.CreatedAt)
            .HasDatabaseName("IX_FlowVersions_CreatedAt");

        // Навигационные свойства
        builder.HasMany(fv => fv.StepVersions)
            .WithOne(sv => sv.FlowVersion)
            .HasForeignKey(sv => sv.FlowVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(fv => fv.Assignments)
            .WithOne(a => a.FlowVersion)
            .HasForeignKey(a => a.FlowVersionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}