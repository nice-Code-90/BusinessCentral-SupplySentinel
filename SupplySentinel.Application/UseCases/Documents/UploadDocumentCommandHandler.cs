using MediatR;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.Documents;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    public Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        
        var documentId = Guid.NewGuid();
        var document = new SupplyDocument(documentId, request.FileName, request.Content, "API Upload");

        // TODO: _repository.Add(document);

        return Task.FromResult(Result.Success(documentId));
    }
}