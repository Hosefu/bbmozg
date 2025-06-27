using Lauf.Domain.Enums;
using Lauf.Domain.Entities.Versions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация Entity Framework для TaskComponentVersion
/// </summary>
public class TaskComponentVersionConfiguration : IEntityTypeConfiguration<TaskComponentVersion>
{
    public void Configure(EntityTypeBuilder<TaskComponentVersion> builder)
    {
        builder.ToTable("TaskComponentVersions");

        // Первичный ключ
        builder.HasKey(tv => tv.ComponentVersionId);

        // Обязательные поля
        builder.Property(tv => tv.ComponentVersionId)
            .IsRequired()
            .HasComment("Идентификатор версии компонента");

        builder.Property(tv => tv.Instructions)
            .IsRequired()
            .HasComment("Подробные инструкции по выполнению задания");

        builder.Property(tv => tv.SubmissionType)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(TaskSubmissionType.Text)
            .HasComment("Тип отправки результата");

        builder.Property(tv => tv.MaxFileSize)
            .HasComment("Максимальный размер файла в байтах");

        builder.Property(tv => tv.AllowedFileTypes)
            .HasMaxLength(200)
            .HasComment("Разрешенные типы файлов");

        builder.Property(tv => tv.RequiresMentorApproval)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Требуется ли одобрение наставника");

        builder.Property(tv => tv.AutoApprovalKeywords)
            .HasComment("Ключевые слова для автоматического одобрения");

        // Индексы для производительности
        builder.HasIndex(tv => tv.SubmissionType)
            .HasDatabaseName("IX_TaskComponentVersions_SubmissionType");

        builder.HasIndex(tv => tv.RequiresMentorApproval)
            .HasDatabaseName("IX_TaskComponentVersions_RequiresMentorApproval");

        builder.HasIndex(tv => tv.MaxFileSize)
            .HasDatabaseName("IX_TaskComponentVersions_MaxFileSize");

        // Навигационные свойства
        builder.HasOne(tv => tv.ComponentVersion)
            .WithOne(cv => cv.TaskVersion)
            .HasForeignKey<TaskComponentVersion>(tv => tv.ComponentVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}