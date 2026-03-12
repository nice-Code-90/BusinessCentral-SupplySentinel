using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Errors;
using SupplySentinel.Domain.Factories;
using SupplySentinel.Domain.ValueObjects;

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

        var vendorResult = await _erpComparer.GetVendorByNameAsync(proposalDto.Vendor.Name, cancellationToken);
        if (vendorResult.IsFailure)
        {
            return Result.Failure<List<PriceConflict>>(VendorErrors.NotFound);
        }
        var vendor = vendorResult.Value;

        var conflicts = new List<PriceConflict>();

        foreach (var proposedItem in proposalDto.Items)
        {
            string cleanSku = proposedItem.Sku.Trim();

            var erpItemResult = await _erpComparer.GetItemBySkuAsync(cleanSku, cancellationToken);

            if (erpItemResult.IsFailure)
            {
                // Item does not exist in ERP, we can't create a price conflict
                continue;
            }

            var erpItem = erpItemResult.Value;
            var erpPriceResult = await _erpComparer.GetPurchasePriceAsync(vendor.Id, erpItem.Id, cancellationToken);
            
            var currentErpPrice = erpPriceResult.IsSuccess 
                ? erpPriceResult.Value 
                : new Money(erpItem.UnitCost, vendor.Currency);

            if (erpPriceResult.IsFailure && erpPriceResult.Error.Code == "BC.Api.NotFound")
            {
                Console.WriteLine($"[Warning] Vendor-specific price not found for item {erpItem.Sku} and vendor {vendor.Name}. Falling back to item unit cost.");
            }

            var conflict = PriceConflictFactory.Create(
                erpItem: erpItem,
                proposedPrice: new Money(proposedItem.Price.Amount, vendor.Currency),
                currentErpPrice: currentErpPrice,
                vendorId: vendor.Id 
            );

            if (conflict != null)
            {
                conflicts.Add(conflict);
            }
        }

        return Result.Success(conflicts);
    }
}