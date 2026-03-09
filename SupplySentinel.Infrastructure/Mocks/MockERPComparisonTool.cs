using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Errors;


namespace SupplySentinel.Infrastructure.Mocks;

public class MockERPComparisonTool : IERPComparisonTool
{
    public Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (sku == "SKU-123")
        {
            var item = new Item(Guid.NewGuid(), "SKU-123", "Mock Item Description", "PCS");
            return Task.FromResult(Result.Success(item));
        }
        return Task.FromResult(Result.Failure<Item>(ItemErrors.NotFound));
    }

    public Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        var vendor = new Vendor(Guid.NewGuid(), "Mock Vendor Ltd.", erpId, "USD", 14);
        return Task.FromResult(Result.Success(vendor));
    }
}