using SupplySentinel.Domain.ValueObjects;

namespace SupplySentinel.Domain.Entities;

public class PriceConflict
{
    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid VendorId { get; private set; }
    public Money ProposedPrice { get; private set; }
    public Money CurrentErpPrice { get; private set; }
    public decimal VariancePercentage { get; private set; }

    public PriceConflict(Guid id, Guid itemId, Guid vendorId, Money proposedPrice, Money currentErpPrice)
    {
        Id = id;
        ItemId = itemId;
        VendorId = vendorId;
        ProposedPrice = proposedPrice;
        CurrentErpPrice = currentErpPrice;

        if (currentErpPrice.Amount != 0)
        {
            VariancePercentage = ((proposedPrice.Amount - currentErpPrice.Amount) / currentErpPrice.Amount) * 100;
        }
    }
}