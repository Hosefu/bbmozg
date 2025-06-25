using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Упрощенная конфигурация сущности Flow для Entity Framework (этап 7)
/// </summary>
public class SimpleFlowConfiguration : IEntityTypeConfiguration<Flow>
{
    public void Configure(EntityTypeBuilder<Flow> builder)
    {
        builder.ToTable("Flows");

        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(FlowStatus.Draft);

        builder.Property(x => x.Version)
            .IsRequired()
            .HasDefaultValue(1);

        // Аудит
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Связь с настройками (один к одному)
        builder.HasOne(x => x.Settings)
            .WithOne(x => x.Flow)
            .HasForeignKey<FlowSettings>(x => x.FlowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с шагами (один ко многим)
        builder.HasMany(x => x.Steps)
            .WithOne(x => x.Flow)
            .HasForeignKey(x => x.FlowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Title);
    }
}