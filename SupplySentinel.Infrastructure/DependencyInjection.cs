using Microsoft.Extensions.DependencyInjection;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Infrastructure.Mocks;

namespace SupplySentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDocumentReaderTool, MockDocumentReaderTool>();
        services.AddScoped<IERPComparisonTool, MockERPComparisonTool>();
        services.AddScoped<IBCDataSyncTool, MockBCDataSyncTool>();
        
        return services;
    }
}