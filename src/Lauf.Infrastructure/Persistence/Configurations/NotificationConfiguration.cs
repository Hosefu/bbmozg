using Lauf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Notifications;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для сущности Notification
/// </summary>
public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        // Таблица
        builder.ToTable("Notifications");

        // Первичный ключ
        builder.HasKey(n => n.Id);

        // Свойства
        builder.Property(n => n.Id)
            .ValueGeneratedNever();

        builder.Property(n => n.UserId)
            .IsRequired();

        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Channel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(n => n.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.Metadata)
            .HasMaxLength(4000);

        builder.Property(n => n.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(n => n.RelatedEntityType)
            .HasMaxLength(100);

        builder.Property(n => n.CreatedAt)
            .IsRequired();

        builder.Property(n => n.ScheduledAt)
            .IsRequired();

        builder.Property(n => n.AttemptCount)
            .HasDefaultValue(0);

        builder.Property(n => n.MaxAttempts)
            .HasDefaultValue(3);

        // Индексы
        builder.HasIndex(n => n.UserId)
            .HasDatabaseName("IX_Notifications_UserId");

        builder.HasIndex(n => new { n.Status, n.ScheduledAt })
            .HasDatabaseName("IX_Notifications_Status_ScheduledAt");

        builder.HasIndex(n => n.Type)
            .HasDatabaseName("IX_Notifications_Type");

        builder.HasIndex(n => n.Priority)
            .HasDatabaseName("IX_Notifications_Priority");

        builder.HasIndex(n => n.CreatedAt)
            .HasDatabaseName("IX_Notifications_CreatedAt");

        builder.HasIndex(n => new { n.RelatedEntityType, n.RelatedEntityId })
            .HasDatabaseName("IX_Notifications_RelatedEntity");

        // Связи
        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}