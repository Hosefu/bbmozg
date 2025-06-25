using Hangfire.Dashboard;

namespace Lauf.Api.Services;

/// <summary>
/// Фильтр авторизации для Hangfire Dashboard в development окружении
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    /// Проверка авторизации для доступа к dashboard
    /// В development разрешаем всем, в production потребуется реальная авторизация
    /// </summary>
    public bool Authorize(DashboardContext context)
    {
        // В development окружении разрешаем доступ всем
        // В production здесь должна быть реальная проверка прав доступа
        return true;
    }
} 