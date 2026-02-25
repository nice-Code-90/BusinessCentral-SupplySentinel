using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupplySentinel.Application.UseCases.Documents;
using SupplySentinel.Domain.Abstractions;

namespace SupplySentinel.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ISender _sender;

    public DocumentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeDocument([FromBody] AnalyzeSupplierDocumentCommand command)
    {
        Result result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok(new { Message = "Document analysis started", DocumentId = command.DocumentId });
    }
}