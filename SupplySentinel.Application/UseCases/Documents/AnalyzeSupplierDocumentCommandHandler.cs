using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;
using SupplySentinel.Domain.Enums;

namespace SupplySentinel.Application.UseCases.Documents;

public class AnalyzeSupplierDocumentCommandHandler : IRequestHandler<AnalyzeSupplierDocumentCommand, Result>
{
    private readonly IDocumentReaderTool _documentReader;
    //IRepository and IAgent will be added here later

    public AnalyzeSupplierDocumentCommandHandler(IDocumentReaderTool documentReader)
    {
        _documentReader = documentReader;
    }

    public async Task<Result> Handle(AnalyzeSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        
        var mockDocument = new SupplyDocument(request.DocumentId, "arlista.pdf", new byte[0], "Upload");

        
        using var stream = new MemoryStream(mockDocument.RawContent);
        string extractedText = await _documentReader.ExtractTextAsync(stream, mockDocument.FileName, cancellationToken);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            return Result.Failure(new Error("Document.Empty", "Failed to extract text from document."));
        }

        mockDocument.UpdateStatus(SyncStatus.Pending); 

        return Result.Success();
    }
}