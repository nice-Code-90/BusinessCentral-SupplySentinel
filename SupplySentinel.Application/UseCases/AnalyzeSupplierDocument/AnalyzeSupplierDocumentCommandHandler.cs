using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Factories;

namespace SupplySentinel.Application.UseCases.AnalyzeSupplierDocument;

public class AnalyzeSupplierDocumentCommandHandler : IRequestHandler<AnalyzeSupplierDocumentCommand, Result<List<PriceConflict>>>
{
    private readonly IDocumentReaderTool _documentReader;
    private readonly IAnalysisAgent _analysisAgent;
    private readonly IERPComparisonTool _erpComparer;

    public AnalyzeSupplierDocumentCommandHandler(
        IDocumentReaderTool documentReader,
        IAnalysisAgent analysisAgent,
        IERPComparisonTool erpComparer)
    {
        _documentReader = documentReader;
        _analysisAgent = analysisAgent;
        _erpComparer = erpComparer;
    }

    public async Task<Result<List<PriceConflict>>> Handle(AnalyzeSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(request.DocumentContent);
        string extractedText = await _documentReader.ExtractTextAsync(
            stream,
            "supplier_document.pdf",
            cancellationToken);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            return Result.Failure<List<PriceConflict>>(new Error("Doc.Empty", "A dokumentum üres vagy nem olvasható."));
        }
        var proposalResult = await _analysisAgent.GetProposalFromTextAsync(extractedText, cancellationToken);

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
                vendorId: Guid.Empty // vendor ID will be implemented in the future when we have vendor information in the proposal DTO
            );

            if (conflict != null)
            {
                conflicts.Add(conflict);
            }
        }

        return Result.Success(conflicts);
    }
}