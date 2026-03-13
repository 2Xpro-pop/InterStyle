using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InterStyle.ApiShared;

public static class MediatRExtensions
{
    public static IServiceCollection AddInterStyleMediatR<TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<TAssemblyMarker>();

            var licenseKey = configuration["MediatR:LicenseKey"];
            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                cfg.LicenseKey = licenseKey;
            }
        });

        return services;
    }
}
