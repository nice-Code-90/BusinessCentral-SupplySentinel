using SupplySentinel.Application.Common.Interfaces;

public class MockDocumentReaderTool : IDocumentReaderTool
{
    public Task<string> ExtractTextAsync(
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        
        string mockContent = $@"
            Supplier: Global Coffee & Office Solutions
            File: {fileName}
            Date: 2026-03-11

            Price List:
            - Item: 1896-S, Description: ATHENS Desk, Price: 145.00 EUR
            - Item: 1900-S, Description: PARIS Guest Chair, Price: 100.00 EUR
            - Item: WRB-1000, Description: Colombia Coffee Beans, Price: 24.50 EUR
        ";

        return Task.FromResult(mockContent);
    }
}