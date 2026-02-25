namespace SupplySentinel.Domain.Entities;

public class Item
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string UnitOfMeasure { get; private set; } = string.Empty;

    public Item(Guid id, string sku, string description, string uom)
    {
        Id = id;
        Sku = sku;
        Description = description;
        UnitOfMeasure = uom;
    }
}