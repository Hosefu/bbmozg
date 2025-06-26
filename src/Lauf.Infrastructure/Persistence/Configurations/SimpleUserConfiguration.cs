using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Users;
using Lauf.Domain.ValueObjects;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Упрощенная конфигурация сущности User для Entity Framework (этап 7)
/// </summary>
public class SimpleUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TelegramUsername)
            .HasMaxLength(100);

        // Telegram User ID как owned entity
        builder.OwnsOne(x => x.TelegramUserId, telegramBuilder =>
        {
            telegramBuilder.Property(x => x.Value)
                .HasColumnName("TelegramUserId")
                .IsRequired();
        });

        // IsActive поле
        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Даты создания и обновления
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.Property(x => x.LastActiveAt)
            .IsRequired(false);

        // Связь с ролями (многие ко многим)
        builder.HasMany(x => x.Roles)
            .WithMany(x => x.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRoles",
                j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                j =>
                {
                    j.HasKey("UserId", "RoleId");
                    j.ToTable("UserRoles");
                });

        // Связь с назначениями потоков
        builder.HasMany(x => x.FlowAssignments)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы
        builder.HasIndex(x => x.TelegramUserId.Value)
            .IsUnique()
            .HasDatabaseName("IX_Users_TelegramUserId");

        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.CreatedAt);
    }
}