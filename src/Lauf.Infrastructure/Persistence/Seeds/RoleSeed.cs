using Lauf.Domain.Entities.Users;
using Lauf.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace Lauf.Infrastructure.Persistence.Seeds;

public static class RoleSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Проверяем, есть ли уже роли в базе
        if (await context.Roles.AnyAsync())
        {
            return; // Роли уже существуют
        }

        // Создаем базовые роли
        var roles = new List<Role>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Admin,
                Description = "Администратор системы",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Buddy,
                Description = "Наставник",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = Roles.Employee,
                Description = "Обычный сотрудник",
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
    }
} 