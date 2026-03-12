using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Errors;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Infrastructure.REST;

public class ERPComparisonTool : IERPComparisonTool
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private record ODataResponse<T>(List<T> Value);
    private record BCItem(Guid Id, string Number, string DisplayName, string BaseUnitOfMeasureCode, decimal UnitCost);
    private record BCVendor(Guid Id, string Number, string DisplayName, string CurrencyCode);

    
    private record BCPurchasePrice(
        [property: JsonPropertyName("Asset_No")] string AssetNo,
        [property: JsonPropertyName("DirectUnitCost")] decimal UnitPrice,
        [property: JsonPropertyName("CurrencyCode")] string? CurrencyCode);

    public ERPComparisonTool(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<Money>> GetPurchasePriceAsync(Guid vendorId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var vendorResult = await GetVendorByIdAsync(vendorId, cancellationToken);
        var itemResult = await GetItemByIdAsync(itemId, cancellationToken);
        if (vendorResult.IsFailure || itemResult.IsFailure) return Result.Failure<Money>(new Error("BC.Data", "Mapping failed."));

        var vNo = vendorResult.Value.ErpId;
        var iNo = itemResult.Value.Sku;

        
        var url = $"../../../ODataV4/Company('Contoso')/PurchasePriceListLines?$filter=SourceNo eq '{vNo}'";
        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<BCPurchasePrice>>(_jsonSerializerOptions, cancellationToken);

            
            var priceRecord = odataResponse?.Value.FirstOrDefault(p => p.AssetNo == iNo);

            if (priceRecord != null)
            {
                return Result.Success(new Money(
                    priceRecord.UnitPrice,
                    !string.IsNullOrEmpty(priceRecord.CurrencyCode) ? priceRecord.CurrencyCode : vendorResult.Value.Currency));
            }
        }

        return Result.Failure<Money>(new Error("BC.PriceNotFound", "Nincs egyedi ár."));
    }

    
    public async Task<Result<List<Item>>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("items", cancellationToken);
        var odata = await response.Content.ReadFromJsonAsync<ODataResponse<BCItem>>(_jsonSerializerOptions, cancellationToken);
        return Result.Success(odata?.Value.Select(i => new Item(i.Id, i.Number, i.DisplayName, i.BaseUnitOfMeasureCode ?? "PCS", i.UnitCost)).ToList() ?? new List<Item>());
    }

    public async Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"items?$filter=number eq '{sku}'", cancellationToken);
        var odata = await response.Content.ReadFromJsonAsync<ODataResponse<BCItem>>(_jsonSerializerOptions, cancellationToken);
        var bc = odata?.Value.FirstOrDefault();
        return bc is null ? Result.Failure<Item>(ItemErrors.NotFound) : Result.Success(new Item(bc.Id, bc.Number, bc.DisplayName, bc.BaseUnitOfMeasureCode ?? "PCS", bc.UnitCost));
    }

    public async Task<Result<Item>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"items({id})", cancellationToken);
        var bc = await response.Content.ReadFromJsonAsync<BCItem>(_jsonSerializerOptions, cancellationToken);
        return bc is null ? Result.Failure<Item>(ItemErrors.NotFound) : Result.Success(new Item(bc.Id, bc.Number, bc.DisplayName, bc.BaseUnitOfMeasureCode ?? "PCS", bc.UnitCost));
    }

    public async Task<Result<Vendor>> GetVendorByIdAsync(Guid vendorId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"vendors({vendorId})", cancellationToken);
        var bc = await response.Content.ReadFromJsonAsync<BCVendor>(_jsonSerializerOptions, cancellationToken);
        return bc is null ? Result.Failure<Vendor>(VendorErrors.NotFound) : Result.Success(new Vendor(bc.Id, bc.DisplayName, bc.Number, bc.CurrencyCode ?? "EUR", 0));
    }

    public async Task<Result<List<Vendor>>> GetVendorsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("vendors", cancellationToken);
        var odata = await response.Content.ReadFromJsonAsync<ODataResponse<BCVendor>>(_jsonSerializerOptions, cancellationToken);
        return Result.Success(odata?.Value.Select(v => new Vendor(v.Id, v.DisplayName, v.Number, v.CurrencyCode ?? "EUR", 0)).ToList() ?? new List<Vendor>());
    }

    public async Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"vendors?$filter=number eq '{erpId}'", cancellationToken);
        var odata = await response.Content.ReadFromJsonAsync<ODataResponse<BCVendor>>(_jsonSerializerOptions, cancellationToken);
        var bc = odata?.Value.FirstOrDefault();
        return bc == null ? Result.Failure<Vendor>(VendorErrors.NotFound) : Result.Success(new Vendor(bc.Id, bc.DisplayName, bc.Number, bc.CurrencyCode ?? "EUR", 0));
    }

    public async Task<Result<Vendor>> GetVendorByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"vendors?$filter=displayName eq '{name}'", cancellationToken);
        var odata = await response.Content.ReadFromJsonAsync<ODataResponse<BCVendor>>(_jsonSerializerOptions, cancellationToken);
        var bc = odata?.Value.FirstOrDefault();
        return bc == null ? Result.Failure<Vendor>(VendorErrors.NotFound) : Result.Success(new Vendor(bc.Id, bc.DisplayName, bc.Number, bc.CurrencyCode ?? "EUR", 0));
    }
}