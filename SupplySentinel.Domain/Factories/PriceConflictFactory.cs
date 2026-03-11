using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Domain.Factories;

public static class PriceConflictFactory
{
    public static PriceConflict? Create(
        Item erpItem,
        Money proposedPrice,
        Guid vendorId)
    {
        var erpPrice = new Money(erpItem.UnitCost, "EUR"); // Assuming EUR for now

        if (proposedPrice.Amount == erpPrice.Amount || proposedPrice.Amount <= 0)
        {
            return null;
        }

        decimal variance;
        if (erpPrice.Amount == 0 && proposedPrice.Amount > 0)
        {
            variance = 100.0m;
        }
        else if (erpPrice.Amount != 0)
        {
            variance = ((proposedPrice.Amount - erpPrice.Amount) / erpPrice.Amount) * 100;
        }
        else
        {
            return null; // Or handle as an error, edge case where both are zero.
        }

        return new PriceConflict(
            Guid.NewGuid(),
            erpItem.Id,
            vendorId,
            proposedPrice,
            erpPrice,
            variance
            );
    }
}
