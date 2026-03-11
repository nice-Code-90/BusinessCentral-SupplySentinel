using Microsoft.Agents.AI;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Factories;
using SupplySentinel.Infrastructure.Models.Agent;

namespace SupplySentinel.Infrastructure.Services;

public class AgentService : IAgentService
{
    private readonly IDocumentReaderTool _documentReader;
    private readonly IERPComparisonTool _erpComparer;
    private readonly IAgentProvider _agentProvider;

    public AgentService(
        IDocumentReaderTool documentReader,
        IERPComparisonTool erpComparer,
        IAgentProvider agentProvider)
    {
        _documentReader = documentReader;
        _erpComparer = erpComparer;
        _agentProvider = agentProvider;
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
        var conflicts = new List<PriceConflict>();

        foreach (var proposedItem in proposalDto.Items)
        {
            string cleanSku = proposedItem.Sku.Trim();

            var erpItemResult = await _erpComparer.GetItemBySkuAsync(cleanSku, cancellationToken);

            if (erpItemResult.IsFailure)
            {
                continue;
            }
            var conflict = PriceConflictFactory.Create(
                erpItem: erpItemResult.Value,
                proposedPrice: proposedItem.Price,
                vendorId: Guid.Empty // name based search will be implemented in the future, for now we can leave it empty
            );

            if (conflict != null)
            {
                conflicts.Add(conflict);
            }
        }

        return Result.Success(conflicts);
    }

    /// <summary>
    /// Using AIAgent to interpret the extracted text and generate a structured proposal.
    /// </summary>
    private async Task<Result<AgentSyncProposalDto>> InterpretTextToProposalAsync(string text, CancellationToken cancellationToken)
    {
        try
        {
            
            AIAgent agent = _agentProvider.GetAnalysisAgent();

            
            AgentResponse<AgentSyncProposalDto> response = await agent.RunAsync<AgentSyncProposalDto>(
                message: text,
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