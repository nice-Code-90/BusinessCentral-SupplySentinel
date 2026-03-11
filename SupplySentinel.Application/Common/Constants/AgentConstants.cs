namespace SupplySentinel.Application.Common.Constants;

public static class AgentConstants
{
    public const string ProcurementAgentInstructions = "You are a procurement analysis agent. Extract vendor and item data accurately.";

    
    public const string VendorNameDescription = "The official name of the supplier found on the document.";

    
    public const string SkuDescription = "The unique item code or SKU. Extract exactly, e.g. '1896-S'. DO NOT include the product name here.";
    public const string DescriptionDescription = "The full description or name of the product.";
}
