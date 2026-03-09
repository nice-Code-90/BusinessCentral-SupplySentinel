using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SupplySentinel.Application.Common.Interfaces;

namespace SupplySentinel.Infrastructure.REST;

public static class DependencyInjection
{
    public static IServiceCollection AddRestInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.Configure<BusinessCentralOptions>(
            configuration.GetSection(BusinessCentralOptions.SectionName));

        services.AddTransient<AuthenticationHandler>();

        
        services.AddHttpClient<IBCDataSyncTool, BCDataSyncTool>((serviceProvider, client) =>
        {
        
            var options = serviceProvider.GetRequiredService<IOptions<BusinessCentralOptions>>().Value;

            if (string.IsNullOrEmpty(options.ApiBaseUrl))
                throw new InvalidOperationException("Business Central ApiBaseUrl is not configured.");

            client.BaseAddress = new Uri(options.ApiBaseUrl);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        
        services.AddHttpClient<IERPComparisonTool, ERPComparisonTool>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BusinessCentralOptions>>().Value;
            client.BaseAddress = new Uri(options.ApiBaseUrl);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }
}