using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Domain.Errors;

public static class ItemErrors
{
    public static readonly Error NotFound = new(
        "Item.NotFound",
        "The item with the specified SKU was not found in the ERP.");
}
