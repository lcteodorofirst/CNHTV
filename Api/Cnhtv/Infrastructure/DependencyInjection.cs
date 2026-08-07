using Cnhtv.Application.Common;
using Cnhtv.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cnhtv.Application.Interfaces.Devices;
using Cnhtv.Infrastructure.Services;

namespace Cnhtv.Infrastructure.DependencyInjection;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        AppConfiguration configuration
    )
    {
        AddDbContexts(services, configuration);
        AddRepositories(services);

        return services;
    }

    private static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        AppConfiguration configuration
    )
    {
        services.AddDbContext<CnhtvDbContext>(options =>
            options.UseSqlServer(configuration.AppDbConnection)
        );

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITvDeviceService, TvDeviceService>();

        return services;
    }
}
