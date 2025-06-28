using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация EF для FlowContent
/// </summary>
public class FlowContentConfiguration : IEntityTypeConfiguration<FlowContent>
{
    public void Configure(EntityTypeBuilder<FlowContent> builder)
    {
        builder.ToTable("FlowContents");

        // Первичный ключ
        builder.HasKey(fc => fc.Id);

        // Свойства
        builder.Property(fc => fc.FlowId)
            .IsRequired();

        builder.Property(fc => fc.Version)
            .IsRequired();

        builder.Property(fc => fc.CreatedAt)
            .IsRequired();

        builder.Property(fc => fc.CreatedBy)
            .IsRequired();

        // Индексы
        builder.HasIndex(fc => fc.FlowId);
        builder.HasIndex(fc => new { fc.FlowId, fc.Version })
            .IsUnique()
            .HasDatabaseName("IX_FlowContents_FlowId_Version");

        // Связь с Flow настроена в SimpleFlowConfiguration
        // чтобы избежать дублирования конфигурации

        // Связь с создателем
        builder.HasOne(fc => fc.CreatedByUser)
            .WithMany()
            .HasForeignKey(fc => fc.CreatedBy)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(fc => fc.Steps)
            .WithOne(fs => fs.FlowContent)
            .HasForeignKey(fs => fs.FlowContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}