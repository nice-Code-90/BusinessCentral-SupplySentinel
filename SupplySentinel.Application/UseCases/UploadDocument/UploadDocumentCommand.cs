using MediatR;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Application.UseCases.UploadDocument;

public record UploadDocumentCommand(string FileName, byte[] Content, string ContentType) : IRequest<Result<Guid>>;