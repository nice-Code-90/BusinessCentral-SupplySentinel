using SupplySentinel.Application.Common.Constants;
using SupplySentinel.Domain.ValueObjects;
using System.ComponentModel;

namespace SupplySentinel.Application.Common.Models.Agent
{
    public class AgentProposedItemDto
    {
        [Description(AgentConstants.SkuDescription)]
        public string Sku { get; set; } = string.Empty;

        [Description(AgentConstants.DescriptionDescription)]
        public string Description { get; set; } = string.Empty;

        public Money Price { get; set; } = new(0, string.Empty);
    }
}
