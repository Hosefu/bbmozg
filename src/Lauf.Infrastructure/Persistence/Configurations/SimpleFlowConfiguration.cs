using Lauf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;

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
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.ActiveContentId)
            .IsRequired(false);


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

        // Связь с активным содержимым (один к одному)
        builder.HasOne(x => x.ActiveContent)
            .WithOne()
            .HasForeignKey<Flow>(x => x.ActiveContentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Связь со всеми версиями содержимого (один ко многим)
        builder.HasMany(x => x.Contents)
            .WithOne(fc => fc.Flow)
            .HasForeignKey(fc => fc.FlowId)
            .OnDelete(DeleteBehavior.Cascade);

        // Связь с создателем (многие к одному)
        builder.HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Индексы
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.CreatedBy);
    }
}