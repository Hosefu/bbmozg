using Lauf.Domain.Entities.Flows;
using Lauf.Domain.Enums;

namespace Lauf.Domain.Interfaces.Repositories;

/// <summary>
/// Интерфейс репозитория потоков
/// </summary>
public interface IFlowRepository
{
    /// <summary>
    /// Получает поток по ID
    /// </summary>
    /// <param name="id">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток или null</returns>
    Task<Flow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает поток со всеми связанными данными
    /// </summary>
    /// <param name="id">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток с данными или null</returns>
    Task<Flow?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает поток с шагами
    /// </summary>
    /// <param name="id">Идентификатор потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток с шагами или null</returns>
    Task<Flow?> GetByIdWithStepsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает поток по названию
    /// </summary>
    /// <param name="title">Название потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток или null</returns>
    Task<Flow?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает потоки по статусу
    /// </summary>
    /// <param name="status">Статус потока</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список потоков</returns>
    Task<IReadOnlyList<Flow>> GetByStatusAsync(FlowStatus status, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает опубликованные потоки
    /// </summary>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список опубликованных потоков</returns>
    Task<IReadOnlyList<Flow>> GetPublishedAsync(int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает потоки по категории
    /// </summary>
    /// <param name="category">Категория</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список потоков</returns>
    Task<IReadOnlyList<Flow>> GetByCategoryAsync(string category, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает обязательные потоки
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список обязательных потоков</returns>
    Task<IReadOnlyList<Flow>> GetRequiredFlowsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Поиск потоков по тексту
    /// </summary>
    /// <param name="searchText">Текст для поиска</param>
    /// <param name="skip">Количество пропускаемых записей</param>
    /// <param name="take">Количество возвращаемых записей</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список найденных потоков</returns>
    Task<IReadOnlyList<Flow>> SearchAsync(string searchText, int skip = 0, int take = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает количество потоков по статусу
    /// </summary>
    /// <param name="status">Статус потока</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Количество потоков</returns>
    Task<int> CountByStatusAsync(FlowStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет существование потока по названию
    /// </summary>
    /// <param name="title">Название потока</param>
    /// <param name="excludeId">Исключаемый ID (для обновления)</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>true, если поток существует</returns>
    Task<bool> ExistsByTitleAsync(string title, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новый поток
    /// </summary>
    /// <param name="flow">Поток</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный поток</returns>
    Task<Flow> AddAsync(Flow flow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет поток
    /// </summary>
    /// <param name="flow">Поток</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Обновленный поток</returns>
    Task<Flow> UpdateAsync(Flow flow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет поток
    /// </summary>
    /// <param name="flow">Поток</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteAsync(Flow flow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все потоки
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех потоков</returns>
    Task<IReadOnlyList<Flow>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает все потоки с шагами и компонентами
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список всех потоков с шагами</returns>
    Task<IReadOnlyList<Flow>> GetAllWithStepsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает поток по идентификатору шага
    /// </summary>
    /// <param name="stepId">Идентификатор шага</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Поток содержащий указанный шаг</returns>
    Task<Flow?> GetFlowByStepIdAsync(Guid stepId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет шаг к потоку
    /// </summary>
    /// <param name="flowId">Идентификатор потока</param>
    /// <param name="step">Шаг для добавления</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Добавленный шаг</returns>
    Task<FlowStep> AddStepAsync(Guid flowId, FlowStep step, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает шаг потока по ID
    /// </summary>
    /// <param name="stepId">Идентификатор шага</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Шаг потока или null</returns>
    Task<FlowStep?> GetStepByIdAsync(Guid stepId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получает шаг потока по идентификатору компонента
    /// </summary>
    /// <param name="componentId">Идентификатор компонента</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Шаг потока содержащий указанный компонент</returns>
    Task<FlowStep?> GetStepByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default);
}