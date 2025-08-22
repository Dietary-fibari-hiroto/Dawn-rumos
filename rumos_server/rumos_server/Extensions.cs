using rumos_server.Features.Repositories;
using rumos_server.Features.Services;
using rumos_server.Features.Interface;

namespace rumos_server.Extensions;

public static class ServiceExtensions
{
    public static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IDeviceRepository,DeviceRepository>();
    }

    public static void RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IPlatformService,PlatformService>();
        services.AddScoped<IDeviceService, DeviceService>();
    }
}