namespace Lauf.Application.Services.Interfaces;

/// <summary>
/// Сервис для работы с файловым хранилищем
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Загрузить файл
    /// </summary>
    Task<FileUploadResult> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string? folder = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Загрузить файл из байтового массива
    /// </summary>
    Task<FileUploadResult> UploadFileAsync(
        byte[] fileBytes,
        string fileName,
        string contentType,
        string? folder = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файл по ключу
    /// </summary>
    Task<FileDownloadResult?> GetFileAsync(string fileKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить URL для скачивания файла
    /// </summary>
    Task<string?> GetFileUrlAsync(string fileKey, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить URL для скачивания файла с временным доступом
    /// </summary>
    Task<string?> GetTemporaryFileUrlAsync(string fileKey, TimeSpan expiration, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить файл
    /// </summary>
    Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существование файла
    /// </summary>
    Task<bool> FileExistsAsync(string fileKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить метаданные файла
    /// </summary>
    Task<FileMetadata?> GetFileMetadataAsync(string fileKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Копировать файл
    /// </summary>
    Task<FileUploadResult?> CopyFileAsync(
        string sourceFileKey,
        string destinationFolder,
        string? newFileName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Переместить файл
    /// </summary>
    Task<FileUploadResult?> MoveFileAsync(
        string sourceFileKey,
        string destinationFolder,
        string? newFileName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить список файлов в папке
    /// </summary>
    Task<IEnumerable<FileInfo>> ListFilesAsync(
        string? folder = null,
        string? filePattern = null,
        int maxResults = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Очистка временных файлов
    /// </summary>
    Task CleanupExpiredFilesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Результат загрузки файла
/// </summary>
public class FileUploadResult
{
    /// <summary>
    /// Ключ файла в хранилище
    /// </summary>
    public string FileKey { get; set; } = string.Empty;

    /// <summary>
    /// Оригинальное имя файла
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Тип содержимого
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// URL для доступа к файлу
    /// </summary>
    public string? FileUrl { get; set; }

    /// <summary>
    /// Папка, в которую загружен файл
    /// </summary>
    public string? Folder { get; set; }

    /// <summary>
    /// Дата загрузки
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// Результат скачивания файла
/// </summary>
public class FileDownloadResult
{
    /// <summary>
    /// Поток данных файла
    /// </summary>
    public Stream FileStream { get; set; } = Stream.Null;

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Тип содержимого
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime? LastModified { get; set; }
}

/// <summary>
/// Метаданные файла
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// Ключ файла
    /// </summary>
    public string FileKey { get; set; } = string.Empty;

    /// <summary>
    /// Оригинальное имя файла
    /// </summary>
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла в байтах
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Тип содержимого
    /// </summary>
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего изменения
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// MD5 хэш файла
    /// </summary>
    public string? MD5Hash { get; set; }

    /// <summary>
    /// Дополнительные метаданные
    /// </summary>
    public Dictionary<string, string> Tags { get; set; } = new();
}

/// <summary>
/// Информация о файле в списке
/// </summary>
public class FileInfo
{
    /// <summary>
    /// Ключ файла
    /// </summary>
    public string FileKey { get; set; } = string.Empty;

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Размер файла
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Папка
    /// </summary>
    public string? Folder { get; set; }
}