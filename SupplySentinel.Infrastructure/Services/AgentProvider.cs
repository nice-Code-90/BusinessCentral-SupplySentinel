using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Infrastructure.Prompts;
using System.ClientModel;

namespace SupplySentinel.Infrastructure.Services;

public class AgentProvider : IAgentProvider
{
    private readonly IConfiguration _configuration;

    public AgentProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public AIAgent GetAnalysisAgent()
    {
        var apiKey = _configuration["LlmProvider:ApiKey"] ?? throw new InvalidOperationException("LLM API Key not configured.");
        var modelId = _configuration["LlmProvider:Model"] ?? "qwen-3-32b";
        var endpoint = _configuration["LlmProvider:Endpoint"] ?? "https://api.cerebras.ai/v1";

        var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
        var chatClient = openAIClient.GetChatClient(modelId);

        AIAgent agent = chatClient.AsAIAgent(AgentConstants.ProcurementAgentInstructions);
        return agent;
    }
}
