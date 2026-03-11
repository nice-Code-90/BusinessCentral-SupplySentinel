using Microsoft.Extensions.DependencyInjection;
using SupplySentinel.Application.Common.Interfaces;

using SupplySentinel.Infrastructure.Mocks;
using SupplySentinel.Infrastructure.Services;

namespace SupplySentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IAgentService, AgentService>();

        services.AddScoped<IDocumentReaderTool, PdfDocumentReaderTool>();
        services.AddScoped<IERPComparisonTool, MockERPComparisonTool>();
        services.AddScoped<IBCDataSyncTool, MockBCDataSyncTool>();
        
        return services;
    }
}