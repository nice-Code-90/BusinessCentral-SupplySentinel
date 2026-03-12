using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IERPComparisonTool
{
    Task<Result<List<Item>>> GetItemsAsync(CancellationToken cancellationToken = default);
    Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<Item>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default);
    Task<Result<Vendor>> GetVendorByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<Vendor>> GetVendorByIdAsync(Guid vendorId, CancellationToken cancellationToken = default);
    Task<Result<Money>> GetPurchasePriceAsync(Guid vendorId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<List<Vendor>>> GetVendorsAsync(CancellationToken cancellationToken = default);
}
