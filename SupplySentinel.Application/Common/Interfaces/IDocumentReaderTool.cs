namespace SupplySentinel.Application.Common.Interfaces;

public interface IDocumentReaderTool
{
    Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
}