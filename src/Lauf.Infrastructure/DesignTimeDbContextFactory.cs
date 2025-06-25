using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Lauf.Infrastructure.Persistence;
using Lauf.Infrastructure.Persistence.Interceptors;

namespace Lauf.Infrastructure;

/// <summary>
/// Factory для создания DbContext во время разработки (design-time)
/// Используется EF Core CLI tools для миграций
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Загружаем конфигурацию из appsettings.Development.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Lauf.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionsBuilder.UseNpgsql(connectionString);

        // Используем конструктор без перехватчиков для миграций
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}