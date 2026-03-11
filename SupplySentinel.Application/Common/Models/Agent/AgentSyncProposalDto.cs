namespace SupplySentinel.Application.Common.Models.Agent
{
    public class AgentSyncProposalDto
    {
        public AgentVendorDto Vendor { get; set; } = new();
        public List<AgentProposedItemDto> Items { get; set; } = [];
    }
}
