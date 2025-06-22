using System.Security.Cryptography;
using System.Text;

namespace BuddyBot.Shared.Helpers;

/// <summary>
/// Вспомогательный класс для работы с паролями
/// </summary>
public static class PasswordHelper
{
    private const int DefaultSaltSize = 32;
    private const int DefaultKeySize = 32;
    private const int DefaultIterations = 100000;
    
    /// <summary>
    /// Создает хэш пароля с солью
    /// </summary>
    /// <param name="password">Пароль для хэширования</param>
    /// <param name="saltSize">Размер соли в байтах</param>
    /// <param name="keySize">Размер ключа в байтах</param>
    /// <param name="iterations">Количество итераций</param>
    /// <returns>Хэш пароля в формате base64</returns>
    public static string HashPassword(string password, int saltSize = DefaultSaltSize, int keySize = DefaultKeySize, int iterations = DefaultIterations)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Пароль не может быть пустым", nameof(password));
            
        // Генерируем соль
        var salt = new byte[saltSize];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        
        // Создаем хэш
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(keySize);
        
        // Объединяем соль и хэш
        var hashBytes = new byte[saltSize + keySize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, keySize);
        
        return Convert.ToBase64String(hashBytes);
    }
    
    /// <summary>
    /// Проверяет пароль против хэша
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <param name="hashedPassword">Хэш пароля</param>
    /// <param name="saltSize">Размер соли в байтах</param>
    /// <param name="keySize">Размер ключа в байтах</param>
    /// <param name="iterations">Количество итераций</param>
    /// <returns>true, если пароль совпадает</returns>
    public static bool VerifyPassword(string password, string hashedPassword, int saltSize = DefaultSaltSize, int keySize = DefaultKeySize, int iterations = DefaultIterations)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;
            
        try
        {
            var hashBytes = Convert.FromBase64String(hashedPassword);
            
            if (hashBytes.Length != saltSize + keySize)
                return false;
                
            // Извлекаем соль
            var salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);
            
            // Создаем хэш из проверяемого пароля
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(keySize);
            
            // Сравниваем хэши
            for (int i = 0; i < keySize; i++)
            {
                if (hashBytes[i + saltSize] != hash[i])
                    return false;
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Генерирует случайный пароль
    /// </summary>
    /// <param name="length">Длина пароля</param>
    /// <param name="includeUppercase">Включать заглавные буквы</param>
    /// <param name="includeLowercase">Включать строчные буквы</param>
    /// <param name="includeNumbers">Включать цифры</param>
    /// <param name="includeSpecialChars">Включать специальные символы</param>
    /// <returns>Сгенерированный пароль</returns>
    public static string GeneratePassword(int length = 12, bool includeUppercase = true, bool includeLowercase = true, bool includeNumbers = true, bool includeSpecialChars = true)
    {
        if (length < 1)
            throw new ArgumentException("Длина пароля должна быть больше 0", nameof(length));
            
        var characters = new StringBuilder();
        
        if (includeUppercase)
            characters.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        if (includeLowercase)
            characters.Append("abcdefghijklmnopqrstuvwxyz");
        if (includeNumbers)
            characters.Append("0123456789");
        if (includeSpecialChars)
            characters.Append("!@#$%^&*()_+-=[]{}|;:,.<>?");
            
        if (characters.Length == 0)
            throw new ArgumentException("Должен быть выбран хотя бы один тип символов");
            
        var charArray = characters.ToString().ToCharArray();
        var password = new char[length];
        
        using var rng = RandomNumberGenerator.Create();
        for (int i = 0; i < length; i++)
        {
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            var randomIndex = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % charArray.Length;
            password[i] = charArray[randomIndex];
        }
        
        return new string(password);
    }
    
    /// <summary>
    /// Проверяет силу пароля
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <returns>Оценка силы пароля от 0 до 100</returns>
    public static int CheckPasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return 0;
            
        int score = 0;
        
        // Длина
        score += Math.Min(password.Length * 2, 25);
        
        // Заглавные буквы
        if (password.Any(char.IsUpper))
            score += 10;
            
        // Строчные буквы
        if (password.Any(char.IsLower))
            score += 10;
            
        // Цифры
        if (password.Any(char.IsDigit))
            score += 10;
            
        // Специальные символы
        if (password.Any(c => !char.IsLetterOrDigit(c)))
            score += 15;
            
        // Разнообразие символов
        var uniqueChars = password.Distinct().Count();
        score += Math.Min(uniqueChars * 2, 20);
        
        // Штрафы
        // Только цифры
        if (password.All(char.IsDigit))
            score -= 20;
            
        // Только буквы
        if (password.All(char.IsLetter))
            score -= 10;
            
        // Повторяющиеся символы
        if (HasRepeatingCharacters(password))
            score -= 10;
            
        // Последовательности
        if (HasSequentialCharacters(password))
            score -= 10;
            
        return Math.Max(0, Math.Min(100, score));
    }
    
    /// <summary>
    /// Получает текстовое описание силы пароля
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <returns>Описание силы пароля</returns>
    public static string GetPasswordStrengthDescription(string password)
    {
        var strength = CheckPasswordStrength(password);
        
        return strength switch
        {
            >= 80 => "Очень сильный",
            >= 60 => "Сильный",
            >= 40 => "Средний",
            >= 20 => "Слабый",
            _ => "Очень слабый"
        };
    }
    
    /// <summary>
    /// Проверяет, соответствует ли пароль минимальным требованиям
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <param name="minLength">Минимальная длина</param>
    /// <param name="requireUppercase">Требовать заглавные буквы</param>
    /// <param name="requireLowercase">Требовать строчные буквы</param>
    /// <param name="requireNumbers">Требовать цифры</param>
    /// <param name="requireSpecialChars">Требовать специальные символы</param>
    /// <returns>true, если пароль соответствует требованиям</returns>
    public static bool IsPasswordValid(string password, int minLength = 8, bool requireUppercase = true, bool requireLowercase = true, bool requireNumbers = true, bool requireSpecialChars = false)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
            
        if (password.Length < minLength)
            return false;
            
        if (requireUppercase && !password.Any(char.IsUpper))
            return false;
            
        if (requireLowercase && !password.Any(char.IsLower))
            return false;
            
        if (requireNumbers && !password.Any(char.IsDigit))
            return false;
            
        if (requireSpecialChars && !password.Any(c => !char.IsLetterOrDigit(c)))
            return false;
            
        return true;
    }
    
    /// <summary>
    /// Получает список требований к паролю, которые не выполняются
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <param name="minLength">Минимальная длина</param>
    /// <param name="requireUppercase">Требовать заглавные буквы</param>
    /// <param name="requireLowercase">Требовать строчные буквы</param>
    /// <param name="requireNumbers">Требовать цифры</param>
    /// <param name="requireSpecialChars">Требовать специальные символы</param>
    /// <returns>Список невыполненных требований</returns>
    public static List<string> GetPasswordValidationErrors(string password, int minLength = 8, bool requireUppercase = true, bool requireLowercase = true, bool requireNumbers = true, bool requireSpecialChars = false)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Пароль не может быть пустым");
            return errors;
        }
        
        if (password.Length < minLength)
            errors.Add($"Пароль должен содержать минимум {minLength} символов");
            
        if (requireUppercase && !password.Any(char.IsUpper))
            errors.Add("Пароль должен содержать заглавные буквы");
            
        if (requireLowercase && !password.Any(char.IsLower))
            errors.Add("Пароль должен содержать строчные буквы");
            
        if (requireNumbers && !password.Any(char.IsDigit))
            errors.Add("Пароль должен содержать цифры");
            
        if (requireSpecialChars && !password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Пароль должен содержать специальные символы");
            
        return errors;
    }
    
    /// <summary>
    /// Проверяет наличие повторяющихся символов
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <returns>true, если есть повторяющиеся символы</returns>
    private static bool HasRepeatingCharacters(string password)
    {
        for (int i = 0; i < password.Length - 1; i++)
        {
            int count = 1;
            for (int j = i + 1; j < password.Length && password[j] == password[i]; j++)
            {
                count++;
            }
            if (count >= 3)
                return true;
        }
        return false;
    }
    
    /// <summary>
    /// Проверяет наличие последовательных символов
    /// </summary>
    /// <param name="password">Проверяемый пароль</param>
    /// <returns>true, если есть последовательные символы</returns>
    private static bool HasSequentialCharacters(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            var char1 = password[i];
            var char2 = password[i + 1];
            var char3 = password[i + 2];
            
            if ((char2 == char1 + 1 && char3 == char2 + 1) ||
                (char2 == char1 - 1 && char3 == char2 - 1))
                return true;
        }
        return false;
    }
}