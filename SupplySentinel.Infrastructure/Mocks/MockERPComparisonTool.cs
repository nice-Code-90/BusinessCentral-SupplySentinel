using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockERPComparisonTool : IERPComparisonTool
{
    public Task<Item?> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (sku == "SKU-123")
        {
            return Task.FromResult<Item?>(new Item(Guid.NewGuid(), "SKU-123", "Mock Item Description", "PCS"));
        }
        return Task.FromResult<Item?>(null);
    }

    public Task<Vendor?> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<Vendor?>(new Vendor(Guid.NewGuid(), "Mock Vendor Ltd.", erpId, "USD", 14));
    }
}