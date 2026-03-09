using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Infrastructure.REST;

public class ERPComparisonTool : IERPComparisonTool
{
    private readonly HttpClient _httpClient;

    public ERPComparisonTool(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Item?> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call to Business Central
        return Task.FromResult<Item?>(null);
    }

    public Task<Vendor?> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call to Business Central
        return Task.FromResult<Vendor?>(null);
    }
}
