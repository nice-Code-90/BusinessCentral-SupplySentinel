using Microsoft.Extensions.Configuration;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.ValueObjects;
using System.ClientModel;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI;

namespace SupplySentinel.Infrastructure.Services;

public class AgentService : IAgentService
{
    #region Private DTOs for Deserialization

    private class AgentSyncProposalDto
    {
        public AgentVendorDto Vendor { get; set; } = new();
        public List<AgentProposedItemDto> Items { get; set; } = [];
    }

    private class AgentVendorDto
    {
        public string Name { get; set; } = string.Empty;
    }

    private class AgentProposedItemDto
    {
        public string Sku { get; set; } = string.Empty;
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
        Console.WriteLine($"[AgentService] AI found {proposalDto.Items.Count} items in the document.");
        var conflicts = new List<PriceConflict>();

        
        foreach (var proposedItem in proposalDto.Items)
        {
            Console.WriteLine($"[AgentService] Checking SKU: {proposedItem.Sku}");
            var erpItemResult = await _erpComparer.GetItemBySkuAsync(proposedItem.Sku, cancellationToken);

            if (erpItemResult.IsFailure)
            {
                Console.WriteLine($"[AgentService] SKU search failed for '{proposedItem.Sku}': {erpItemResult.Error.Name}");
                continue;
            }

            var erpItem = erpItemResult.Value;

            var erpPrice = new Money(erpItem.UnitCost, "EUR");


            if (proposedItem.Price.Amount != erpPrice.Amount && proposedItem.Price.Amount > 0)
            {
                Console.WriteLine($"[AgentService] Conflict detected for SKU: {proposedItem.Sku}. Proposed: {proposedItem.Price.Amount}, ERP: {erpPrice.Amount}");
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

            var instructions = @"You are a procurement analysis agent. Your task is to extract data from supplier documents.
Return ONLY a JSON object with this structure:
{
  ""vendor"": { ""name"": ""string"" },
  ""items"": [
    { ""sku"": ""string"", ""description"": ""string"", ""price"": { ""amount"": 0.0, ""currency"": ""string"" } }
  ]
}";

            var chatHistory = new List<ChatMessage>
            {
                new SystemChatMessage(instructions),
                new UserChatMessage(text)
            };

            var options = new ChatCompletionOptions
            {
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
                Temperature = 0.0f
            };

            var response = await chatClient.CompleteChatAsync(chatHistory, options, cancellationToken);
            string rawJson = response.Value.Content[0].Text;

            
            if (rawJson.Contains("</think>"))
            {
                rawJson = rawJson.Split("</think>").Last();
            }

            
            rawJson = rawJson.Trim().Replace("```json", "").Replace("```", "").Trim();

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var proposal = JsonSerializer.Deserialize<AgentSyncProposalDto>(rawJson, jsonOptions);

            return proposal is not null
                ? Result.Success(proposal)
                : Result.Failure<AgentSyncProposalDto>(new Error("Agent.Deserialization", "Failed to parse AI response."));
        }
        catch (Exception ex)
        {
            return Result.Failure<AgentSyncProposalDto>(new Error("Agent.Error", ex.Message));
        }
    }
}