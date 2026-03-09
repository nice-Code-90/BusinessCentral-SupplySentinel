namespace SupplySentinel.Infrastructure.REST;

public class BusinessCentralOptions
{
    public const string SectionName = "BusinessCentral";

    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string Environment { get; set; } = "Sandbox"; 
    public string BaseUrl { get; set; } = "https://api.businesscentral.dynamics.com/v2.0/";
    public string FullApiUrl => $"{BaseUrl.TrimEnd('/')}/{TenantId}/{Environment}/api/v2.0/companies({CompanyId})/";
}