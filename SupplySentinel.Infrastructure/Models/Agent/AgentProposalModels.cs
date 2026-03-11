using System.ComponentModel;
using SupplySentinel.Domain.ValueObjects;
using SupplySentinel.Infrastructure.Prompts;

namespace SupplySentinel.Infrastructure.Models.Agent;

public class AgentSyncProposalDto
{
    public AgentVendorDto Vendor { get; set; } = new();
    public List<AgentProposedItemDto> Items { get; set; } = [];
}

public class AgentVendorDto
{
    [Description(AgentConstants.VendorNameDescription)]
    public string Name { get; set; } = string.Empty;
}

public class AgentProposedItemDto
{
    [Description(AgentConstants.SkuDescription)]
    public string Sku { get; set; } = string.Empty;

    [Description(AgentConstants.DescriptionDescription)]
    public string Description { get; set; } = string.Empty;

    public Money Price { get; set; } = new(0, string.Empty);
}
