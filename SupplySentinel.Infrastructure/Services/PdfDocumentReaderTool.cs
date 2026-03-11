using System.Text;
using SupplySentinel.Application.Common.Interfaces;
using UglyToad.PdfPig;

namespace SupplySentinel.Infrastructure.Services;

public class PdfDocumentReaderTool : IDocumentReaderTool
{
    public async Task<string> ExtractTextAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        try
        {
            return await Task.Run(() =>
            {
     
                using var document = PdfDocument.Open(fileStream);
                var sb = new StringBuilder();

                foreach (var page in document.GetPages())
                {
     
                    sb.AppendLine(page.Text);
                }

                return sb.ToString();
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PdfReader] Error reading PDF: {ex.Message}");
            return string.Empty;
        }
    }
}