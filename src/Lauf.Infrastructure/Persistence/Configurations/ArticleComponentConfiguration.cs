using Lauf.Domain.Enums;
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

        // ReadingTimeMinutes теперь вычисляемое свойство, не хранится в БД
        builder.Ignore(a => a.ReadingTimeMinutes);
    }
}