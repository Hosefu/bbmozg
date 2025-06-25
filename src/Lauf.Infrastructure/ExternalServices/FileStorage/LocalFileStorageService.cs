using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lauf.Application.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Lauf.Infrastructure.ExternalServices.FileStorage;

/// <summary>
/// Локальная реализация сервиса файлового хранилища
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
    {
        _logger = logger;
        _basePath = configuration.GetValue<string>("FileStorage:LocalPath") ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        
        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(Stream fileStream, string fileName, string contentType, string? folder = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileId = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(fileName);
            var storedFileName = $"{fileId}{extension}";
            
            var targetFolder = string.IsNullOrEmpty(folder) ? _basePath : Path.Combine(_basePath, folder);
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var filePath = Path.Combine(targetFolder, storedFileName);

            using var fileStreamDestination = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamDestination, cancellationToken);
            
            var fileInfo = new System.IO.FileInfo(filePath);
            
            _logger.LogDebug("Файл загружен: {FileName} -> {FilePath}", fileName, filePath);

            return new FileUploadResult
            {
                FileKey = fileId,
                OriginalFileName = fileName,
                FileSize = fileInfo.Length,
                ContentType = contentType,
                FileUrl = $"/files/{fileId}",
                Folder = folder,
                UploadedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла {FileName}", fileName);
            throw;
        }
    }

    public async Task<FileUploadResult> UploadFileAsync(byte[] fileBytes, string fileName, string contentType, string? folder = null, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(fileBytes);
        return await UploadFileAsync(stream, fileName, contentType, folder, cancellationToken);
    }

    public async Task<FileDownloadResult?> GetFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = await FindFilePathAsync(fileKey);
            if (filePath == null)
            {
                return null;
            }

            var fileInfo = new System.IO.FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            return new FileDownloadResult
            {
                FileStream = fileStream,
                FileName = Path.GetFileName(filePath),
                ContentType = GetContentType(filePath),
                FileSize = fileInfo.Length,
                LastModified = fileInfo.LastWriteTime
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении файла {FileKey}", fileKey);
            return null;
        }
    }

    public Task<string?> GetFileUrlAsync(string fileKey, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        // Для локального хранилища возвращаем простой URL
        return Task.FromResult<string?>($"/files/{fileKey}");
    }

    public Task<string?> GetTemporaryFileUrlAsync(string fileKey, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        // Для локального хранилища возвращаем простой URL (временность не реализована)
        return Task.FromResult<string?>($"/files/{fileKey}?temp=true");
    }

    public async Task<bool> DeleteFileAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = await FindFilePathAsync(fileKey);
            if (filePath == null)
            {
                return false;
            }

            File.Delete(filePath);
            _logger.LogDebug("Файл удален: {FilePath}", filePath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении файла {FileKey}", fileKey);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        var filePath = await FindFilePathAsync(fileKey);
        return filePath != null && File.Exists(filePath);
    }

    public async Task<FileMetadata?> GetFileMetadataAsync(string fileKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var filePath = await FindFilePathAsync(fileKey);
            if (filePath == null)
            {
                return null;
            }

            var fileInfo = new System.IO.FileInfo(filePath);
            var md5Hash = await CalculateMD5HashAsync(filePath);

            return new FileMetadata
            {
                FileKey = fileKey,
                OriginalFileName = Path.GetFileName(filePath),
                FileSize = fileInfo.Length,
                ContentType = GetContentType(filePath),
                CreatedAt = fileInfo.CreationTime,
                LastModified = fileInfo.LastWriteTime,
                MD5Hash = md5Hash
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении метаданных файла {FileKey}", fileKey);
            return null;
        }
    }

    public async Task<FileUploadResult?> CopyFileAsync(string sourceFileKey, string destinationFolder, string? newFileName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var sourcePath = await FindFilePathAsync(sourceFileKey);
            if (sourcePath == null)
            {
                return null;
            }

            var fileName = newFileName ?? Path.GetFileName(sourcePath);
            var fileBytes = await File.ReadAllBytesAsync(sourcePath, cancellationToken);
            var contentType = GetContentType(sourcePath);

            return await UploadFileAsync(fileBytes, fileName, contentType, destinationFolder, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при копировании файла {FileKey}", sourceFileKey);
            return null;
        }
    }

    public async Task<FileUploadResult?> MoveFileAsync(string sourceFileKey, string destinationFolder, string? newFileName = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var copyResult = await CopyFileAsync(sourceFileKey, destinationFolder, newFileName, cancellationToken);
            if (copyResult != null)
            {
                await DeleteFileAsync(sourceFileKey, cancellationToken);
            }
            return copyResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при перемещении файла {FileKey}", sourceFileKey);
            return null;
        }
    }

    public Task<IEnumerable<Lauf.Application.Services.Interfaces.FileInfo>> ListFilesAsync(string? folder = null, string? filePattern = null, int maxResults = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchPath = string.IsNullOrEmpty(folder) ? _basePath : Path.Combine(_basePath, folder);
            if (!Directory.Exists(searchPath))
            {
                return Task.FromResult(Enumerable.Empty<Lauf.Application.Services.Interfaces.FileInfo>());
            }

            var pattern = filePattern ?? "*.*";
            var files = Directory.GetFiles(searchPath, pattern, SearchOption.TopDirectoryOnly)
                .Take(maxResults)
                .Select(filePath =>
                {
                    var fileInfo = new System.IO.FileInfo(filePath);
                    var fileName = Path.GetFileName(filePath);
                    var fileKey = Path.GetFileNameWithoutExtension(fileName);

                    return new Lauf.Application.Services.Interfaces.FileInfo
                    {
                        FileKey = fileKey,
                        FileName = fileName,
                        FileSize = fileInfo.Length,
                        CreatedAt = fileInfo.CreationTime,
                        Folder = folder
                    };
                });

            return Task.FromResult(files.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка файлов в папке {Folder}", folder);
            return Task.FromResult(Enumerable.Empty<Lauf.Application.Services.Interfaces.FileInfo>());
        }
    }

    public Task CleanupExpiredFilesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Удаляем файлы старше 30 дней
            var cutoffDate = DateTime.UtcNow.AddDays(-30);
            var files = Directory.GetFiles(_basePath, "*", SearchOption.AllDirectories)
                .Where(f => new System.IO.FileInfo(f).CreationTime < cutoffDate);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                    _logger.LogDebug("Удален устаревший файл: {FilePath}", file);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Не удалось удалить устаревший файл: {FilePath}", file);
                }
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при очистке устаревших файлов");
            return Task.CompletedTask;
        }
    }

    private async Task<string?> FindFilePathAsync(string fileKey)
    {
        // Поиск файла по всем папкам
        var files = Directory.GetFiles(_basePath, $"{fileKey}.*", SearchOption.AllDirectories);
        return files.FirstOrDefault();
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".zip" => "application/zip",
            _ => "application/octet-stream"
        };
    }

    private async Task<string> CalculateMD5HashAsync(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}