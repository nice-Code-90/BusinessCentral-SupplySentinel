using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Infrastructure.REST;

public class BCDataSyncTool : IBCDataSyncTool
{
    private readonly HttpClient _httpClient;

    public BCDataSyncTool(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<Result> UpdateItemPriceAsync(Guid itemId, Money newPrice, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call to Business Central
        return Task.FromResult(Result.Success());
    }

    public Task<Result> CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual API call to Business Central
        return Task.FromResult(Result.Success());
    }
}
