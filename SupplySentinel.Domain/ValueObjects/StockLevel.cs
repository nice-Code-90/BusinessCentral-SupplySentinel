namespace SupplySentinel.Domain.ValueObjects;

public record StockLevel(decimal Current, decimal Threshold)
{
    public bool IsCritical => Current < Threshold;
}