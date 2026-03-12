using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Domain.Factories;

public static class PriceConflictFactory
{
    public static PriceConflict? Create(
        Item erpItem,
        Money proposedPrice,
        Money currentErpPrice,
        Guid vendorId)
    {
        if (proposedPrice.Amount == currentErpPrice.Amount || proposedPrice.Amount <= 0)
        {
            return null;
        }

        decimal variance;
        if (currentErpPrice.Amount == 0 && proposedPrice.Amount > 0)
        {
            variance = 100.0m;
        }
        else if (currentErpPrice.Amount != 0)
        {
            variance = ((proposedPrice.Amount - currentErpPrice.Amount) / currentErpPrice.Amount) * 100;
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
            currentErpPrice,
            variance
            );
    }
}
