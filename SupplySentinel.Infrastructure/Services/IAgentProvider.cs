using Microsoft.Agents.AI;

namespace SupplySentinel.Infrastructure.Services;

public interface IAgentProvider
{
    AIAgent GetAnalysisAgent();
}
