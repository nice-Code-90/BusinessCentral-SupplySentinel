using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IBCDataSyncTool
{
    Task UpdateItemPriceAsync(Guid itemId, Money newPrice, CancellationToken cancellationToken = default);
    Task CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default);
}