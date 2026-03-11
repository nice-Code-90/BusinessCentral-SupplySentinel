using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Errors;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockERPComparisonTool : IERPComparisonTool
{
    private readonly List<Item> _items =
    [
        
        new(Guid.NewGuid(), "C-1001", "Standard Keyboard", "PCS", 12.50m),
        new(Guid.NewGuid(), "C-1002", "Optical Mouse", "PCS", 8.20m),
        new(Guid.NewGuid(), "SKU-123", "Mock Item Description", "PCS", 45.00m),
        
        new(Guid.NewGuid(), "1896-S", "ATHENS Desk", "PCS", 115.00m),
        new(Guid.NewGuid(), "1900-S", "PARIS Guest Chair", "PCS", 180.00m),
        new(Guid.NewGuid(), "WRB-1000", "Colombia Coffee Beans", "PCS", 22.00m)
    ];

    public Task<Result<List<Item>>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success(_items));
    }

    public Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        
        var item = _items.FirstOrDefault(i => i.Sku == sku);

        return item is not null
            ? Task.FromResult(Result.Success(item))
            : Task.FromResult(Result.Failure<Item>(ItemErrors.NotFound));
    }

    public Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        var vendor = new Vendor(Guid.NewGuid(), "Mock Vendor Ltd.", erpId, "USD", 14);
        return Task.FromResult(Result.Success(vendor));
    }
}