using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Entities.Components;

namespace Lauf.Infrastructure.Persistence.Configurations;

/// <summary>
/// Связующая сущность между FlowStep и Component
/// </summary>
public class FlowStepComponentLink
{
    /// <summary>
    /// Уникальный идентификатор связи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор шага потока
    /// </summary>
    public Guid FlowStepId { get; set; }

    /// <summary>
    /// Идентификатор компонента
    /// </summary>
    public Guid ComponentId { get; set; }

    /// <summary>
    /// Порядковый номер компонента в шаге
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Обязательный ли компонент для завершения шага
    /// </summary>
    public bool IsRequired { get; set; } = true;

    /// <summary>
    /// Дата создания связи
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Навигационные свойства
    /// </summary>
    public virtual FlowStep FlowStep { get; set; } = null!;
    public virtual ComponentBase Component { get; set; } = null!;
}

/// <summary>
/// Конфигурация для связи FlowStep и Component
/// </summary>
public class FlowStepComponentLinkConfiguration : IEntityTypeConfiguration<FlowStepComponentLink>
{
    public void Configure(EntityTypeBuilder<FlowStepComponentLink> builder)
    {
        // Настройка таблицы
        builder.ToTable("FlowStepComponentLinks");

        // Первичный ключ
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        // Свойства
        builder.Property(x => x.FlowStepId)
            .IsRequired();

        builder.Property(x => x.ComponentId)
            .IsRequired();

        builder.Property(x => x.Order)
            .IsRequired();

        builder.Property(x => x.IsRequired)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        // Связи
        builder.HasOne(x => x.FlowStep)
            .WithMany() // FlowStep не имеет прямой коллекции компонентов
            .HasForeignKey(x => x.FlowStepId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Component)
            .WithMany() // ComponentBase не знает о связях с шагами
            .HasForeignKey(x => x.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Составные индексы
        builder.HasIndex(x => new { x.FlowStepId, x.Order })
            .IsUnique();

        builder.HasIndex(x => new { x.FlowStepId, x.ComponentId })
            .IsUnique(); // Один компонент не может быть добавлен в один шаг дважды

        // Индексы
        builder.HasIndex(x => x.FlowStepId);
        builder.HasIndex(x => x.ComponentId);
        builder.HasIndex(x => x.Order);
        builder.HasIndex(x => x.IsRequired);
    }
}