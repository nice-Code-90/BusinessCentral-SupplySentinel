using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockBCDataSyncTool : IBCDataSyncTool
{
    public Task<Result> UpdateItemPriceAsync(Guid itemId, Money newPrice, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result> CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}