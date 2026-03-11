using SupplySentinel.Application.Common.Constants;
using System.ComponentModel;

namespace SupplySentinel.Application.Common.Models.Agent
{
    public class AgentVendorDto
    {
        [Description(AgentConstants.VendorNameDescription)]
        public string Name { get; set; } = string.Empty;
    }
}
