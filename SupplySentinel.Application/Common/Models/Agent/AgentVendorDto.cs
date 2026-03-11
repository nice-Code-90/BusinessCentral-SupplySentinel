using SupplySentinel.Infrastructure.Prompts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SupplySentinel.Application.Common.Models.Agent
{
    public class AgentVendorDto
    {
        [Description(AgentConstants.VendorNameDescription)]
        public string Name { get; set; } = string.Empty;
    }
}
