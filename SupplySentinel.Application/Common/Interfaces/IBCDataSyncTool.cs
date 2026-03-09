using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IBCDataSyncTool
{
    Task<Result> UpdateItemPriceAsync(Guid itemId, Money newPrice, CancellationToken cancellationToken = default);
    Task<Result> CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default);
}