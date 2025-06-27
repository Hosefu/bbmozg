using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация EF для FlowAssignmentProgress
/// </summary>
public class FlowAssignmentProgressConfiguration : IEntityTypeConfiguration<FlowAssignmentProgress>
{
    public void Configure(EntityTypeBuilder<FlowAssignmentProgress> builder)
    {
        builder.ToTable("FlowAssignmentProgress");

        // Первичный ключ
        builder.HasKey(fap => fap.Id);

        // Свойства
        builder.Property(fap => fap.FlowAssignmentId)
            .IsRequired();

        builder.Property(fap => fap.ProgressPercent)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(fap => fap.CompletedSteps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(fap => fap.TotalSteps)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(fap => fap.AttemptCount)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(fap => fap.StartedAt)
            .IsRequired(false);

        builder.Property(fap => fap.CompletedAt)
            .IsRequired(false);

        builder.Property(fap => fap.UserFeedback)
            .HasMaxLength(2000)
            .IsRequired(false);

        // Индексы
        builder.HasIndex(fap => fap.FlowAssignmentId)
            .IsUnique()
            .HasDatabaseName("IX_FlowAssignmentProgress_FlowAssignmentId");

        // Связи
        builder.HasOne<FlowAssignment>()
            .WithOne(fa => fa.Progress)
            .HasForeignKey<FlowAssignmentProgress>(fap => fap.FlowAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}