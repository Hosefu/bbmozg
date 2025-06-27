using Lauf.Domain.Entities.Versions;
using Lauf.Domain.Enums;
using Lauf.Domain.Interfaces;
using Lauf.Domain.Interfaces.Repositories;
using Lauf.Domain.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lauf.Infrastructure.Services;

/// <summary>
/// Реализация сервиса для управления версионированием сущностей
/// </summary>
public class VersioningService : IVersioningService
{
    private readonly IFlowVersionRepository _flowVersionRepository;
    private readonly IComponentVersionRepository _componentVersionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VersioningService> _logger;

    public VersioningService(
        IFlowVersionRepository flowVersionRepository,
        IComponentVersionRepository componentVersionRepository,
        IUnitOfWork unitOfWork,
        ILogger<VersioningService> logger)
    {
        _flowVersionRepository = flowVersionRepository ?? throw new ArgumentNullException(nameof(flowVersionRepository));
        _componentVersionRepository = componentVersionRepository ?? throw new ArgumentNullException(nameof(componentVersionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Создать новую версию потока и всех его компонентов
    /// </summary>
    public async Task<FlowVersion> CreateNewFlowVersionAsync(FlowVersion sourceFlowVersion, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Создание новой версии потока на основе версии {SourceVersion} (ID: {SourceId})", 
            sourceFlowVersion.Version, sourceFlowVersion.Id);

        try
        {
            // Получаем исходную версию с полными данными
            var sourceWithDetails = await _flowVersionRepository.GetByIdWithDetailsAsync(sourceFlowVersion.Id, cancellationToken);
            if (sourceWithDetails == null)
            {
                throw new InvalidOperationException($"Исходная версия потока {sourceFlowVersion.Id} не найдена");
            }

            // Получаем следующий номер версии
            var maxVersion = await _flowVersionRepository.GetMaxVersionAsync(sourceFlowVersion.OriginalId, cancellationToken);
            var newVersion = maxVersion + 1;

            // Создаем новую версию потока
            var newFlowVersion = new FlowVersion(
                sourceFlowVersion.OriginalId,
                newVersion,
                sourceFlowVersion.Title,
                sourceFlowVersion.Description,
                sourceFlowVersion.Tags,
                sourceFlowVersion.Status,
                sourceFlowVersion.Priority,
                sourceFlowVersion.IsRequired,
                sourceFlowVersion.CreatedById,
                false // Новая версия не активна по умолчанию
            );

            // Добавляем новую версию потока
            await _flowVersionRepository.AddAsync(newFlowVersion, cancellationToken);

            // Создаем версии всех этапов
            foreach (var sourceStepVersion in sourceWithDetails.StepVersions.OrderBy(sv => sv.Order))
            {
                var newStepVersion = await CreateNewStepVersionAsync(sourceStepVersion, newFlowVersion.Id, cancellationToken);
                newFlowVersion.AddStepVersion(newStepVersion);
            }

            _logger.LogInformation("Создана новая версия потока {NewVersion} с {StepCount} этапами", 
                newVersion, newFlowVersion.StepVersions.Count);

            return newFlowVersion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании новой версии потока на основе версии {SourceVersion}", 
                sourceFlowVersion.Version);
            throw;
        }
    }

    /// <summary>
    /// Создать новую версию этапа и всех его компонентов
    /// </summary>
    public async Task<FlowStepVersion> CreateNewStepVersionAsync(FlowStepVersion sourceStepVersion, Guid newFlowVersionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Создание новой версии этапа на основе версии {SourceVersion} (ID: {SourceId})", 
            sourceStepVersion.Version, sourceStepVersion.Id);

        try
        {
            // Получаем следующий номер версии для этапа
            var maxVersion = await GetMaxStepVersionAsync(sourceStepVersion.OriginalId, cancellationToken);
            var newVersion = maxVersion + 1;

            // Создаем новую версию этапа
            var newStepVersion = new FlowStepVersion(
                sourceStepVersion.OriginalId,
                newVersion,
                newFlowVersionId,
                sourceStepVersion.Title,
                sourceStepVersion.Description,
                sourceStepVersion.Order,
                sourceStepVersion.IsRequired,
                sourceStepVersion.EstimatedDurationMinutes,
                sourceStepVersion.Status,
                sourceStepVersion.Instructions,
                sourceStepVersion.Notes,
                false // Новая версия не активна по умолчанию
            );

            // Создаем версии всех компонентов
            foreach (var sourceComponentVersion in sourceStepVersion.ComponentVersions.OrderBy(cv => cv.Order))
            {
                var newComponentVersion = await CreateNewComponentVersionAsync(sourceComponentVersion, newStepVersion.Id, cancellationToken);
                newStepVersion.AddComponentVersion(newComponentVersion);
            }

            _logger.LogDebug("Создана новая версия этапа {NewVersion} с {ComponentCount} компонентами", 
                newVersion, newStepVersion.ComponentVersions.Count);

            return newStepVersion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании новой версии этапа на основе версии {SourceVersion}", 
                sourceStepVersion.Version);
            throw;
        }
    }

    /// <summary>
    /// Создать новую версию компонента
    /// </summary>
    public async Task<ComponentVersion> CreateNewComponentVersionAsync(ComponentVersion sourceComponentVersion, Guid newStepVersionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Создание новой версии компонента на основе версии {SourceVersion} (ID: {SourceId})", 
            sourceComponentVersion.Version, sourceComponentVersion.Id);

        try
        {
            // Получаем исходную версию с полными данными
            var sourceWithDetails = await _componentVersionRepository.GetByIdWithDetailsAsync(sourceComponentVersion.Id, cancellationToken);
            if (sourceWithDetails == null)
            {
                throw new InvalidOperationException($"Исходная версия компонента {sourceComponentVersion.Id} не найдена");
            }

            // Получаем следующий номер версии для компонента
            var maxVersion = await _componentVersionRepository.GetMaxVersionAsync(sourceComponentVersion.OriginalId, cancellationToken);
            var newVersion = maxVersion + 1;

            // Создаем новую версию компонента
            var newComponentVersion = new ComponentVersion(
                sourceComponentVersion.OriginalId,
                newVersion,
                newStepVersionId,
                sourceComponentVersion.Title,
                sourceComponentVersion.Description,
                sourceComponentVersion.ComponentType,
                sourceComponentVersion.Status,
                sourceComponentVersion.Order,
                sourceComponentVersion.IsRequired,
                sourceComponentVersion.EstimatedDurationMinutes,
                sourceComponentVersion.MaxAttempts,
                sourceComponentVersion.MinPassingScore,
                sourceComponentVersion.Instructions,
                false // Новая версия не активна по умолчанию
            );

            // Добавляем новую версию компонента
            await _componentVersionRepository.AddAsync(newComponentVersion, cancellationToken);

            // Копируем специализированные данные в зависимости от типа компонента
            await CopySpecializedComponentDataAsync(sourceWithDetails, newComponentVersion, cancellationToken);

            _logger.LogDebug("Создана новая версия компонента {ComponentType} {NewVersion}", 
                newComponentVersion.ComponentType, newVersion);

            return newComponentVersion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании новой версии компонента на основе версии {SourceVersion}", 
                sourceComponentVersion.Version);
            throw;
        }
    }

    /// <summary>
    /// Активировать версию потока (деактивировать предыдущую активную версию)
    /// </summary>
    public async Task ActivateFlowVersionAsync(Guid flowVersionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Активация версии потока {FlowVersionId}", flowVersionId);

        try
        {
            await _flowVersionRepository.ActivateVersionAsync(flowVersionId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Версия потока {FlowVersionId} успешно активирована", flowVersionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при активации версии потока {FlowVersionId}", flowVersionId);
            throw;
        }
    }

    /// <summary>
    /// Активировать версию этапа (деактивировать предыдущую активную версию)
    /// </summary>
    public async Task ActivateStepVersionAsync(Guid stepVersionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Активация версии этапа {StepVersionId}", stepVersionId);

        try
        {
            await ActivateStepVersionInternalAsync(stepVersionId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Версия этапа {StepVersionId} успешно активирована", stepVersionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при активации версии этапа {StepVersionId}", stepVersionId);
            throw;
        }
    }

    /// <summary>
    /// Активировать версию компонента (деактивировать предыдущую активную версию)
    /// </summary>
    public async Task ActivateComponentVersionAsync(Guid componentVersionId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Активация версии компонента {ComponentVersionId}", componentVersionId);

        try
        {
            await _componentVersionRepository.ActivateVersionAsync(componentVersionId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Версия компонента {ComponentVersionId} успешно активирована", componentVersionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при активации версии компонента {ComponentVersionId}", componentVersionId);
            throw;
        }
    }

    /// <summary>
    /// Получить активную версию потока
    /// </summary>
    public async Task<FlowVersion?> GetActiveFlowVersionAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _flowVersionRepository.GetActiveVersionAsync(originalFlowId, cancellationToken);
    }

    /// <summary>
    /// Получить активную версию этапа
    /// </summary>
    public async Task<FlowStepVersion?> GetActiveStepVersionAsync(Guid originalStepId, CancellationToken cancellationToken = default)
    {
        return await GetActiveStepVersionInternalAsync(originalStepId, cancellationToken);
    }

    /// <summary>
    /// Получить активную версию компонента
    /// </summary>
    public async Task<ComponentVersion?> GetActiveComponentVersionAsync(Guid originalComponentId, CancellationToken cancellationToken = default)
    {
        return await _componentVersionRepository.GetActiveVersionAsync(originalComponentId, cancellationToken);
    }

    /// <summary>
    /// Получить конкретную версию потока
    /// </summary>
    public async Task<FlowVersion?> GetFlowVersionAsync(Guid originalFlowId, int version, CancellationToken cancellationToken = default)
    {
        return await _flowVersionRepository.GetVersionAsync(originalFlowId, version, cancellationToken);
    }

    /// <summary>
    /// Получить все версии потока
    /// </summary>
    public async Task<IList<FlowVersion>> GetAllFlowVersionsAsync(Guid originalFlowId, CancellationToken cancellationToken = default)
    {
        return await _flowVersionRepository.GetAllVersionsAsync(originalFlowId, cancellationToken);
    }

    /// <summary>
    /// Удалить неиспользуемые версии (не связанные с активными назначениями)
    /// </summary>
    public async Task<int> CleanupUnusedVersionsAsync(Guid originalFlowId, int keepMinimumVersions = 3, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Очистка неиспользуемых версий потока {OriginalFlowId}, сохранить минимум {KeepMinimum}", 
            originalFlowId, keepMinimumVersions);

        try
        {
            var unusedVersions = await _flowVersionRepository.GetUnusedVersionsAsync(originalFlowId, cancellationToken);
            var versionsToDelete = unusedVersions
                .OrderByDescending(v => v.Version)
                .Skip(keepMinimumVersions)
                .ToList();

            if (versionsToDelete.Count > 0)
            {
                await _flowVersionRepository.DeleteRangeAsync(versionsToDelete, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Удалено {DeletedCount} неиспользуемых версий потока {OriginalFlowId}", 
                    versionsToDelete.Count, originalFlowId);
            }

            return versionsToDelete.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при очистке неиспользуемых версий потока {OriginalFlowId}", originalFlowId);
            throw;
        }
    }

    #region Приватные методы

    /// <summary>
    /// Копировать специализированные данные компонента
    /// </summary>
    private async Task CopySpecializedComponentDataAsync(ComponentVersion source, ComponentVersion target, CancellationToken cancellationToken)
    {
        switch (source.ComponentType)
        {
            case ComponentType.Article:
                if (source.ArticleVersion != null)
                {
                    target.ArticleVersion = new ArticleComponentVersion(
                        target.Id,
                        source.ArticleVersion.Content,
                        source.ArticleVersion.ReadingTimeMinutes
                    );
                }
                break;

            case ComponentType.Quiz:
                if (source.QuizVersion != null)
                {
                    target.QuizVersion = new QuizComponentVersion(
                        target.Id,
                        source.QuizVersion.PassingScore,
                        source.QuizVersion.TimeLimitMinutes,
                        source.QuizVersion.AllowMultipleAttempts,
                        source.QuizVersion.ShowCorrectAnswers,
                        source.QuizVersion.ShuffleQuestions,
                        source.QuizVersion.ShuffleAnswers
                    );

                    // Копируем варианты ответов
                    foreach (var sourceOption in source.QuizVersion.Options.OrderBy(o => o.Order))
                    {
                        var targetOption = new QuizOptionVersion(
                            target.QuizVersion.ComponentVersionId,
                            sourceOption.Text,
                            sourceOption.IsCorrect,
                            sourceOption.Points,
                            sourceOption.Order,
                            sourceOption.Explanation
                        );
                        target.QuizVersion.AddOption(targetOption);
                    }
                }
                break;

            case ComponentType.Task:
                if (source.TaskVersion != null)
                {
                    target.TaskVersion = new TaskComponentVersion(
                        target.Id,
                        source.TaskVersion.Instructions,
                        source.TaskVersion.SubmissionType,
                        source.TaskVersion.MaxFileSize,
                        source.TaskVersion.AllowedFileTypes,
                        source.TaskVersion.RequiresMentorApproval,
                        source.TaskVersion.AutoApprovalKeywords
                    );
                }
                break;
        }
    }

    /// <summary>
    /// Получить максимальный номер версии для этапа
    /// </summary>
    private async Task<int> GetMaxStepVersionAsync(Guid originalStepId, CancellationToken cancellationToken)
    {
        // Здесь должен быть репозиторий для этапов, пока возвращаем 0
        // TODO: Реализовать IFlowStepVersionRepository
        return 0;
    }

    /// <summary>
    /// Получить активную версию этапа (внутренняя реализация)
    /// </summary>
    private async Task<FlowStepVersion?> GetActiveStepVersionInternalAsync(Guid originalStepId, CancellationToken cancellationToken)
    {
        // TODO: Реализовать через IFlowStepVersionRepository
        return null;
    }

    /// <summary>
    /// Активировать версию этапа (внутренняя реализация)
    /// </summary>
    private async Task ActivateStepVersionInternalAsync(Guid stepVersionId, CancellationToken cancellationToken)
    {
        // TODO: Реализовать через IFlowStepVersionRepository
        await Task.CompletedTask;
    }

    #endregion
}