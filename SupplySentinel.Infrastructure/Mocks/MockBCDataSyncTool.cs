using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockBCDataSyncTool : IBCDataSyncTool
{
    public Task UpdateItemPriceAsync(Guid itemId, Money newPrice, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}