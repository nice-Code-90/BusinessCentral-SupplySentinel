using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Domain.Errors;

public static class VendorErrors
{
    public static readonly Error NotFound = new(
        "Vendor.NotFound",
        "The vendor with the specified ERP ID was not found.");
}
