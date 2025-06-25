using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Lauf.Infrastructure.ExternalServices.FileStorage;
using System.Text;
using Xunit;

namespace Lauf.Infrastructure.Tests.ExternalServices;

/// <summary>
/// Тесты для LocalFileStorageService
/// </summary>
public class LocalFileStorageServiceTests : IDisposable
{
    private readonly string _testBasePath;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<LocalFileStorageService>> _loggerMock;
    private readonly LocalFileStorageService _service;

    public LocalFileStorageServiceTests()
    {
        _testBasePath = Path.Combine(Path.GetTempPath(), "lauf_tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testBasePath);

        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["FileStorage:BasePath"]).Returns(_testBasePath);

        _loggerMock = new Mock<ILogger<LocalFileStorageService>>();
        
        _service = new LocalFileStorageService(_configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task UploadFileAsync_ValidFile_ShouldUploadSuccessfully()
    {
        // Arrange
        var fileContent = "Тестовое содержимое файла";
        var fileName = "test.txt";
        var contentType = "text/plain";
        
        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Act
        var result = await _service.UploadFileAsync(fileStream, fileName, contentType);

        // Assert
        result.Should().NotBeNullOrEmpty();
        
        // Проверяем, что файл действительно создан
        var files = Directory.GetFiles(_testBasePath, "*.*", SearchOption.AllDirectories);
        files.Should().HaveCount(1);
        
        var uploadedFilePath = files.First();
        var uploadedContent = await File.ReadAllTextAsync(uploadedFilePath);
        uploadedContent.Should().Be(fileContent);
    }

    [Fact]
    public async Task UploadFileAsync_EmptyStream_ShouldThrowArgumentException()
    {
        // Arrange
        using var emptyStream = new MemoryStream();
        var fileName = "empty.txt";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UploadFileAsync(emptyStream, fileName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task UploadFileAsync_InvalidFileName_ShouldThrowArgumentException(string invalidFileName)
    {
        // Arrange
        var fileContent = "Содержимое";
        using var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UploadFileAsync(fileStream, invalidFileName));
    }

    [Fact]
    public async Task DownloadFileAsync_ExistingFile_ShouldReturnFileStream()
    {
        // Arrange
        var fileContent = "Тестовое содержимое для скачивания";
        var fileName = "download_test.txt";
        
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var fileId = await _service.UploadFileAsync(uploadStream, fileName);

        // Act
        var downloadStream = await _service.DownloadFileAsync(fileId);

        // Assert
        downloadStream.Should().NotBeNull();
        
        using var reader = new StreamReader(downloadStream!);
        var downloadedContent = await reader.ReadToEndAsync();
        downloadedContent.Should().Be(fileContent);
    }

    [Fact]
    public async Task DownloadFileAsync_NonExistingFile_ShouldReturnNull()
    {
        // Arrange
        var nonExistingFileId = "non_existing_file.txt";

        // Act
        var result = await _service.DownloadFileAsync(nonExistingFileId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteFileAsync_ExistingFile_ShouldDeleteSuccessfully()
    {
        // Arrange
        var fileContent = "Файл для удаления";
        var fileName = "delete_test.txt";
        
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var fileId = await _service.UploadFileAsync(uploadStream, fileName);

        // Убеждаемся, что файл создан
        var filesBefore = Directory.GetFiles(_testBasePath, "*.*", SearchOption.AllDirectories);
        filesBefore.Should().HaveCount(1);

        // Act
        var result = await _service.DeleteFileAsync(fileId);

        // Assert
        result.Should().BeTrue();
        
        var filesAfter = Directory.GetFiles(_testBasePath, "*.*", SearchOption.AllDirectories);
        filesAfter.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteFileAsync_NonExistingFile_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingFileId = "non_existing_file.txt";

        // Act
        var result = await _service.DeleteFileAsync(nonExistingFileId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetFileMetadataAsync_ExistingFile_ShouldReturnMetadata()
    {
        // Arrange
        var fileContent = "Содержимое для метаданных";
        var fileName = "metadata_test.txt";
        var contentType = "text/plain";
        
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var fileId = await _service.UploadFileAsync(uploadStream, fileName, contentType);

        // Act
        var metadata = await _service.GetFileMetadataAsync(fileId);

        // Assert
        metadata.Should().NotBeNull();
        metadata!.FileName.Should().Contain("metadata_test.txt");
        metadata.ContentType.Should().Be(contentType);
        metadata.Size.Should().Be(fileContent.Length);
        metadata.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetFileMetadataAsync_NonExistingFile_ShouldReturnNull()
    {
        // Arrange
        var nonExistingFileId = "non_existing_file.txt";

        // Act
        var metadata = await _service.GetFileMetadataAsync(nonExistingFileId);

        // Assert
        metadata.Should().BeNull();
    }

    [Fact]
    public async Task FileExistsAsync_ExistingFile_ShouldReturnTrue()
    {
        // Arrange
        var fileContent = "Проверка существования";
        var fileName = "exists_test.txt";
        
        using var uploadStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var fileId = await _service.UploadFileAsync(uploadStream, fileName);

        // Act
        var exists = await _service.FileExistsAsync(fileId);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task FileExistsAsync_NonExistingFile_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingFileId = "non_existing_file.txt";

        // Act
        var exists = await _service.FileExistsAsync(nonExistingFileId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task CleanupOldFilesAsync_WithOldFiles_ShouldDeleteOldFiles()
    {
        // Arrange
        var oldFileContent = "Старый файл";
        var newFileContent = "Новый файл";
        
        // Создаем старый файл
        using var oldStream = new MemoryStream(Encoding.UTF8.GetBytes(oldFileContent));
        var oldFileId = await _service.UploadFileAsync(oldStream, "old_file.txt");

        // Создаем новый файл
        using var newStream = new MemoryStream(Encoding.UTF8.GetBytes(newFileContent));
        var newFileId = await _service.UploadFileAsync(newStream, "new_file.txt");

        // Модифицируем время создания старого файла (симулируем, что он создан давно)
        var oldFilePath = Directory.GetFiles(_testBasePath, "*.*", SearchOption.AllDirectories)
            .First(f => Path.GetFileNameWithoutExtension(f).StartsWith(Path.GetFileNameWithoutExtension(oldFileId)));
        File.SetCreationTime(oldFilePath, DateTime.UtcNow.AddDays(-8)); // Делаем файл старше 7 дней

        // Act
        var deletedCount = await _service.CleanupOldFilesAsync(TimeSpan.FromDays(7));

        // Assert
        deletedCount.Should().Be(1);
        
        var remainingFiles = Directory.GetFiles(_testBasePath, "*.*", SearchOption.AllDirectories);
        remainingFiles.Should().HaveCount(1);
        
        // Новый файл должен остаться
        var newFileExists = await _service.FileExistsAsync(newFileId);
        newFileExists.Should().BeTrue();
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testBasePath))
            {
                Directory.Delete(_testBasePath, true);
            }
        }
        catch
        {
            // Игнорируем ошибки при удалении тестовой директории
        }
    }
}