using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Упрощенная конфигурация сущности FlowAssignment для Entity Framework (этап 7)
/// </summary>
public class SimpleFlowAssignmentConfiguration : IEntityTypeConfiguration<FlowAssignment>
{
    public void Configure(EntityTypeBuilder<FlowAssignment> builder)
    {
        builder.ToTable("FlowAssignments");

        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Основные свойства
        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.FlowId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(AssignmentStatus.Assigned);

        builder.Property(x => x.DueDate);
        builder.Property(x => x.CompletedAt);

        // Аудит
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        // Связи
        builder.HasOne(x => x.User)
            .WithMany(x => x.FlowAssignments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Flow)
            .WithMany()
            .HasForeignKey(x => x.FlowId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с назначившим пользователем
        builder.HasOne(x => x.AssignedBy)
            .WithMany() // Не добавляем обратную навигацию, чтобы избежать циклических ссылок
            .HasForeignKey(x => x.AssignedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с куратором (бадди)
        builder.HasOne(x => x.Buddy)
            .WithMany() // Не добавляем обратную навигацию
            .HasForeignKey(x => x.BuddyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Дополнительные свойства
        builder.Property(x => x.AssignedById)
            .IsRequired();

        builder.Property(x => x.BuddyId)
            .IsRequired(false);

        // Индексы
        builder.HasIndex(x => new { x.UserId, x.FlowId });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.DueDate);
        builder.HasIndex(x => x.CreatedAt);
    }
}