using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");

        // Primary Key
        builder.HasKey(o => o.Id);

        // Properties
        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.IsCorrect)
            .IsRequired();

        builder.Property(o => o.Order)
            .IsRequired();

        builder.Property(o => o.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(o => o.Points)
            .IsRequired()
            .HasDefaultValue(1);

        // Индексы
        builder.HasIndex(o => o.IsCorrect)
            .HasDatabaseName("IX_QuestionOptions_IsCorrect");

        builder.HasIndex(o => o.Order)
            .HasDatabaseName("IX_QuestionOptions_Order");
    }
} 