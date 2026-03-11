using SupplySentinel.Domain.ValueObjects;
using SupplySentinel.Infrastructure.Prompts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

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
