using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Domain.Errors;

public static class ItemErrors
{
    
    public static readonly Error NotFound = new(
        "Item.NotFound",
        "The item with the specified SKU was not found in Business Central.");
}

public static class VendorErrors
{
    public static readonly Error NotFound = new(
        "Vendor.NotFound",
        "The vendor was not found in the ERP system.");
}