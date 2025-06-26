using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Базовая конфигурация для компонентов (Table-Per-Type)
/// </summary>
public class ComponentBaseConfiguration : IEntityTypeConfiguration<ComponentBase>
{
    public void Configure(EntityTypeBuilder<ComponentBase> builder)
    {
        // Настройка Table-Per-Type наследования
        builder.ToTable("Components");
        
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
            .HasDefaultValue(ComponentStatus.Draft);

        builder.Property(x => x.EstimatedDurationMinutes)
            .IsRequired()
            .HasDefaultValue(15);

        builder.Property(x => x.MaxAttempts)
            .IsRequired(false);

        builder.Property(x => x.MinPassingScore)
            .IsRequired(false);

        builder.Property(x => x.Instructions)
            .HasMaxLength(2000);

        // Аудит
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Индексы
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Title);
    }
}