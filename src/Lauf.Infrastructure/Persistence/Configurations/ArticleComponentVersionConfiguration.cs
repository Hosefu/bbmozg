using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для ArticleComponentVersion
/// </summary>
public class ArticleComponentVersionConfiguration : IEntityTypeConfiguration<ArticleComponentVersion>
{
    public void Configure(EntityTypeBuilder<ArticleComponentVersion> builder)
    {
        builder.ToTable("ArticleComponentVersions");

        // Первичный ключ
        builder.HasKey(av => av.ComponentVersionId);

        // Обязательные поля
        builder.Property(av => av.ComponentVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии компонента");

        builder.Property(av => av.Content)
            .IsRequired()
            .HasComment("Содержимое статьи в формате Markdown");

        builder.Property(av => av.ReadingTimeMinutes)
            .IsRequired()
            .HasDefaultValue(15)
            .HasComment("Время чтения статьи в минутах");

        // Индексы для производительности
        builder.HasIndex(av => av.ReadingTimeMinutes)
            .HasDatabaseName("IX_ArticleComponentVersions_ReadingTime");

        // Навигационные свойства
        builder.HasOne(av => av.ComponentVersion)
            .WithOne(cv => cv.ArticleVersion)
            .HasForeignKey<ArticleComponentVersion>(av => av.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}