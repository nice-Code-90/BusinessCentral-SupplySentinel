using MediatR;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Application.UseCases.Documents;


public record AnalyzeSupplierDocumentCommand(Guid DocumentId) : IRequest<Result>;