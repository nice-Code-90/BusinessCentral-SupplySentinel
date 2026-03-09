using System.Net.Http.Json;
using System.Text.Json;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Errors;

namespace SupplySentinel.Infrastructure.REST;

public class ERPComparisonTool : IERPComparisonTool
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private record ODataResponse<T>(List<T> Value);
    private record BCItem(Guid Id, string Number, string DisplayName, string BaseUnitOfMeasureCode);
    private record BCVendor(Guid Id, string Number, string DisplayName, string CurrencyCode);

    public ERPComparisonTool(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Result<Item>> GetItemBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        
        var response = await _httpClient.GetAsync($"items?$filter=number%20eq%20'{sku}'", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            
            var errorBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(errorBody);

            return Result.Failure<Item>(ItemErrors.NotFound);
        }

        var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<BCItem>>(_jsonSerializerOptions, cancellationToken);
        var bcItem = odataResponse?.Value.FirstOrDefault();

        if (bcItem is null)
        {
            return Result.Failure<Item>(ItemErrors.NotFound);
        }

        var item = new Item(bcItem.Id, bcItem.Number, bcItem.DisplayName, bcItem.BaseUnitOfMeasureCode ?? "PCS");
        return Result.Success(item);
    }

    public async Task<Result<Vendor>> GetVendorByErpIdAsync(string erpId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"vendors?$filter=number eq '{erpId}'", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure<Vendor>(VendorErrors.NotFound);
        }

        var odataResponse = await response.Content.ReadFromJsonAsync<ODataResponse<BCVendor>>(_jsonSerializerOptions, cancellationToken);
        var bcVendor = odataResponse?.Value.FirstOrDefault();

        if (bcVendor is null)
        {
            return Result.Failure<Vendor>(VendorErrors.NotFound);
        }

        var vendor = new Vendor(bcVendor.Id, bcVendor.DisplayName, bcVendor.Number, bcVendor.CurrencyCode ?? "EUR", 0);
        return Result.Success(vendor);
    }
}