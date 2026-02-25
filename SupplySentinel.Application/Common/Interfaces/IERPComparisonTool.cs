using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IERPComparisonTool
{
    Task<Item?> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Vendor?> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default);
}