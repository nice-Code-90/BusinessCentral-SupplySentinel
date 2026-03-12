using Microsoft.Extensions.DependencyInjection;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Infrastructure.REST;
using SupplySentinel.Infrastructure.Services;

namespace SupplySentinel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddScoped<IAnalysisAgent, AnalysisAgent>();

        services.AddScoped<IDocumentReaderTool, PdfDocumentReaderTool>();

        

        return services;
    }
}