using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для компонентов заданий - обновлена под новую архитектуру
/// </summary>
public class TaskComponentConfiguration : IEntityTypeConfiguration<TaskComponent>
{
    public void Configure(EntityTypeBuilder<TaskComponent> builder)
    {
        builder.ToTable("TaskComponents");

        // Настройки базового компонента уже заданы в ComponentBaseConfiguration
        
        // Специфичные для TaskComponent свойства
        builder.Property(x => x.CodeWord)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Score)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(x => x.IsCaseSensitive)
            .IsRequired()
            .HasDefaultValue(false);
    }
}