using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Application.Common.Models.Agent;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Infrastructure.Prompts;
using System.ClientModel;

namespace SupplySentinel.Infrastructure.Services;

public class AnalysisAgent : IAnalysisAgent
{
    private readonly IConfiguration _configuration;

    public AnalysisAgent(IConfiguration configuration)
    {
        _configuration = configuration;
    }

   
    public async Task<Result<AgentSyncProposalDto>> GetProposalFromTextAsync(string text, CancellationToken ct)
    {
        try
        {
            var agent = GetInternalAgent();
            var response = await agent.RunAsync<AgentSyncProposalDto>(
                message: text,
                cancellationToken: ct);

            if (response.Result == null)
            {
                return Result.Failure<AgentSyncProposalDto>(
                    new Error("Agent.Empty", "AI hasn't sent data."));
            }

            return Result.Success(response.Result);
        }
        catch (Exception ex)
        {
            return Result.Failure<AgentSyncProposalDto>(
                new Error("Agent.Error", ex.Message));
        }
    }

   
    private AIAgent GetInternalAgent()
    {
        var apiKey = _configuration["LlmProvider:ApiKey"] ?? throw new InvalidOperationException("LLM API Key not configured.");
        var modelId = _configuration["LlmProvider:Model"] ?? "qwen-3-32b";
        var endpoint = _configuration["LlmProvider:Endpoint"] ?? "https://api.cerebras.ai/v1";

        var openAIClient = new OpenAIClient(
            new ApiKeyCredential(apiKey),
            new OpenAIClientOptions { Endpoint = new Uri(endpoint) });

        var chatClient = openAIClient.GetChatClient(modelId);

        
        return chatClient.AsAIAgent(AgentConstants.ProcurementAgentInstructions);
    }
}