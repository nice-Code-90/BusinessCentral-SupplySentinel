using SupplySentinel.Application.Common.Interfaces;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockDocumentReaderTool : IDocumentReaderTool
{
    public Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult($"[MOCK] Extracted text from {fileName}. Content: Item SKU-123, Price: 100 USD.");
    }
}