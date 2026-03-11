using Microsoft.Extensions.Configuration;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.ValueObjects;
using System.ClientModel;
using OpenAI.Chat;
using OpenAI;
using Microsoft.Agents.AI;
using System.ComponentModel;

namespace SupplySentinel.Infrastructure.Services;

public class AgentService : IAgentService
{
    #region Private DTOs for Structured Output

    public class AgentSyncProposalDto
    {
        public AgentVendorDto Vendor { get; set; } = new();
        public List<AgentProposedItemDto> Items { get; set; } = [];
    }

    public class AgentVendorDto
    {
        [Description("The official name of the supplier found on the document.")]
        public string Name { get; set; } = string.Empty;
    }

    public class AgentProposedItemDto
    {
        [Description("The unique item code or SKU. Extract exactly, e.g. '1896-S'. DO NOT include the product name here.")]
        public string Sku { get; set; } = string.Empty;

        [Description("The full description or name of the product.")]
        public string Description { get; set; } = string.Empty;

        public Money Price { get; set; } = new(0, string.Empty);
    }

    #endregion

    private readonly IDocumentReaderTool _documentReader;
    private readonly IERPComparisonTool _erpComparer;
    private readonly IConfiguration _configuration;

    public AgentService(
        IDocumentReaderTool documentReader,
        IERPComparisonTool erpComparer,
        IConfiguration configuration)
    {
        _documentReader = documentReader;
        _erpComparer = erpComparer;
        _configuration = configuration;
    }

    public async Task<Result<List<PriceConflict>>> AnalyzeDocumentAsync(byte[] documentContent, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(documentContent);
        string extractedText = await _documentReader.ExtractTextAsync(stream, "supplier_document.pdf", cancellationToken);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            return Result.Failure<List<PriceConflict>>(new Error("Doc.Empty", "The document is empty or could not be read."));
        }

        var proposalResult = await InterpretTextToProposalAsync(extractedText, cancellationToken);

        if (proposalResult.IsFailure)
        {
            return Result.Failure<List<PriceConflict>>(proposalResult.Error);
        }

        var proposalDto = proposalResult.Value;
        Console.WriteLine($"[AgentService] AI found {proposalDto.Items.Count} items.");

        var conflicts = new List<PriceConflict>();

        foreach (var proposedItem in proposalDto.Items)
        {
        
            string cleanSku = proposedItem.Sku.Trim();
            Console.WriteLine($"[AgentService] Checking SKU: {cleanSku}");

            var erpItemResult = await _erpComparer.GetItemBySkuAsync(cleanSku, cancellationToken);

            if (erpItemResult.IsFailure)
            {
                Console.WriteLine($"[AgentService] SKU search failed for '{cleanSku}': {erpItemResult.Error.Name}");
                continue;
            }

            var erpItem = erpItemResult.Value;
            var erpPrice = new Money(erpItem.UnitCost, "EUR");

            if (proposedItem.Price.Amount != erpPrice.Amount && proposedItem.Price.Amount > 0)
            {
                Console.WriteLine($"[AgentService] Conflict detected for {cleanSku}");
                var conflict = new PriceConflict(
                    Guid.NewGuid(),
                    erpItem.Id,
                    Guid.Empty,
                    proposedItem.Price,
                    erpPrice
                );
                conflicts.Add(conflict);
            }
        }

        return Result.Success(conflicts);
    }

    private async Task<Result<AgentSyncProposalDto>> InterpretTextToProposalAsync(string text, CancellationToken cancellationToken)
    {
        try
        {
            var apiKey = _configuration["LlmProvider:ApiKey"] ?? throw new InvalidOperationException("LLM API Key not configured.");
            var modelId = _configuration["LlmProvider:Model"] ?? "qwen-3-32b";
            var endpoint = _configuration["LlmProvider:Endpoint"] ?? "https://api.cerebras.ai/v1";

            
            var openAIClient = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(endpoint) });
            var chatClient = openAIClient.GetChatClient(modelId);

            
            var instructions = "You are a procurement analysis agent. Extract vendor and item data accurately.";
            AIAgent agent = chatClient.AsAIAgent(instructions);


            AgentResponse<AgentSyncProposalDto> response = await agent.RunAsync<AgentSyncProposalDto>(
            text,
            cancellationToken: cancellationToken);

            if (response.Result == null)
            {
                return Result.Failure<AgentSyncProposalDto>(new Error("Agent.EmptyResponse", "AI returned no data."));
            }

            return Result.Success(response.Result);
        }
        catch (Exception ex)
        {
            return Result.Failure<AgentSyncProposalDto>(new Error("Agent.Error", ex.Message));
        }
    }
}