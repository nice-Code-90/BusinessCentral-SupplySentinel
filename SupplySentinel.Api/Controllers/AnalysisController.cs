using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplySentinel.Application.Common.Interfaces;
using SupplySentinel.Application.UseCases.AnalyzeSupplierDocument;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IERPComparisonTool _erpComparisonTool;

    public AnalysisController(ISender sender, IERPComparisonTool erpComparisonTool)
    {
        _sender = sender;
        _erpComparisonTool = erpComparisonTool;
    }

    [HttpGet("items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetItems(CancellationToken cancellationToken)
    {
        var result = await _erpComparisonTool.GetItemsAsync(cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
    
    [HttpPost("analyze")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AnalyzeDocument(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new Error("Request.Validation", "No file was uploaded."));

        var allowedExtensions = new[] { ".pdf", ".xlsx", ".xls" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest(new Error("Request.InvalidType", "Only PDF and Excel files are supported."));

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);

            var command = new AnalyzeSupplierDocumentCommand(
                DocumentId: Guid.NewGuid(),
                DocumentContent: memoryStream.ToArray());
            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : BadRequest(result.Error);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest);
        }
    }
}
