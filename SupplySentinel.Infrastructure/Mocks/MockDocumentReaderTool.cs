using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SupplySentinel.Application.Common.Interfaces;

namespace SupplySentinel.Infrastructure.Mocks;

public class MockDocumentReaderTool : IDocumentReaderTool
{
    public Task<string> ExtractTextAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        string mockContent = $@"
            Supplier: Contoso Electronics
            File: {fileName}
            Date: 2026-03-11

            Price List:
            - Item: C-1001, Description: Standard Keyboard, Price: 15.99 USD
            - Item: C-1002, Description: Optical Mouse, Price: 9.50 USD
            - Item: H-550, Description: HDMI Cable, 2m, Price: 7.25 USD
        ";

        return Task.FromResult(mockContent);
    }
}