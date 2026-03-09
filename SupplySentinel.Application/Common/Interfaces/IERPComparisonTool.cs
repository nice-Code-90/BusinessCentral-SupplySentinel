using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IERPComparisonTool
{
    Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default);
}