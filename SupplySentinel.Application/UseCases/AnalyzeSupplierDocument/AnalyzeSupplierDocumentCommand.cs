using MediatR;
using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.UseCases.AnalyzeSupplierDocument;

public record AnalyzeSupplierDocumentCommand(byte[] DocumentContent)
    : IRequest<Result<List<PriceConflict>>>;
