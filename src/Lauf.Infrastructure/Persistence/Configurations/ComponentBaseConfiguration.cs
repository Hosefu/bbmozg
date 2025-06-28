using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Базовая конфигурация для компонентов (Table-Per-Type) - минимальная для работы
/// </summary>
public class ComponentBaseConfiguration : IEntityTypeConfiguration<ComponentBase>
{
    public void Configure(EntityTypeBuilder<ComponentBase> builder)
    {
        // Настройка Table-Per-Type наследования
        builder.ToTable("Components");
        
        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Content).IsRequired().HasMaxLength(10000);
        builder.Property(x => x.Order).IsRequired().HasMaxLength(50);
        builder.Property(x => x.IsRequired).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.IsEnabled).IsRequired().HasDefaultValue(true);

        // Связь с FlowStep
        builder.Property(x => x.FlowStepId).IsRequired();
        builder.HasOne<FlowStep>()
            .WithMany(s => s.Components)
            .HasForeignKey(x => x.FlowStepId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}