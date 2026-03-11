using SupplySentinel.Domain.Abstractions;
using SupplySentinel.Domain.Entities;

namespace SupplySentinel.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for the AI Agent service responsible for orchestrating document analysis.
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Analyzes a supplier document using an agentic workflow.
    /// </summary>
    /// <param name="documentContent">The raw byte content of the document (e.g., PDF, XLSX).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A Result containing a list of identified price conflicts or an error.</returns>
    Task<Result<List<PriceConflict>>> AnalyzeDocumentAsync(byte[] documentContent, CancellationToken cancellationToken);
}
