using Lauf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для TaskComponent
/// </summary>
public class TaskComponentConfiguration : IEntityTypeConfiguration<TaskComponent>
{
    public void Configure(EntityTypeBuilder<TaskComponent> builder)
    {
        builder.ToTable("TaskComponents");

        // Наследование от ComponentBase
        builder.HasBaseType<ComponentBase>();

        // Основные свойства
        builder.Property(t => t.Instruction)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.CodeWord)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Hint)
            .IsRequired()
            .HasMaxLength(1000);

        // Индексы для производительности
        builder.HasIndex(t => t.CodeWord)
            .HasDatabaseName("IX_TaskComponents_CodeWord");
    }
}