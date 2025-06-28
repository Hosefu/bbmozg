using Lauf.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;

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

        builder.Property(x => x.FlowContentId)
            .IsRequired();

        builder.Property(x => x.AssignedBy)
            .IsRequired();

        // CompletedAt и Buddy теперь вычисляемые свойства, не хранятся в БД
        builder.Ignore(x => x.CompletedAt);
        builder.Ignore(x => x.Buddy);
        builder.Ignore(x => x.Deadline);

        // Аудит
        builder.Property(x => x.AssignedAt)
            .IsRequired();

        // Связи
        builder.HasOne(x => x.User)
            .WithMany(x => x.FlowAssignments)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Flow)
            .WithMany(x => x.Assignments)
            .HasForeignKey(x => x.FlowId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с контентом потока
        builder.HasOne(x => x.FlowContent)
            .WithMany(fc => fc.Assignments)
            .HasForeignKey(x => x.FlowContentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с назначившим пользователем
        builder.HasOne(x => x.AssignedByUser)
            .WithMany()
            .HasForeignKey(x => x.AssignedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Связь с наставниками (многие ко многим через промежуточную таблицу)
        builder.HasMany(x => x.Buddies)
            .WithMany()
            .UsingEntity("FlowAssignmentBuddies");

        // Связь с прогрессом (один к одному)
        builder.HasOne(x => x.Progress)
            .WithOne(p => p.FlowAssignment)
            .HasForeignKey<FlowAssignmentProgress>(x => x.FlowAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Дополнительные свойства - AssignedById убрано из новой архитектуры

        // Индексы
        builder.HasIndex(x => new { x.UserId, x.FlowId });
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.AssignedAt);
    }
}