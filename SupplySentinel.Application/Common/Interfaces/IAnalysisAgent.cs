using SupplySentinel.Application.Common.Models.Agent;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Application.Common.Interfaces;

public interface IAnalysisAgent
{
    // Nem AIAgent-et adunk vissza, hanem rögtön az eredményt!
    Task<Result<AgentSyncProposalDto>> GetProposalFromTextAsync(string text, CancellationToken ct);
}