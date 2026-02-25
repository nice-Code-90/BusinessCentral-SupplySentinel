namespace SupplySentinel.Domain.Entities;

public class Vendor
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ErpId { get; private set; } = string.Empty;
    public string Currency { get; private set; } = string.Empty;
    public int LeadTimeDays { get; private set; }

    public Vendor(Guid id, string name, string erpId, string currency, int leadTimeDays)
    {
        Id = id;
        Name = name;
        ErpId = erpId;
        Currency = currency;
        LeadTimeDays = leadTimeDays;
    }
}