using System.Net.Http.Json;
using System.Text.Json;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Infrastructure.REST;

public class BCDataSyncTool : IBCDataSyncTool
{
    private readonly HttpClient _httpClient;
    private readonly IERPComparisonTool _erpComparisonTool;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public BCDataSyncTool(HttpClient httpClient, IERPComparisonTool erpComparisonTool)
    {
        _httpClient = httpClient;
        _erpComparisonTool = erpComparisonTool;
    }

    public async Task<Result> UpdateItemPriceAsync(Guid vendorId, Guid itemId, Money newPrice, CancellationToken cancellationToken = default)
    {
        // 1. Data Retrieval
        var vendorResult = await _erpComparisonTool.GetVendorByIdAsync(vendorId, cancellationToken);
        var itemResult = await _erpComparisonTool.GetItemByIdAsync(itemId, cancellationToken);

        string vendorNo = vendorResult.IsSuccess ? vendorResult.Value.ErpId : string.Empty;
        string sku = itemResult.IsSuccess ? itemResult.Value.Sku : string.Empty;

        if (string.IsNullOrEmpty(sku))
            return Result.Failure(new Error("BC.Sync", "Required SKU is missing."));

        // 2. Context & URL Setup
        var companyInfo = await _httpClient.GetFromJsonAsync<JsonElement>("", cancellationToken);
        string companyName = companyInfo.GetProperty("name").GetString()!;
        string baseAddress = _httpClient.BaseAddress!.ToString();
        string oDataRoot = baseAddress.Substring(0, baseAddress.IndexOf("/api/v2.0/"));

        string headerUrl = $"{oDataRoot}/ODataV4/Company('{companyName}')/PurchasePriceLists";
        string linesUrl = $"{oDataRoot}/ODataV4/Company('{companyName}')/PurchasePriceLines";

        // 3. Resolve Target Price List (Vendor specific or P00001)
        string priceListCode = await ResolvePriceListCode(vendorNo, headerUrl, cancellationToken);

        
        string lineFilter = $"?$filter=Price_List_Code eq '{priceListCode}' and Asset_No eq '{sku}'&$top=1";
        var linesResponse = await _httpClient.GetFromJsonAsync<JsonElement>(linesUrl + lineFilter, cancellationToken);
        var existingLines = linesResponse.GetProperty("value").EnumerateArray();

        int lineNo;
        if (existingLines.Any())
        {
            lineNo = existingLines.First().GetProperty("Line_No").GetInt32();
        }
        else
        {
            // Create new line if it doesn't exist
            var createPayload = new { Price_List_Code = priceListCode, Asset_Type = "Item", Asset_No = sku };
            var postResponse = await _httpClient.PostAsJsonAsync(linesUrl, createPayload, _jsonSerializerOptions, cancellationToken);

            if (!postResponse.IsSuccessStatusCode)
                return Result.Failure(new Error("BC.Sync", "Failed to create new price line."));

            var createdLine = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
            lineNo = createdLine.GetProperty("Line_No").GetInt32();
        }

        // 5. Update the Price
        string patchUrl = $"{linesUrl}(Price_List_Code='{priceListCode}',Line_No={lineNo})";
        var patchRequest = new HttpRequestMessage(HttpMethod.Patch, patchUrl)
        {
            Content = JsonContent.Create(new { DirectUnitCost = newPrice.Amount }, options: _jsonSerializerOptions)
        };
        patchRequest.Headers.TryAddWithoutValidation("If-Match", "*");

        var response = await _httpClient.SendAsync(patchRequest, cancellationToken);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure(new Error("BC.Sync", "Failed to update price on the resolved line."));
    }

    private async Task<string> ResolvePriceListCode(string vendorNo, string headerUrl, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(vendorNo))
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonElement>($"{headerUrl}?$filter=SourceNo eq '{vendorNo}'&$top=1", ct);
                var list = response.GetProperty("value").EnumerateArray();
                if (list.Any()) return list.First().GetProperty("Code").GetString()!;
            }
            catch { /* Fallback to P00001 */ }
        }
        return "P00001";
    }

    public async Task<Result> CreatePurchaseOrderAsync(Guid vendorId, List<(Guid ItemId, int Quantity)> items, CancellationToken cancellationToken = default)
        => Result.Success();
}