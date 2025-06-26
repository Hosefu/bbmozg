using Lauf.Domain.Entities.Components;
using Lauf.Domain.Entities.Flows;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория для работы с компонентами
/// </summary>
public interface IComponentRepository
{
    /// <summary>
    /// Получает компонент статьи по ID
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Компонент статьи или null</returns>
    Task<ArticleComponent?> GetArticleComponentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает компонент квиза по ID
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Компонент квиза или null</returns>
    Task<QuizComponent?> GetQuizComponentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает компонент задания по ID
    /// </summary>
    /// <param name="id">Идентификатор компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Компонент задания или null</returns>
    Task<TaskComponent?> GetTaskComponentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет компонент статьи
    /// </summary>
    /// <param name="component">Компонент статьи</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный компонент</returns>
    Task<ArticleComponent> AddArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет компонент квиза
    /// </summary>
    /// <param name="component">Компонент квиза</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный компонент</returns>
    Task<QuizComponent> AddQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет компонент задания
    /// </summary>
    /// <param name="component">Компонент задания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный компонент</returns>
    Task<TaskComponent> AddTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет связь между шагом и компонентом
    /// </summary>
    /// <param name="stepComponent">Связь между шагом и компонентом</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленная связь</returns>
    Task<FlowStepComponent> AddStepComponentAsync(FlowStepComponent stepComponent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет компонент статьи
    /// </summary>
    /// <param name="component">Компонент статьи</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный компонент</returns>
    Task<ArticleComponent> UpdateArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет компонент квиза
    /// </summary>
    /// <param name="component">Компонент квиза</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный компонент</returns>
    Task<QuizComponent> UpdateQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет компонент задания
    /// </summary>
    /// <param name="component">Компонент задания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный компонент</returns>
    Task<TaskComponent> UpdateTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет компонент статьи
    /// </summary>
    /// <param name="component">Компонент статьи</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteArticleComponentAsync(ArticleComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет компонент квиза
    /// </summary>
    /// <param name="component">Компонент квиза</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteQuizComponentAsync(QuizComponent component, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет компонент задания
    /// </summary>
    /// <param name="component">Компонент задания</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteTaskComponentAsync(TaskComponent component, CancellationToken cancellationToken = default);
} 