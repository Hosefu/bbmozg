using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для ArticleComponent
/// </summary>
public class ArticleComponentConfiguration : IEntityTypeConfiguration<ArticleComponent>
{
    public void Configure(EntityTypeBuilder<ArticleComponent> builder)
    {
        builder.ToTable("ArticleComponents");

        // Наследование от ComponentBase
        builder.HasBaseType<ComponentBase>();

        // Основные свойства
        builder.Property(a => a.Content)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(a => a.ReadingTimeMinutes)
            .IsRequired()
            .HasDefaultValue(15);

        // Индексы для производительности
        builder.HasIndex(a => a.ReadingTimeMinutes)
            .HasDatabaseName("IX_ArticleComponents_ReadingTimeMinutes");
    }
}