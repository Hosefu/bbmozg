using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Lauf.Api.Services;

/// <summary>
/// Валидатор авторизации Telegram Web App
/// </summary>
public class TelegramAuthValidator
{
    private readonly string _botToken;

    public TelegramAuthValidator(string botToken)
    {
        _botToken = botToken ?? throw new ArgumentNullException(nameof(botToken));
    }

    /// <summary>
    /// Валидирует данные от Telegram Web App
    /// </summary>
    /// <param name="initData">Строка initData от Telegram Web App</param>
    /// <returns>true если данные валидны</returns>
    public bool ValidateInitData(string initData)
    {
        if (string.IsNullOrEmpty(initData))
            return false;

        try
        {
            var urlParams = HttpUtility.ParseQueryString(initData);
            var hash = urlParams["hash"];
            
            if (string.IsNullOrEmpty(hash))
                return false;

            // Удаляем hash из параметров для проверки подписи
            urlParams.Remove("hash");
            
            // Сортируем параметры и создаем строку для проверки
            var sortedParams = urlParams.AllKeys
                .Where(key => !string.IsNullOrEmpty(key))
                .OrderBy(key => key)
                .Select(key => $"{key}={urlParams[key]}")
                .ToArray();

            var dataCheckString = string.Join("\n", sortedParams);
            
            // Вычисляем ожидаемый hash
            var expectedHash = ComputeHmacSha256(dataCheckString, _botToken);
            
            return hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Извлекает пользовательские данные из initData
    /// </summary>
    /// <param name="initData">Строка initData от Telegram Web App</param>
    /// <returns>Данные пользователя или null</returns>
    public TelegramUserData? ExtractUserData(string initData)
    {
        if (string.IsNullOrEmpty(initData))
            return null;

        try
        {
            var urlParams = HttpUtility.ParseQueryString(initData);
            var userJson = urlParams["user"];
            
            if (string.IsNullOrEmpty(userJson))
                return null;

            var userObject = JsonSerializer.Deserialize<JsonElement>(userJson);
            
            return new TelegramUserData
            {
                Id = userObject.GetProperty("id").GetInt64(),
                FirstName = userObject.TryGetProperty("first_name", out var firstName) ? firstName.GetString() : null,
                LastName = userObject.TryGetProperty("last_name", out var lastName) ? lastName.GetString() : null,
                Username = userObject.TryGetProperty("username", out var username) ? username.GetString() : null,
                PhotoUrl = userObject.TryGetProperty("photo_url", out var photoUrl) ? photoUrl.GetString() : null,
                LanguageCode = userObject.TryGetProperty("language_code", out var languageCode) ? languageCode.GetString() : "ru"
            };
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Проверяет, что данные не слишком старые
    /// </summary>
    /// <param name="initData">Строка initData от Telegram Web App</param>
    /// <param name="maxAgeSeconds">Максимальный возраст в секундах (по умолчанию 86400 - 24 часа)</param>
    /// <returns>true если данные актуальны</returns>
    public bool ValidateAuthDate(string initData, int maxAgeSeconds = 86400)
    {
        if (string.IsNullOrEmpty(initData))
            return false;

        try
        {
            var urlParams = HttpUtility.ParseQueryString(initData);
            var authDateStr = urlParams["auth_date"];
            
            if (string.IsNullOrEmpty(authDateStr) || !long.TryParse(authDateStr, out var authDate))
                return false;

            var authDateTime = DateTimeOffset.FromUnixTimeSeconds(authDate);
            var now = DateTimeOffset.UtcNow;
            
            return (now - authDateTime).TotalSeconds <= maxAgeSeconds;
        }
        catch
        {
            return false;
        }
    }

    private string ComputeHmacSha256(string data, string key)
    {
        // Согласно документации Telegram, сначала нужно получить secret_key
        var secretKey = ComputeSha256($"WebAppData{key}");
        
        using var hmac = new HMACSHA256(secretKey);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private byte[] ComputeSha256(string data)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
    }
}

/// <summary>
/// Данные пользователя из Telegram
/// </summary>
public class TelegramUserData
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? PhotoUrl { get; set; }
    public string? LanguageCode { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}