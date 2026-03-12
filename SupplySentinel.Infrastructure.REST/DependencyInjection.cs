using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Infrastructure.REST;

public static class DependencyInjection
{
    public static IServiceCollection AddRestInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BusinessCentralOptions>(
            configuration.GetSection(BusinessCentralOptions.SectionName));

        services.AddTransient<AuthenticationHandler>();

        
        services.AddHttpClient<IERPComparisonTool, ERPComparisonTool>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BusinessCentralOptions>>().Value;
            client.BaseAddress = new Uri(options.FullApiUrl);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        
        services.AddHttpClient<IBCDataSyncTool, BCDataSyncTool>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<BusinessCentralOptions>>().Value;
            client.BaseAddress = new Uri(options.FullApiUrl);
        })
        .AddHttpMessageHandler<AuthenticationHandler>();

        return services;
    }
}