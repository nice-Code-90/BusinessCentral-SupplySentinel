using MediatR;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.AnalyzeSupplierDocument;

public class AnalyzeSupplierDocumentCommandHandler : IRequestHandler<AnalyzeSupplierDocumentCommand, Result<List<PriceConflict>>>
{
    private readonly IAgentService _agentService;

    public AnalyzeSupplierDocumentCommandHandler(IAgentService agentService)
    {
        _agentService = agentService;
    }

    public async Task<Result<List<PriceConflict>>> Handle(AnalyzeSupplierDocumentCommand request, CancellationToken cancellationToken)
    {
        return await _agentService.AnalyzeDocumentAsync(request.DocumentContent, cancellationToken);
    }
}
