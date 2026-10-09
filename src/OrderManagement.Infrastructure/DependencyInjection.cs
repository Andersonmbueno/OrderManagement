using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Common;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Repositories;

namespace OrderManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=orders.db";

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // We use a feature-specific repository instead of IRepository<T>.
        // This keeps the abstraction focused on the use cases we actually need.
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
